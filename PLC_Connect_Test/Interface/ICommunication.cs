using PLC_Connect_Test.Model.DTO;
using System;
using System.Collections.Generic;

namespace PLC_Connect_Test.Interface
{
    public interface ICommunication
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool IsConnected { get; }
        public DateTime LastConnectTime { get; set; }
        public List<PlcInfoResDto> PlcInfo { get; set; }
        public void Start();
        public void Stop();
        public void ConnectCheck();
        public void DataRead();
        public void DataWrite(string register, string value);
    }
}
