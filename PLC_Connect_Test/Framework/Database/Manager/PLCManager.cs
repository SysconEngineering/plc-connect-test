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

        public event OuterPlcInfoDataEvt outerPlcInfoData_Evt;
        public delegate void OuterPlcInfoDataEvt(Dictionary<string, OuterPlc> values);

        public void ModbusInit(string key)
        {
            this.ModbusProtocolInit(key);
        }

        public void ModbusProtocolInit(string key)
        {
            if (Data.Instance.PlcInfos != null)
            {
                var plcInfo = Data.Instance.PlcInfos[key];
                if (!PlcList.ContainsKey(plcInfo.Ip))
                {
                    CommunicationModbus modbus = new CommunicationModbus(plcInfo.Ip, plcInfo.Port);
                    if (!modbus.PlcInfo.Contains(plcInfo))
                    {
                        modbus.PlcInfo.Add(plcInfo);
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
            if (Data.Instance.PlcInfos != null)
            {
                var plcInfo = Data.Instance.PlcInfos[key];
                if (!PlcList.ContainsKey(plcInfo.Ip))
                {
                    Mitusbishi_Client_Socket plcSocket = new Mitusbishi_Client_Socket(plcInfo.Ip, plcInfo.Port, plcInfo.ReadLoc);
                    plcSocket.AddPlc(plcInfo.Idx, plcInfo.PointName, plcInfo.PlcType);
                    plcSocket.PLCdataEvt += new Mitusbishi_Client_Socket.PLCDataResponse(Mitsubishi_PLCdataEvt);
                    PlcList.Add(plcInfo.Ip, plcSocket);
                }
                else
                {
                    Console.WriteLine("이미 연결된 PLC입니다.");
                }
            }
        }

        public void OuterPLCStart()
        {
            foreach (string key in PlcList.Keys)
            {
                PlcList[key].Start();
                Thread.Sleep(10);
            }

        }

        public void OuterPLCStop()
        {
            foreach (string key in PlcList.Keys)
            {
                PlcList[key].Stop();
                Thread.Sleep(10);
            }
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
                                    val.address = plc.info[i].addr;
                                    val.value = plc.info[i].value;

                                    valueList.Add(val);
                                }
                            }
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
