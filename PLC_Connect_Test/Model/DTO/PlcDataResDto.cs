namespace PLC_Connect_Test.Model.DTO
{
    public class PlcDataResDto
    {
        public int register { get; set; }
        public string desc { get; set; }
        public int dataType { get; set; }
        public int value { get; set; }
        public string readWrite { get; set; }
        public string plcName { get; set; }
    }
}
