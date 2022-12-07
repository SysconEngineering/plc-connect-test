using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PLC_Connect_Test.Interface
{
    /// <summary>
    /// Modbus TCP common driver class. 
    /// </summary>
    /// 
    /// This class implements a modbus TCP master driver. It supports the following commands:
    /// 
    /// Read coils
    /// Read discrete inputs
    /// Write single coil
    /// Write multiple cooils
    /// Read holding register
    /// Read input register
    /// Write single register
    /// Write multiple register
    /// 
    /// All commands can be sent in synchronous or asynchronous mode. If a value is accessed
    /// in synchronous mode the program will stop and wait for slave to response. If the 
    /// slave didn't answer within a specified time a timeout exception is called.
    /// The class uses multi threading for both synchronous and asynchronous access. For
    /// the communication two lines are created. This is necessary because the synchronous
    /// thread has to wait for a previous command to finish.
    /// The synchronous channel can be disabled during connection. This can be necessary when
    /// the slave only supports one connection.
    /// 
    public class ModbusTcpClient
    {
        // ------------------------------------------------------------------------
        // Constants for access
        private const byte fctReadCoil = 1;
        private const byte fctReadDiscreteInputs = 2;
        private const byte fctReadHoldingRegister = 3;
        private const byte fctReadInputRegister = 4;
        private const byte fctWriteSingleCoil = 5;
        private const byte fctWriteSingleRegister = 6;
        private const byte fctWriteMultipleCoils = 15;
        private const byte fctWriteMultipleRegister = 16;
        private const byte fctReadWriteMultipleRegister = 23;

        /// <summary>Constant for exception illegal function.</summary>
        public const byte excIllegalFunction = 1;
        /// <summary>Constant for exception illegal data address.</summary>
        public const byte excIllegalDataAdr = 2;
        /// <summary>Constant for exception illegal data value.</summary>
        public const byte excIllegalDataVal = 3;
        /// <summary>Constant for exception slave device failure.</summary>
        public const byte excSlaveDeviceFailure = 4;
        /// <summary>Constant for exception acknowledge. This is triggered if a write request is executed while the watchdog has expired.</summary>
        public const byte excAck = 5;
        /// <summary>Constant for exception slave is busy/booting up.</summary>
        public const byte excSlaveIsBusy = 6;
        /// <summary>Constant for exception gate path unavailable.</summary>
        public const byte excGatePathUnavailable = 10;
        /// <summary>Constant for exception not connected.</summary>
        public const byte excExceptionNotConnected = 253;
        /// <summary>Constant for exception connection lost.</summary>
        public const byte excExceptionConnectionLost = 254;
        /// <summary>Constant for exception response timeout.</summary>
        public const byte excExceptionTimeout = 255;
        /// <summary>Constant for exception wrong offset.</summary>
        private const byte excExceptionOffset = 128;
        /// <summary>Constant for exception send failt.</summary>
        private const byte excSendFailt = 100;

        private Socket _dataClientSocket = null;

        private Socket tcpAsyCl;
        private byte[] tcpAsyClBuffer = new byte[2048];

        private Socket tcpSynCl;
        private byte[] tcpSynClBuffer = new byte[2048];

        //private string _deviceCode = "";
        private string _ip = "";
        private int _port = 502;
        private Thread _connectCheckThread;
        private Thread _readDataThread;
        private ushort _id;
        private byte _unit;
        private ushort _startAddress;
        private ushort _numInputs;

        private bool _isConnectCheck = false;
        private bool _isConnected = false;
        private int _timeout = 10;
        public DateTime? LastConnectTime { get; set; } = new DateTime();

        // ------------------------------------------------------------------------
        /// <summary>Response data event. This event is called when new data arrives</summary>
        public delegate void ResponseData(ushort id, byte unit, byte function, byte[] data);//, string deviceCode);
        /// <summary>Response data event. This event is called when new data arrives</summary>
        public event ResponseData OnResponseData;
        /// <summary>Exception data event. This event is called when the data is incorrect</summary>
        public delegate void ExceptionData(ushort id, byte unit, byte function, byte exception);//, string deviceCode);
        /// <summary>Exception data event. This event is called when the data is incorrect</summary>
        public event ExceptionData OnException;

        // ------------------------------------------------------------------------
        /// <summary>Shows if a connection is active.</summary>
        public bool IsConnected
        {
            get
            {
                if (tcpAsyCl == null || tcpAsyCl.Connected == false)
                    return false;
                return true;
            }
        }

        // ------------------------------------------------------------------------
        /// <summary>Create master instance with parameters.</summary>
        /// <param name="ip">IP adress of modbus slave.</param>
        /// <param name="port">Port number of modbus slave. Usually port 502 is used.</param>
        public ModbusTcpClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void Start(ushort numInputs, int startAddr)
        {
            //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_DEBUG, string.Format("ModbusTcpClient Start (numInputs:{0})", numInputs));

            _id = 3;
            _unit = 0;
            _startAddress = (ushort)startAddr;
            _numInputs = numInputs;

            _isConnectCheck = true;
            if (_connectCheckThread != null)
            {
                _connectCheckThread = null;
            }
            _connectCheckThread = new Thread(OnConnectCheck);
            _connectCheckThread.Start();
        }
        /// <summary>
        /// Robot InnerDevice ConnectionCheck
        /// </summary>
        public void OnConnectCheck()
        {
            try
            {
                //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("OnConnectCheck thread start"));

                while (_isConnectCheck)
                {
                    //robot Data Recieve Timmer
                    if (LastConnectTime != null && LastConnectTime < DateTime.Now)
                    {
                        _isConnected = false;
                        Thread.Sleep(300);
                    }
                    else if (tcpAsyCl != null && tcpAsyCl.Connected == false)
                    {
                        _isConnected = false;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                    if (_isConnected == false)
                    {
                        DataDoConnect();
                        break;
                    }

                    Thread.Sleep(100);
                }

                //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("OnConnectCheck thread terminated"));
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
            }
        }
        /// <summary>
        /// Outer Device ConnectionCheck
        /// </summary>
        public void OnConnectCheckOuter()
        {
            try
            {
                //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("OnConnectCheck thread start"));

                if (LastConnectTime != null && LastConnectTime < DateTime.Now)
                {
                    _isConnected = false;
                    Thread.Sleep(300);
                }
                else if (tcpAsyCl != null && tcpAsyCl.Connected == false)
                {
                    _isConnected = false;
                    Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(10);
                }
                if (_isConnected == false)
                {
                    DataDoConnect();
                }

                Thread.Sleep(100);
                //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("OnConnectCheck thread terminated"));
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
            }
        }

        public void DataDoConnect()
        {
            try
            {
                _dataClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                DataBeginConnect();
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
                _isConnected = false;
            }
        }
        public void DataBeginConnect()
        {
            Console.WriteLine("InnerPlc 접속 대기중...");

            try
            {
                Thread.Sleep(100);

                LastConnectTime = DateTime.Now.AddSeconds(_timeout);
                _dataClientSocket.BeginConnect(_ip, _port, new AsyncCallback(ConnectCallBackData), _dataClientSocket);
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
                _isConnected = false;
                DataDoConnect();
            }
        }

        private void ConnectCallBackData(IAsyncResult IAR)
        {
            try
            {
                Socket tempSocket = (Socket)IAR.AsyncState;
                //IPEndPoint svrEP = (IPEndPoint)tempSocket.RemoteEndPoint;

                tempSocket.EndConnect(IAR);

                tcpAsyCl = tempSocket;

                if (tcpAsyCl.Connected)
                {
                    _isConnected = true;

                    //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("InnerPLCDevice connected!!"));

                    if (_readDataThread != null)
                    {
                        _readDataThread = null;
                    }

                    _readDataThread = new Thread(OnReadData);
                    _readDataThread.Start();
                }
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
                DataDoConnect();
            }
        }

        internal void CallException(ushort id, byte unit, byte function, byte exception)
        {
            if (tcpAsyCl == null || tcpSynCl == null) return;
            if (exception == excExceptionConnectionLost)
            {
                tcpSynCl = null;
                tcpAsyCl = null;
            }
            if (OnException != null) OnException(id, unit, function, exception);//, _deviceCode);
        }

        internal static UInt16 SwapUInt16(UInt16 inValue)
        {
            return (UInt16)(((inValue & 0xff00) >> 8) |
                     ((inValue & 0x00ff) << 8));
        }

        private void OnReadData()
        {
            try
            {
                //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("OnReadData is Start"));

                while (true)
                {
                    if (tcpAsyCl != null && tcpAsyCl.Connected)
                    {
                        ReadHoldingRegister(_id, _unit, _startAddress, _numInputs);

                        LastConnectTime = DateTime.Now.AddSeconds(_timeout);
                        Thread.Sleep(200);
                    }
                    else
                    {
                        //sleep-add
                        Thread.Sleep(200);
                    }
                }

                //SARI.Manager.LogManager.LogWrite(SARI.Framework.Common.Consts.LOGTYPE_INFOR, string.Format("OnReadData is Terminated"));
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
                _isConnected = false;
            }
        }

        public void ReadHoldingRegister(ushort id, byte unit, ushort startAddress, ushort numInputs)
        {
            if (numInputs > 125)
            {
                CallException(id, unit, fctReadHoldingRegister, excIllegalDataVal);
                return;
            }

            WriteAsyncData(CreateReadHeader(id, unit, startAddress, numInputs, fctReadHoldingRegister), id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write single register in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        public void WriteSingleRegister(ushort id, byte unit, ushort startAddress, byte[] values)
        {
            if (values.GetUpperBound(0) != 1)
            {
                CallException(id, unit, fctReadCoil, excIllegalDataVal);
                return;
            }
            byte[] data;
            data = CreateWriteHeader(id, unit, startAddress, 1, 1, fctWriteSingleRegister);
            data[10] = values[0];
            data[11] = values[1];
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        public void WriteMultipleRegister(ushort id, byte unit, ushort startAddress, byte[] values)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateWriteHeader(id, unit, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister);
            Array.Copy(values, 0, data, 13, values.Length);
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Write multiple registers in slave synchronous.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        /// <param name="result">Contains the result of the synchronous write.</param>
        public void WriteMultipleRegister(ushort id, byte unit, ushort startAddress, byte[] values, ref byte[] result)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateWriteHeader(id, unit, startAddress, Convert.ToUInt16(numBytes / 2), Convert.ToUInt16(numBytes + 2), fctWriteMultipleRegister);
            Array.Copy(values, 0, data, 13, values.Length);
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read/Write multiple registers in slave asynchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startReadAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="startWriteAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        public void ReadWriteMultipleRegister(ushort id, byte unit, ushort startReadAddress, ushort numInputs, ushort startWriteAddress, byte[] values)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctReadWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateReadWriteHeader(id, unit, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes / 2));
            Array.Copy(values, 0, data, 17, values.Length);
            WriteAsyncData(data, id);
        }

        // ------------------------------------------------------------------------
        /// <summary>Read/Write multiple registers in slave synchronous. The result is given in the response function.</summary>
        /// <param name="id">Unique id that marks the transaction. In asynchonous mode this id is given to the callback function.</param>
        /// <param name="unit">Unit identifier (previously slave address). In asynchonous mode this unit is given to the callback function.</param>
        /// <param name="startReadAddress">Address from where the data read begins.</param>
        /// <param name="numInputs">Length of data.</param>
        /// <param name="startWriteAddress">Address to where the data is written.</param>
        /// <param name="values">Contains the register information.</param>
        /// <param name="result">Contains the result of the synchronous command.</param>
        public void ReadWriteMultipleRegister(ushort id, byte unit, ushort startReadAddress, ushort numInputs, ushort startWriteAddress, byte[] values, ref byte[] result)
        {
            ushort numBytes = Convert.ToUInt16(values.Length);
            if (numBytes > 250)
            {
                CallException(id, unit, fctReadWriteMultipleRegister, excIllegalDataVal);
                return;
            }

            if (numBytes % 2 > 0) numBytes++;
            byte[] data;

            data = CreateReadWriteHeader(id, unit, startReadAddress, numInputs, startWriteAddress, Convert.ToUInt16(numBytes / 2));
            Array.Copy(values, 0, data, 17, values.Length);
            result = WriteSyncData(data, id);
        }

        // ------------------------------------------------------------------------
        // Create modbus header for read action
        private byte[] CreateReadHeader(ushort id, byte unit, ushort startAddress, ushort length, byte function)
        {
            byte[] data = new byte[12];

            byte[] _id = BitConverter.GetBytes((short)id);
            data[0] = _id[1];			    // Slave id high byte
            data[1] = _id[0];				// Slave id low byte
            data[5] = 6;					// Message size
            data[6] = unit;					// Slave address
            data[7] = function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = _adr[0];				// Start address
            data[9] = _adr[1];				// Start address
            byte[] _length = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)length));
            data[10] = _length[0];			// Number of data to read
            data[11] = _length[1];			// Number of data to read
            return data;
        }

        // ------------------------------------------------------------------------
        // Create modbus header for write action
        private byte[] CreateWriteHeader(ushort id, byte unit, ushort startAddress, ushort numData, ushort numBytes, byte function)
        {
            byte[] data = new byte[numBytes + 11];

            byte[] _id = BitConverter.GetBytes((short)id);
            data[0] = _id[1];				// Slave id high byte
            data[1] = _id[0];				// Slave id low byte
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(5 + numBytes)));
            data[4] = _size[0];				// Complete message size in bytes
            data[5] = _size[1];				// Complete message size in bytes
            data[6] = unit;					// Slave address
            data[7] = function;				// Function code
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            data[8] = _adr[0];				// Start address
            data[9] = _adr[1];				// Start address
            if (function >= fctWriteMultipleCoils)
            {
                byte[] _cnt = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numData));
                data[10] = _cnt[0];			// Number of bytes
                data[11] = _cnt[1];			// Number of bytes
                data[12] = (byte)(numBytes - 2);
            }
            return data;
        }

        // ------------------------------------------------------------------------
        // Create modbus header for read/write action
        private byte[] CreateReadWriteHeader(ushort id, byte unit, ushort startReadAddress, ushort numRead, ushort startWriteAddress, ushort numWrite)
        {
            byte[] data = new byte[numWrite * 2 + 17];

            byte[] _id = BitConverter.GetBytes((short)id);
            data[0] = _id[1];						// Slave id high byte
            data[1] = _id[0];						// Slave id low byte
            byte[] _size = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)(11 + numWrite * 2)));
            data[4] = _size[0];						// Complete message size in bytes
            data[5] = _size[1];						// Complete message size in bytes
            data[6] = unit;							// Slave address
            data[7] = fctReadWriteMultipleRegister;	// Function code
            byte[] _adr_read = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startReadAddress));
            data[8] = _adr_read[0];					// Start read address
            data[9] = _adr_read[1];					// Start read address
            byte[] _cnt_read = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numRead));
            data[10] = _cnt_read[0];				// Number of bytes to read
            data[11] = _cnt_read[1];				// Number of bytes to read
            byte[] _adr_write = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startWriteAddress));
            data[12] = _adr_write[0];				// Start write address
            data[13] = _adr_write[1];				// Start write address
            byte[] _cnt_write = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numWrite));
            data[14] = _cnt_write[0];				// Number of bytes to write
            data[15] = _cnt_write[1];				// Number of bytes to write
            data[16] = (byte)(numWrite * 2);

            return data;
        }

        // ------------------------------------------------------------------------
        // Write asynchronous data
        private void WriteAsyncData(byte[] write_data, ushort id)
        {
            if (tcpAsyCl != null && tcpAsyCl.Connected)
            {
                try
                {
                    tcpAsyCl.BeginSend(write_data, 0, write_data.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                    //IAsyncResult result = tcpAsyCl.BeginSend(write_data, 0, write_data.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                    //Thread.Sleep(1);

                    //// Wait for the WaitHandle to become signaled.
                    //result.AsyncWaitHandle.WaitOne();
                }
                catch (SystemException ex)
                {
                    //Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    //Get the top stack frame
                    var frame = st.GetFrame(0);
                    //Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    //Inner exception message
                    var innerException = ex.InnerException != null ? ex.InnerException.ToString() : "";

                    //SARI.Manager.LogManager.ErrLogWrite(ex.Message.ToString(), string.Format("Line:{0}, Func:{1}, InnerException:{2}, StackTrace:{3}", line.ToString(), frame.GetMethod().Name, innerException.ToString(), st.ToString()));

                    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                }
            }
            else CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
        }

        // ------------------------------------------------------------------------
        // Write asynchronous data acknowledge
        private void OnSend(IAsyncResult result)
        {
            try
            {
                if (tcpAsyCl == null) return;

                int size = tcpAsyCl.EndSend(result);
                if (result.IsCompleted == false)
                    CallException(0xFFFF, 0xFF, 0xFF, excSendFailt);
                else
                {
                    tcpAsyClBuffer.Initialize();
                    tcpAsyCl.BeginReceive(tcpAsyClBuffer, 0, tcpAsyClBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), tcpAsyCl);
                }
            }
            catch (Exception ex)
            {
                //SARI.Manager.LogManager.ExpLogWrite(ex);
            }
        }

        // ------------------------------------------------------------------------
        // Write asynchronous data response
        private void OnReceive(IAsyncResult result)
        {
            if (tcpAsyCl == null) return;

            try
            {
                tcpAsyCl.EndReceive(result);
                if (result.IsCompleted == false) CallException(0xFF, 0xFF, 0xFF, excExceptionConnectionLost);
            }
            catch (Exception) { }

            ushort id = SwapUInt16(BitConverter.ToUInt16(tcpAsyClBuffer, 0));
            byte unit = tcpAsyClBuffer[6];
            byte function = tcpAsyClBuffer[7];
            byte[] data;

            // ------------------------------------------------------------
            // Write response data
            if ((function >= fctWriteSingleCoil) && (function != fctReadWriteMultipleRegister))
            {
                data = new byte[2];
                Array.Copy(tcpAsyClBuffer, 10, data, 0, 2);
            }
            // ------------------------------------------------------------
            // Read response data
            else
            {
                data = new byte[tcpAsyClBuffer[8]];
                Array.Copy(tcpAsyClBuffer, 9, data, 0, tcpAsyClBuffer[8]);
            }
            // ------------------------------------------------------------
            // Response data is slave exception
            if (function > excExceptionOffset)
            {
                function -= excExceptionOffset;
                CallException(id, unit, function, tcpAsyClBuffer[8]);
            }
            // ------------------------------------------------------------
            // Response data is regular data
            else if (OnResponseData != null) OnResponseData(id, unit, function, data);//, _deviceCode);
        }

        // ------------------------------------------------------------------------
        // Write data and and wait for response
        private byte[] WriteSyncData(byte[] write_data, ushort id)
        {

            if (tcpSynCl.Connected)
            {
                try
                {
                    tcpSynCl.Send(write_data, 0, write_data.Length, SocketFlags.None);
                    int result = tcpSynCl.Receive(tcpSynClBuffer, 0, tcpSynClBuffer.Length, SocketFlags.None);

                    byte unit = tcpSynClBuffer[6];
                    byte function = tcpSynClBuffer[7];
                    byte[] data;

                    if (result == 0) CallException(id, unit, write_data[7], excExceptionConnectionLost);

                    // ------------------------------------------------------------
                    // Response data is slave exception
                    if (function > excExceptionOffset)
                    {
                        function -= excExceptionOffset;
                        CallException(id, unit, function, tcpSynClBuffer[8]);
                        return null;
                    }
                    // ------------------------------------------------------------
                    // Write response data
                    else if ((function >= fctWriteSingleCoil) && (function != fctReadWriteMultipleRegister))
                    {
                        data = new byte[2];
                        Array.Copy(tcpSynClBuffer, 10, data, 0, 2);
                    }
                    // ------------------------------------------------------------
                    // Read response data
                    else
                    {
                        data = new byte[tcpSynClBuffer[8]];
                        Array.Copy(tcpSynClBuffer, 9, data, 0, tcpSynClBuffer[8]);
                    }
                    return data;
                }
                catch (SystemException)
                {
                    CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
                }
            }
            else CallException(id, write_data[6], write_data[7], excExceptionConnectionLost);
            return null;
        }
    }
}
