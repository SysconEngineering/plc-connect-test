namespace PLC_Connect_Test.Interface
{
    //public class DeviceDataInfo
    //{
    //    public int deviceId;
    //    public int seq;
    //    public int deviceDataTypeId;
    //    public string register;
    //    public string value;
    //    public string remark;

    //    public DeviceDataInfo(string addr, string remark, string value)
    //    {
    //        this.register = addr;
    //        this.value = value;
    //        this.remark = remark;
    //    }
    //}
    //public class ModbusOuterPlc
    //{
    //    public string register;
    //    public string value;
    //    public int start_addr;

    //    public int deviceId;
    //    public Dictionary<int, DeviceDataInfo> info = new Dictionary<int, DeviceDataInfo>();
    //    List<TbDeviceData> deviceData = new List<TbDeviceData>();

    //    public ModbusOuterPlc(int deviceId, List<TbDeviceData> data)
    //    {
    //        this.deviceId = deviceId;
    //        this.deviceData = data;

    //        DeviceDataSetting();
    //    }

    //    private void DeviceDataSetting()
    //    {
    //        for (int i = 0; i < deviceData.Count; i++)
    //        {
    //            TbDeviceData data = deviceData[i];
    //            info.Add(i, new DeviceDataInfo(data.Register, data.Remark, data.Value));
    //        }
    //    }

    //}
}
