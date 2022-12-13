using PLC_Connect_Test.Framework.Database.Manager;
using PLC_Connect_Test.Interface;
using PLC_Connect_Test.Model.DTO;
using PLC_Connect_Test.Structure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace PLC_Connect_Test.OuterPLC
{
    public class Mitusbishi_Client_Socket : OuterBase
    {
        public event PLCDataResponse PLCdataEvt;
        public delegate void PLCDataResponse(Dictionary<string, OuterPlc> data);
        public event StatusResponse StatusResult_Evt;
        public delegate void StatusResponse(string ip, bool conn, string message);

        public event OuterPlcInfoDataEvt outerPlcInfoData_Evt;
        public delegate void OuterPlcInfoDataEvt(Dictionary<string, OuterPlc> values);


        private int headerLength;
        private byte[] header;
        private byte[] recvHeader;

        private int START_ADDR = 0;
        private int LAST_ADDR = 0;
        private string AREA = "";
        private int READ_CNT = 0;
        private int wordCnt = 0;
        private string wordReadLen = "FF";
        private int wordGoodLen = 0;
        private string oldReadWord = "";
        private bool bSocketError = true;
        private bool _connectYn = false;

        private DBManager _db = new DBManager();

        public Dictionary<int, PlcDataResDto> dicOuterPlcAddr = new Dictionary<int, PlcDataResDto>();
        public Dictionary<string, byte> dicDeviceCode = new Dictionary<string, byte>();
        public Dictionary<string, OuterPlc> dicOuterPlc = new Dictionary<string, OuterPlc>();
        PLCProtocol PLCProtocolType = PLCProtocol.PT_QnA;

        public Mitusbishi_Client_Socket(string ip, int port, string area)
            : base(ip, port)
        {
            AREA = area;
        }

        public override void AddPlc(string name, int plc_type)
        {
            this.plc_type = plc_type;

            //List<TbPlcInfoDtl> dtls = _db.PlcController.GetPlcInfoDtl(idx);
            List<PlcDataResDto> dtls = Data.Instance.PlcInfo.data;

            if (!dicOuterPlc.ContainsKey(name))
            {
                dicOuterPlc.Add(name, new OuterPlc(name, AREA, dtls));
            }

            if (dtls.Count > 0)
            {
                foreach (PlcDataResDto dtl in dtls)
                {
                    if (dtl.register != -1)
                    {
                        dtl.plcName = name;
                        dicOuterPlcAddr.Add(dtl.register, dtl);
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Processiong()
        {
            base.Processiong();
        }

        public override void Start()
        {
            if (dicOuterPlc.Count > 0)
            {
                if (dicOuterPlcAddr.Keys.Count == 0)
                {
                    START_ADDR = 0;
                    LAST_ADDR = 0;
                }
                else
                {
                    START_ADDR = dicOuterPlcAddr.Keys.Min();
                    LAST_ADDR = dicOuterPlcAddr.Keys.Max();
                }
                READ_CNT = LAST_ADDR - START_ADDR + 1;
                onDataSocketInit();
            }
        }

        public override void Stop()
        {
            if (clientsock_data != null)
            {
                if (clientsock_data.Connected)
                {
                    clientsock_data.Shutdown(SocketShutdown.Both);
                    clientsock_data.Close();
                }
            }

            if (ServerConnect_Checkthread != null)
            {
                ServerConnect_Checkthread = null;
            }
            if (DataRecvThread != null)
            {
                DataRecvThread = null;
            }
            if (DataRequestThread != null)
            {
                DataRequestThread = null;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Write(string plc_name, string address, string value)
        {
            string addNum = Regex.Replace(address, @"\D", "");
            SetPLCWord(plc_name, address, dicOuterPlcAddr[Convert.ToInt32(addNum)].dataType, value);
        }

        public void onDataSocketInit()
        {
            PLCProtocolType = PLCProtocol.PT_QnA;
            wordReadLen = READ_CNT.ToString("X");

            #region ## Init Header
            headerLength = 11;
            header = new byte[headerLength];
            header[0] = 0x50;    //서브 헤더 L
            header[1] = 0x00;    //서브 헤더 H
            header[2] = 0x00;    //네트워크 번호
            header[3] = 0xFF;    //PLC번호
            header[4] = 0xFF;    //요구상대모듈 I/O번호 L
            header[5] = 0x03;    //요구상대모듈 I/O번호 H
            header[6] = 0x00;    //요구상대모듈 국번호 
            header[7] = 0x0C;    //요구데이터 길이 L
            header[8] = 0x00;    //요구데이터 길이 H
            header[9] = 0x10;    //CPU감시타이머 L
            header[10] = 0x00;   //CPU감시타이머 H

            recvHeader = new byte[headerLength];
            recvHeader[0] = 0xD0;
            recvHeader[1] = 0x00;
            recvHeader[2] = 0x00;
            recvHeader[3] = 0xFF;
            recvHeader[4] = 0xFF;
            recvHeader[5] = 0x03;
            recvHeader[6] = 0x00;
            recvHeader[7] = 0x00;
            recvHeader[8] = 0x00;
            recvHeader[9] = 0x00;
            recvHeader[10] = 0x00;
            #endregion

            RunInitialize();

            if (ServerConnect_Checkthread != null)
            {
                ServerConnect_Checkthread = null;
            }
            bConnectCheck = true;

            // 서버 연결
            ServerConnect_Checkthread = new Thread(onConnectChkThread);
            ServerConnect_Checkthread.Name = this.GetType().Name + ": onCoonConnectChkThread";
            ServerConnect_Checkthread.IsBackground = true;
            ServerConnect_Checkthread.Start();

            // data recive 스레드 연결
            DataRecvThread = new Thread(onDataRecvThread);
            DataRecvThread.Name = this.GetType().Name + ": onDataRecvThread";
            DataRecvThread.IsBackground = true;
            DataRecvThread.Start();

            DataRequestThread = new Thread(onDataResponseThread);
            DataRequestThread.Name = this.GetType().Name + ": onDataResponseThread";
            DataRequestThread.IsBackground = true;
            DataRequestThread.Start();

        }

        private void RunInitialize()
        {
            // Set Device Code
            dicDeviceCode.Add("B", 0XA0);
            dicDeviceCode.Add("D", 0xA8);
            dicDeviceCode.Add("M", 0x90);
            dicDeviceCode.Add("R", 0xAF);
            dicDeviceCode.Add("W", 0xB4);
            dicDeviceCode.Add("X", 0x9C);

            wordGoodLen = 20;
            oldReadWord = "";
            ipep = new IPEndPoint(IPAddress.Parse(HOST), PORT);
        }

        #region Thread
        private void onConnectChkThread()
        {
            try
            {
                while (bConnectCheck)
                {
                    Thread.Sleep(1000);
                    if (bSocketError)
                    {
                        try
                        {
                            if (clientsock_data != null)
                            {
                                if (clientsock_data.Connected)
                                {
                                    // 에러났는데 연결되어있으면 shutdown
                                    clientsock_data.Shutdown(SocketShutdown.Both);
                                    clientsock_data.Close();
                                }
                            }
                            // 신규 연결
                            clientsock_data = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            clientsock_data.Connect(ipep);
                            bSocketError = false;
                            _connectYn = true;
                        }
                        catch (SocketException ex)
                        {
                            if (StatusResult_Evt != null)
                                StatusResult_Evt(HOST, false, string.Format("[{0}] PLC접속 실패", HOST));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("onConnectCheck err :={0}", ex.Message.ToString());
                if (StatusResult_Evt != null)
                    StatusResult_Evt(HOST, false, string.Format("[{0}] PLC접속 실패", HOST));
            }
        }

        private void onDataResponseThread()
        {
            try
            {
                while (true)
                {
                    if (bConnectCheck)
                    {
                        Thread.Sleep(500);

                        if (clientsock_data == null) continue;
                        if (!clientsock_data.Connected) continue;

                        GetPLCWord(string.Format("{0:D4}", START_ADDR));
                    }
                    else
                    {
                        break;
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("onDataRecvThread err :={0}", ex.Message.ToString());
            }
        }

        private void onDataRecvThread()
        {
            int maxLength = READ_CNT * 2;
            byte[] recvData;

            // 응답받을 데이터 크기 세팅 : (40바이트 * 설비수) + 2(완료데이터?)
            byte[] recvLength = BitConverter.GetBytes(maxLength + 2);
            recvHeader[7] = recvLength[0];
            if (recvLength.Length > 1)
            {
                recvHeader[8] = recvLength[1];
            }

            while (true)
            {
                try
                {
                    if (clientsock_data != null && dicOuterPlc != null && clientsock_data.Connected)
                    {
                        int availableLength = clientsock_data.Available;
                        if (availableLength > headerLength)
                        {
                            recvData = new byte[availableLength];
                            clientsock_data.Receive(recvData);

                            bool isCorrectHeader = true;
                            // 헤더 체크
                            for (int i = 0; i < headerLength; i++)
                            {
                                if (recvHeader[i] != recvData[i])
                                {
                                    isCorrectHeader = false;
                                    break;
                                }
                            }

                            if (isCorrectHeader == false)
                            {
                                continue;
                            }

                            // 수신 데이터 저장
                            byte[] useRecvData = new byte[maxLength];
                            Array.Copy(recvData, headerLength, useRecvData, 0, maxLength);

                            string headerWord = string.Empty;
                            string newWord = string.Empty;
                            string tmpWord;
                            string binary;

                            for (int i = 0; i < useRecvData.Length; i++)
                            {
                                newWord += string.Format("{0:X}", useRecvData[i]).PadLeft(2, '0');
                            }

                            oldReadWord = newWord;
                            for (int i = 0; i < READ_CNT; i++)
                            {
                                string plcName;

                                tmpWord = newWord.Substring(4 * i + 2, 2);
                                tmpWord += newWord.Substring(4 * i, 2);
                                binary = int.Parse(tmpWord, NumberStyles.HexNumber).ToString();

                                //int addr = START_ADDR + (i - (i % WordCnt));
                                int addr = START_ADDR + i;
                                if (dicOuterPlcAddr.ContainsKey(addr))
                                {
                                    plcName = dicOuterPlcAddr[addr].plcName;
                                }
                                else
                                {
                                    continue;
                                }

                                var outerPlc = dicOuterPlc[plcName];
                                for (int p = 0; p < outerPlc.info.Count; p++)
                                {
                                    if (outerPlc.info[p].addr.Substring(1).Equals(addr.ToString()))
                                    {
                                        if (outerPlc.info[p].plcDataType == OuterPlcInfo.PlcDataType.Binary)
                                        {
                                            outerPlc.info[p].value = binary;
                                        }
                                        else if (outerPlc.info[p].plcDataType == OuterPlcInfo.PlcDataType.ASCII)
                                        {
                                            outerPlc.info[p].value = WordToASCII(newWord, i);
                                        }
                                    }
                                }

                            }

                            PLCdataEvt(dicOuterPlc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Logger.Error($"onDataRecvThread 오류 : {ex.Message}");
#if LOGWRITE_TO_FILE
                    //Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    //Get the top stack frame
                    var frame = st.GetFrame(0);
                    //Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();

                    LogAgent.ErrLogWrite(ex.Message.ToString(), string.Format("Line:{0}, Func:{1}, StackTrace:{2}", line.ToString(), frame.GetMethod().Name, st.ToString()));
#endif
                }
                finally
                {
                    Thread.Sleep(50);
                }
            }
        }
        #endregion

        #region Converter
        private bool GetPLCWord(string addr)
        {
            string plcName = HOST;

            try
            {
                byte[] wSend;

                if (addr == "") return false;

                byte[] tmp = BitConverter.GetBytes(Convert.ToInt32(addr));

                wSend = new byte[21];

                // 헤더부
                wSend[0] = header[0];    //서브 헤더 L
                wSend[1] = header[1];    //서브 헤더 H
                wSend[2] = header[2];    //네트워크 번호
                wSend[3] = header[3];    //PLC번호
                wSend[4] = header[4];    //요구상대모듈 I/O번호 L
                wSend[5] = header[5];    //요구상대모듈 I/O번호 H
                wSend[6] = header[6];    //요구상대모듈 국번호 
                wSend[7] = header[7];    //요구데이터 길이 L
                wSend[8] = header[8];    //요구데이터 길이 H
                wSend[9] = header[9];    //CPU감시타이머 L
                wSend[10] = header[10];   //CPU감시타이머 H

                //커맨드 부
                wSend[11] = 0x01;    //커맨드 L
                wSend[12] = 0x04;    //커맨드 H
                wSend[13] = 0x00;    //서브 커맨드 L
                wSend[14] = 0x00;    //서브 커맨드 H
                wSend[15] = tmp[0];  //선두디바이스1
                wSend[16] = tmp[1];  //선두디바이스2
                wSend[17] = tmp[2];   //선두디바이스3
                wSend[18] = dicDeviceCode[AREA];    // 디바이스 코드 (영역구분 EX=D)
                wSend[19] = (byte)Int16.Parse(wordReadLen, NumberStyles.HexNumber);     //디바이스점수L
                wSend[20] = 0x00;   //디바이스점수R

                clientsock_data.Send(wSend);

                return true;
            }
            catch (SocketException ex1)
            {
                bSocketError = true;
                //Logger.Error($"Mit GetPLCWord({plcName}) : {ex1.Message}");
                return false;
            }
            catch (Exception ex)
            {
                //Logger.Error($"Mit GetPLCWord({plcName}) : {ex.Message}");
                return false;
            }
        }
        private bool SetPLCWord(string plc_name, string address, int dataType, string value)
        {
            try
            {
                byte[] wSend;
                byte[] wRecv;
                int wRetry = 0;
                int i_RecvCnt = 0;

                byte[] bAddress = BitConverter.GetBytes(Convert.ToInt32(address.Substring(1)));
                byte[] tmpByte;

                if (string.IsNullOrEmpty(value))
                {
                    string strError = "SetPLCWord-" + address + ":Value Length Error(1~256)";
                    //lbErrorSet(strError);
                    return false;
                }

                // ASCII
                if ((OuterPlcInfo.PlcDataType)dataType == OuterPlcInfo.PlcDataType.ASCII)
                {
                    value = ASCIIToWord(value);
                }

                if (PLCProtocolType == PLCProtocol.PT_A)
                {
                    wSend = new byte[12 + (2)];
                    wSend[0] = 0x03;    //선언부
                    wSend[1] = 0xff;    //PC번호
                    wSend[2] = 0x0a;    //CPU감시타이머1
                    wSend[3] = 0x00;    //CPU감시타이머2
                    wSend[4] = bAddress[0];
                    wSend[5] = bAddress[1];
                    wSend[6] = 0x00;    //선두디바이스3
                    wSend[7] = 0x00;    //선두디바이스4
                    wSend[8] = 0x20;    //선두디바이스5
                    wSend[9] = 0x44;    //선두디바이스6(영역구분 EX=D)

                    wSend[10] = (byte)1;   //디바이스점수
                    wSend[11] = 0x00;   //종료

                    tmpByte = BitConverter.GetBytes(Convert.ToInt16(value));
                    wSend[12] = tmpByte[0];
                    wSend[13] = tmpByte[1];

                    i_RecvCnt = 2;
                }
                else
                {
                    wSend = new byte[21 + (2)];
                    byte[] tmpByte2 = BitConverter.GetBytes(12 + (2));
                    // 헤더부
                    wSend[0] = 0x50;    //서브 헤더 L
                    wSend[1] = 0x00;    //서브 헤더 H
                    wSend[2] = 0x00;    //네트워크 번호
                    wSend[3] = 0xFF;    //PLC번호
                    wSend[4] = 0xFF;    //요구상대모듈 I/O번호 L
                    wSend[5] = 0x03;    //요구상대모듈 I/O번호 H
                    wSend[6] = 0x00;    //요구상대모듈 국번호 


                    wSend[7] = tmpByte2[0];    //요구데이터 길이 L
                    wSend[8] = tmpByte2[1];    //요구데이터 길이 H


                    wSend[9] = 0x10;    //CPU감시타이머 L
                    wSend[10] = 0x00;   //CPU감시타이머 H

                    //커맨드 부
                    wSend[11] = 0x01;    //커맨드 L
                    wSend[12] = 0x14;    //커맨드 H
                    wSend[13] = 0x00;    //서브 커맨드 L
                    wSend[14] = 0x00;    //서브 커맨드 H
                    wSend[15] = bAddress[0];  //선두디바이스1
                    wSend[16] = bAddress[1];  //선두디바이스2
                    wSend[17] = 0x00;    //선두디바이스3
                    wSend[18] = dicDeviceCode[AREA];    // 디바이스 코드 (영역구분 EX=D)
                    wSend[19] = (byte)1;   //디바이스점수 L
                    wSend[20] = 0x00;   //디바이스점수 R

                    tmpByte = BitConverter.GetBytes(Convert.ToInt16(value));
                    wSend[21] = tmpByte[0];
                    wSend[22] = tmpByte[1];

                    i_RecvCnt = 11;
                }
                if (clientsock_data == null)
                    return false;

                clientsock_data.Send(wSend);

                return true;
            }
            catch (SocketException ex1)
            {
                string strError = "SetPLCWord-" + ex1.Message;
                //lbErrorSet(strError);
                bSocketError = true;
                //Logger.Info(strError);
                return false;
            }
            catch (Exception ex)
            {
                string strError = "SetPLCWord-" + ex.Message;
                //lbErrorSet(strError);
                //Logger.Info(strError);
                return false;
            }
        }

        private string ASCIIToWord(string ascii)
        {
            string word;
            string temp = "";
            string[] arr = new string[ascii.Length];

            try
            {
                for (int i = 0; i < ascii.Length; i++)
                {
                    arr[i] = ((int)ascii[i]).ToString("X");
                }

                for (int i = arr.Length - 1; i >= 0; i--)
                {
                    temp += arr[i].ToString();
                }

                word = (Int32.Parse(temp, NumberStyles.HexNumber)).ToString();
            }
            catch (Exception ex)
            {
#if LOGWRITE_TO_FILE
                //Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                //Get the top stack frame
                var frame = st.GetFrame(0);
                //Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                LogAgent.ErrLogWrite(ex.Message.ToString(), string.Format("Line:{0}, Func:{1}, StackTrace:{2}", line.ToString(), frame.GetMethod().Name, st.ToString()));
#endif
                return "";
            }

            return word;
        }

        private string WordToASCII(string word, int idx)
        {
            string ascii;
            try
            {
                string ascii1 = word.Substring(4 * idx, 2);
                string ascii2 = word.Substring(4 * idx + 2, 2);

                ascii = Convert.ToChar(Convert.ToInt16(ascii1, 16)).ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
            return ascii;
        }
        #endregion
    }
}
