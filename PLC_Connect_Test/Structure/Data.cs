using PLC_Connect_Test.Model.DTO;
using PLC_Connect_Test.Pattern;
using System.Collections.Generic;

namespace PLC_Connect_Test.Structure
{
    public class Data : Singleton<Data>
    {
        //public Dictionary<string, TbPlcInfo> PlcInfos { get; set; } = new Dictionary<string, TbPlcInfo>();
        public PlcInfoResDto PlcInfo { get; set; }
        public Dictionary<string, string> DeviceLiveData { get; set; } = new Dictionary<string, string>();
    }
}
