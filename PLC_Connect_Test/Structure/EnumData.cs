namespace PLC_Connect_Test.Structure
{
    public class EnumData
    {
        public enum PLC_Type
        {
            // 0:Mitsubishi, 1:LS, 2:Siemens, 3:OPC-UA
            Mitusbishi,
            LS,
            Simens,
            OPC_UA
        }

        public enum Protocol_Type
        {
            MC = 1,
            Modbus
        }
    }
}
