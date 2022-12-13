using System.Collections.Generic;

namespace PLC_Connect_Test.Model.DTO
{
    public class PlcInfoResDto
    {
        public string name { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public int plcType { get; set; }
        public string readLoc { get; set; }
        public string desc { get; set; }
        public List<PlcDataResDto> data { get; set; }
    }
}
