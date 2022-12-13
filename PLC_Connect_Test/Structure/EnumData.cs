namespace PLC_Connect_Test.Structure
{
    public class EnumData
    {
        public enum PLC_Type
        {
            // 0:Mitsubishi, 1:LS, 2:Siemens
            Mitusbishi,
            LS,
            Simens
        }

        public enum Protocol_Type
        {
            // 1: MC, 2: Modbus, 3: OPC_UA
            MC = 1,
            Modbus,
            OPC_UA
        }
    }
}
