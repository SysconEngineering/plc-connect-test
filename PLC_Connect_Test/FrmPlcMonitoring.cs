
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PLC_Connect_Test.Framework.Database.Manager;
using PLC_Connect_Test.Model.DTO;
using PLC_Connect_Test.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static PLC_Connect_Test.Structure.EnumData;

namespace PLC_Connect_Test
{
    public partial class FrmPlcMonitoring : Form
    {
        public DBManager _dbManager = new DBManager();
        PLCManager _plcManager = new PLCManager();
        PlcInfoResDto _plcInfo = new PlcInfoResDto();
        System.Timers.Timer timer = new System.Timers.Timer();
        delegate void TimerEventDelegate(Dictionary<string, string> dic, DataGridView dgv);

        private int _plcType = 0;
        private int _protocolType = -1;
        private int _loopCnt = 0;

        public FrmPlcMonitoring()
        {
            InitializeComponent();
            this.Shown += FrmPlcMonitoring_Shown;
        }

        #region ## Initialize
        private void FrmPlcMonitoring_Shown(object sender, System.EventArgs e)
        {
            //Initialize();
        }

        private void Initialize()
        {
            ReadJsonFIle();
            SetMonitoringInitControl();
            InitPLC();
            SetPLCInfo();
            timer.Interval = 1000;
            // 주기마다 이벤트 실행
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SetMonitoringData(Data.Instance.DeviceLiveData, dgvMonitoring);
        }

        private void InitPLC()
        {
            Data.Instance.PlcInfo = null;
            Data.Instance.PlcInfo = _plcInfo;
            //btnConnect.Enabled = false;
        }

        private void SetPLCInfo()
        {
            tbName.Text = _plcInfo.name ?? "";
            tbIp.Text = _plcInfo.ip;
            tbPort.Text = _plcInfo.port.ToString() ?? "";
            _plcType = _plcInfo.plcType;             // 0:Mitsubishi, 1:LS, 2:Siemens
            if (_plcInfo.plcType == (int)PLC_Type.Mitusbishi)
            {
                label4.Visible = true;
                tbPlcType.Visible = true;
                tbPlcType.Text = PLC_Type.Mitusbishi.ToString();
                _plcType = (int)PLC_Type.Mitusbishi;
            }
            if (_protocolType != -1)
            {
                if (_protocolType == (int)Protocol_Type.MC)
                {
                    labelReadLoc.Visible = true;
                    tbReadLoc.Visible = true;
                    tbReadLoc.Text = _plcInfo.readLoc;
                }
                else
                {
                    // Modbus는 readLoc 필요없음
                    labelReadLoc.Visible = false;
                    tbReadLoc.Visible = false;
                }
            }

        }
        private void SetMonitoringInitControl()
        {
            dgvMonitoring.Rows.Clear();
            dgvMonitoring.Columns.Add("colRegister", "register");
            dgvMonitoring.Columns.Add("colValue", "value");
        }

        private void SetMonitoringData(Dictionary<string, string> data, DataGridView dgv)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    TimerEventDelegate timerEvent = new TimerEventDelegate(SetMonitoringData);
                    this.Invoke(timerEvent, data, dgv);
                }
                else
                {
                    if (_loopCnt > 0)
                    {
                        string curKey = dgv.Rows[0].Cells[0].FormattedValue.ToString();
                        foreach (var d in data)
                        {
                            bool updateYn = false;
                            for (int i = 0; i < dgv.Rows.Count; i++)
                            {
                                if (dgv.Rows[i].Cells[0].FormattedValue.ToString().Equals(d.Key))
                                {
                                    dgv.Rows[i].Cells[1].Value = d.Value;
                                    updateYn = true;
                                }
                            }
                            if (updateYn == false)
                            {
                                dgv.Rows.Add(d.Key, d.Value);
                            }
                        }
                    }
                    else
                    {
                        foreach (var d in data)
                        {
                            dgv.Rows.Add(d.Key, d.Value);
                        }
                        _loopCnt++;
                    }
                }
            }
            catch (Exception ex)
            {
                //Get stack trace for the exception witdh source file information
                var st = new StackTrace(ex, true);
                //Get the top stack frame
                var frame = st.GetFrame(0);
                //Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                //Inner exception message
                var innerException = ex.InnerException != null ? ex.InnerException.ToString() : "";
            }
        }
        #endregion

        #region ## Event
        private void btnConnect_Click(object sender, System.EventArgs e)
        {
            if (tbName.Text != "")
            {
                if (btnConnect.Text.Equals("Connect"))
                {
                    // Connect
                    if (_protocolType == (int)EnumData.Protocol_Type.MC)
                    {
                        _plcManager.MCInit(tbName.Text);
                    }
                    else if (_protocolType == (int)EnumData.Protocol_Type.Modbus)
                    {
                        _plcManager.ModbusInit(tbName.Text);
                    }
                    btnConnect.Text = "Disconnect";
                    //lbConnectYn.BackColor = Color.PaleGreen;
                    timer.Start();
                }
                else
                {
                    // Disconnect
                    // 연결 다 끊고 초기화 하구
                    Stop();

                }
            }
            else
            {
                MessageBox.Show("연결할 PLC를 선택해주세요!", "PLC 선택 필요", MessageBoxButtons.OK);
            }

        }

        private void Stop()
        {
            timer.Stop();
            _plcManager.Stop();
            _loopCnt = 0;
            Data.Instance.DeviceLiveData.Clear();
            Data.Instance.PlcInfo = null;
            dgvMonitoring.Rows.Clear();
            btnConnect.Text = "Connect";
        }

        private void ReadJsonFIle()
        {
            FileInfo fi = null;
            if (_protocolType == (int)EnumData.Protocol_Type.Modbus)
            {
                fi = new FileInfo(Application.StartupPath + @"modbus_data.json");
            }
            else if (_protocolType == (int)EnumData.Protocol_Type.MC)
            {
                fi = new FileInfo(Application.StartupPath + @"mc_data.json");
            }
            string str = string.Empty;
            string users = string.Empty;
            if (fi.Exists)
            {
                using (StreamReader file = File.OpenText(fi.ToString()))
                using (JsonTextReader reader = new JsonTextReader(file))
                {

                    JObject json = (JObject)JToken.ReadFrom(reader);
                    _plcInfo = JsonConvert.DeserializeObject<PlcInfoResDto>(json.ToString());
                }
            }
        }
        private void cbProtType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProtType.SelectedItem.Equals(EnumData.Protocol_Type.Modbus.ToString()))
            {
                _protocolType = (int)EnumData.Protocol_Type.Modbus;
            }
            else if (cbProtType.SelectedItem.Equals(EnumData.Protocol_Type.MC.ToString()))
            {
                _protocolType = (int)EnumData.Protocol_Type.MC;
            }
            else if (cbProtType.SelectedItem.Equals(EnumData.Protocol_Type.OPC_UA.ToString()))
            {

            }
            else
            {

            }
            //Stop();
            btnConnect.Enabled = true;
            Initialize();
        }
        #endregion


    }
}
