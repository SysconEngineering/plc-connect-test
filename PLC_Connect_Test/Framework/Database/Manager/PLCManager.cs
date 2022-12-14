using PLC_Connect_Test.Interface;
using PLC_Connect_Test.Interface.Communication;
using PLC_Connect_Test.Model.DTO;
using PLC_Connect_Test.OuterPLC;
using PLC_Connect_Test.Structure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PLC_Connect_Test.Framework.Database.Manager
{
    public class PLCManager
    {
        public Dictionary<string, IOuterPlc> PlcList = new Dictionary<string, IOuterPlc>();
        public Dictionary<string, ICommunication> mPlcList = new Dictionary<string, ICommunication>();

        public event OuterPlcInfoDataEvt outerPlcInfoData_Evt;
        public delegate void OuterPlcInfoDataEvt(Dictionary<string, OuterPlc> values);
        public Dictionary<string, string> dicDeviceDataValue = new Dictionary<string, string>();

        public void ModbusInit(string key)
        {
            this.ModbusProtocolInit(key);
        }

        public void ModbusProtocolInit(string key)
        {
            if (Data.Instance.PlcInfo != null)
            {
                var plcInfo = Data.Instance.PlcInfo;
                if (!mPlcList.ContainsKey(plcInfo.ip))
                {
                    CommunicationModbus modbus = new CommunicationModbus(plcInfo.ip, plcInfo.port);
                    if (!modbus.PlcInfo.Contains(plcInfo))
                    {
                        modbus.PlcInfo.Add(plcInfo);
                        mPlcList.Add(plcInfo.ip, modbus);
                    }
                    modbus.Start();
                }
                else { Console.WriteLine("이미 연결된 PLC입니다."); }
            }
        }

        public void MCInit(string key)
        {
            this.MCProtocolInit(key);
            this.OuterPLCStart();
        }

        public void Stop()
        {
            this.OuterPLCStop();
        }
        public void MCProtocolInit(string key)
        {
            if (Data.Instance.PlcInfo != null)
            {
                var plcInfo = Data.Instance.PlcInfo;
                if (!PlcList.ContainsKey(plcInfo.ip))
                {
                    Mitusbishi_Client_Socket plcSocket = new Mitusbishi_Client_Socket(plcInfo.ip, plcInfo.port, plcInfo.readLoc);
                    plcSocket.AddPlc(plcInfo.name, plcInfo.plcType);
                    plcSocket.PLCdataEvt += new Mitusbishi_Client_Socket.PLCDataResponse(Mitsubishi_PLCdataEvt);
                    PlcList.Add(plcInfo.ip, plcSocket);
                }
                else
                {
                    Console.WriteLine("이미 연결된 PLC입니다.");
                }
            }
        }

        public void OuterPLCStart()
        {
            if (PlcList != null)
            {
                foreach (string key in PlcList.Keys)
                {
                    PlcList[key].Start();
                    Thread.Sleep(10);
                }
            }
        }

        public void OuterPLCStop()
        {
            if (PlcList != null)
            {
                foreach (string key in PlcList.Keys)
                {
                    PlcList[key].Stop();
                    Thread.Sleep(10);
                }
            }
            if (mPlcList != null)
            {
                foreach (string key in mPlcList.Keys)
                {
                    mPlcList[key].Stop();
                    Thread.Sleep(10);
                }
            }
            PlcList.Clear();
            mPlcList.Clear();
        }

        public void Mitsubishi_PLCdataEvt(Dictionary<string, OuterPlc> values)
        {
            if (values != null)
            {
                if (outerPlcInfoData_Evt != null)
                    outerPlcInfoData_Evt?.Invoke(values);

                try
                {
                    foreach (string name in values.Keys)
                    {
                        OuterPlc plc = values[name];

                        try
                        {
                            if (plc.info.Count < 1)
                                continue;
                            List<PlcDataResDto> valueList = new List<PlcDataResDto>();
                            for (int i = 0; i < plc.info.Count; i++)
                            {
                                if (plc.info[i] != null)
                                {
                                    PlcDataResDto val = new PlcDataResDto();
                                    val.register = Int32.Parse(plc.info[i].addr);
                                    val.value = Int32.Parse(plc.info[i].value);

                                    valueList.Add(val);

                                    if (dicDeviceDataValue.ContainsKey(val.register.ToString()))
                                    {
                                        dicDeviceDataValue[val.register.ToString()] = val.value.ToString();
                                    }
                                    else
                                    {
                                        //  없으면 신규 추가
                                        dicDeviceDataValue.Add(val.register.ToString(), val.value.ToString());
                                    }
                                }
                            }
                            Data.Instance.DeviceLiveData = dicDeviceDataValue;
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
