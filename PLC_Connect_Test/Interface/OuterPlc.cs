using PLC_Connect_Test.Model.DTO;
using System;
using System.Collections.Generic;

namespace PLC_Connect_Test.Interface
{
    public class OuterPlcInfo
    {
        public enum PlcDataType { Binary, ASCII };

        public string addr;
        public string desc;
        public PlcDataType plcDataType;
        public string value;
        public string oldValue;
        public OuterPlcInfo(string addr, string desc, PlcDataType plcDataType)
        {
            this.addr = addr;
            this.desc = desc;
            this.plcDataType = plcDataType;
            value = "";
            oldValue = "";
        }
    }
    public class OuterPlc
    {
        public const string PLC_STANDBY = "대기 중";
        public const string PLC_REQUEST_CALL = "호출 요청함";
        public const string PLC_RESPONSE_DOCKING = "도킹 응답함";
        public const string PLC_RESPONSE_COMPLETE = "완료 응답함";
        public const string ROBOT_STANDBY = "대기 중";
        public const string ROBOT_RESPONSE_CALL = "호출 응답함";
        public const string ROBOT_REQUEST_DOCKING = "도킹 요청함";
        public const string ROBOT_REQUEST_COMPLETE = "완료 요청함";

        public DateTime alarmRequestTime { get; set; } = new DateTime();
        public string name;
        private string area;
        public int start_addr;
        public bool docking = false;
        public Dictionary<int, OuterPlcInfo> info = new Dictionary<int, OuterPlcInfo>();
        List<PlcDataResDto> infoDtls = new List<PlcDataResDto>();

        public OuterPlc(string name, string area, List<PlcDataResDto> dtls)
        {
            this.name = name;
            this.area = area;
            infoDtls = dtls;

            PlcDataTypeInitialize();
        }

        private void PlcDataTypeInitialize()
        {
            for (int i = 0; i < infoDtls.Count; i++)
            {
                PlcDataResDto dtl = infoDtls[i];
                info.Add(i, new OuterPlcInfo(area + (dtl.register), dtl.desc, (OuterPlcInfo.PlcDataType)dtl.dataType));
            }
        }
    }
}
