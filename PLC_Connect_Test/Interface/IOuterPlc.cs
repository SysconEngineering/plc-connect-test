namespace PLC_Connect_Test.Interface
{
    public interface IOuterPlc
    {
        public void Init();
        public void Start();
        public void Stop();
        public void Processiong();

        public void AddPlc(int idx, string name, int plc_type);

        public void Write(string plc_name, string address, string data);

        //public void SetCore(Core core);

        public int GetPlcType();
    }
}
