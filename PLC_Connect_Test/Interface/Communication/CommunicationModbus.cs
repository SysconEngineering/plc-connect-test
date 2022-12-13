using PLC_Connect_Test.Framework.Database.Manager;
using PLC_Connect_Test.Model.DTO;
using PLC_Connect_Test.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PLC_Connect_Test.Interface.Communication
{
    public class CommunicationModbus : ICommunication
    {
        #region Member
        private ModbusTcpClient _client;
        private DBManager _db = new DBManager();
        private int _startAddress = 40001;
        private int _numInput = 0;

        public Dictionary<int, PlcInfoResDto> dicDeviceAddr = new Dictionary<int, PlcInfoResDto>();
        public Dictionary<string, string> dicDeviceDataValue = new Dictionary<string, string>();
        public List<int> _writeRegisterList = new List<int>();
        //public Dictionary<string, ModbusOuterPlc> dicDevice = new Dictionary<string, ModbusOuterPlc>();

        public string Ip { get; set; }
        public int Port { get; set; }

        public bool IsConnected { get => _client?.IsConnected ?? false; }

        public DateTime LastConnectTime { get; set; } = DateTime.Now;
        public List<PlcInfoResDto> PlcInfo { get; set; } = new List<PlcInfoResDto>();
        System.DateTime ICommunication.LastConnectTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        #endregion

        #region 생성자
        public CommunicationModbus(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
        #endregion

        public void ConnectCheck()
        {
            throw new NotImplementedException();
        }

        public void DataRead()
        {
            if (_client != null)
            {
                try
                {
                    _client.OnResponseData -= new ModbusTcpClient.ResponseData(OnResponseData);
                    _client.OnResponseData += new ModbusTcpClient.ResponseData(OnResponseData);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void OnResponseData(ushort id, byte unit, byte function, byte[] values)
        {
            try
            {
                if (values.Count() == _numInput * 2)
                {
                    List<string> deviceStateData = new List<string>();

                    for (int i = 0; i < _numInput; i++)
                    {
                        var register = $"{_startAddress + i}";
                        //var targetDevice = PlcInfo?.FirstOrDefault(x => x.Dtl.FirstOrDefault(d => d.Address.ToString().Equals(register)) != null);
                        //var targetDeviceData = targetDevice?.Dtl.FirstOrDefault(x => x.Address.ToString().Equals(register));

                        var targetDevice = PlcInfo;
                        var targetDeviceData = Data.Instance.DeviceLiveData.FirstOrDefault(x => x.Key.Equals(register));

                        var data = $"{values[i * 2]}{values[i * 2 + 1]}";
                        int.TryParse(data, out var binary);

                        deviceStateData.Add($"{binary}");
                        // dic에 register 키가 있으면 덮어쓰기
                        if (dicDeviceDataValue.ContainsKey(register))
                        {
                            dicDeviceDataValue[register] = data;
                        }
                        else
                        {
                            //  없으면 신규 추가
                            dicDeviceDataValue.Add(register, data);
                        }
                    }
                    // 전역으로 사용할수 있게 넣음
                    Data.Instance.DeviceLiveData = dicDeviceDataValue;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void DataWrite()
        {
            throw new NotImplementedException();
        }

        public void ExtractWriteRegister()
        {

            List<PlcDataResDto> writeRegister = Data.Instance.PlcInfo.data.FindAll(x => x.readWrite.Equals("W"));
            foreach (PlcDataResDto data in writeRegister)
            {
                _writeRegisterList.Add(data.register);
            }
        }
        public void Start()
        {
            Console.WriteLine("CommunicationModbus start (IP:{0}, Port:{1})", Ip, Port);
            if (string.IsNullOrEmpty(Ip))
                return;
            _client = new ModbusTcpClient(Ip, Port);

            ExtractWriteRegister(); // Write Register 추출
            _numInput = Data.Instance.PlcInfo.data.Count;
            _client.Start((ushort)_numInput, _startAddress);

            DataRead();

            Thread DeviceCheck = new Thread(DeviceStreamCheck);
            DeviceCheck.Start();
        }

        public void DeviceStreamCheck()
        {
            try
            {
                while (true)
                {
                    StreamData();
                    Thread.Sleep(300);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void StreamData()
        {
            try
            {
                if (_client?.IsConnected == true)
                {

                    foreach (string key in Data.Instance.DeviceLiveData.Keys)
                    {
                        if (_writeRegisterList.Contains(Int32.Parse(key)))
                        {
                            DataWrite(key, "22");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Stop()
        {
            _client.OnResponseData -= new ModbusTcpClient.ResponseData(OnResponseData);
            _client.Stop();
        }

        public void DataWrite(string register, string value)
        {
            if (ushort.TryParse(register, out var startAddress)
                && byte.TryParse(value, out var values))
            {
                startAddress -= (ushort)_startAddress;
                _client?.WriteSingleRegister(7, 0, startAddress, GetData(values));
            }
        }
        private byte[] GetData(byte value)
        {
            byte[] data = new byte[2];
            data[0] = 0;
            data[1] = value;
            return data;
        }
    }
}
