
using PLC_Connect_Test.Framework.Database.Manager;
using PLC_Connect_Test.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using static PLC_Connect_Test.Structure.EnumData;

namespace PLC_Connect_Test
{
    public partial class FrmPlcMonitoring : Form
    {
        public DBManager _dbManager = new DBManager();
        PLCManager _plcManager = new PLCManager();
        System.Timers.Timer timer = new System.Timers.Timer();
        delegate void TimerEventDelegate(Dictionary<string, string> dic, DataGridView dgv);

        private string _plcType = null;
        private int _protocolType = 0;
        private int _loopCnt = 0;

        public FrmPlcMonitoring()
        {
            InitializeComponent();
            this.Shown += FrmPlcMonitoring_Shown;
        }

        #region ## Initialize
        private void FrmPlcMonitoring_Shown(object sender, System.EventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            InitPLCList();
            SetMonitoringInitControl();
            timer.Interval = 1000;
            // 주기마다 이벤트 실행
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_protocolType == (int)EnumData.Protocol_Type.Modbus)
            {
                SetMonitoringData(Data.Instance.DeviceLiveData, dgvMonitoring);
            }
        }

        private void InitPLCList()
        {
            Data.Instance.PlcInfos = null;
            var plcInfos = _dbManager.PlcController.GetPlcInfos();
            Data.Instance.PlcInfos = plcInfos.ToDictionary(x => x.PointName);
            btnConnect.Enabled = false;
        }

        private void SetPLCInfo()
        {
            var plcInfo = Data.Instance.PlcInfos.FirstOrDefault(x => x.Key.Equals(cbPlcList.SelectedItem));

            tbName.Text = plcInfo.Value.PointName ?? "";
            tbIp.Text = plcInfo.Value.Ip ?? "";
            tbPort.Text = plcInfo.Value.Port.ToString() ?? "";
            if (plcInfo.Value.PlcType == (int)PLC_Type.Mitusbishi)
            {
                label4.Visible = true;
                tbPlcType.Visible = true;
                tbPlcType.Text = PLC_Type.Mitusbishi.ToString();
                _plcType = PLC_Type.Mitusbishi.ToString();
            }
            if (_protocolType != 0)
            {
                if (_protocolType == (int)Protocol_Type.MC)
                {
                    labelReadLoc.Visible = true;
                    tbReadLoc.Visible = true;
                    tbReadLoc.Text = plcInfo.Value.ReadLoc;
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
        private void cbPlcList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SetPLCInfo();
        }

        private void btnConnect_Click(object sender, System.EventArgs e)
        {
            if (tbName.Text != "")
            {
                if (btnConnect.Text.Equals("Connect"))
                {
                    // Connect
                    if (rbMC.Checked)
                    {
                        _plcManager.MCInit(tbName.Text);
                    }
                    else if (rbModbus.Checked)
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
                    timer.Stop();
                    _plcManager.Stop();
                    _loopCnt = 0;
                    Data.Instance.DeviceLiveData.Clear();
                    dgvMonitoring.Rows.Clear();
                    btnConnect.Text = "Connect";
                }
            }
            else
            {
                MessageBox.Show("연결할 PLC를 선택해주세요!", "PLC 선택 필요", MessageBoxButtons.OK);
            }

        }
        private void rbMC_CheckedChanged(object sender, System.EventArgs e)
        {
            cbPlcList.Items.Clear();
            //MC 프로토콜
            _protocolType = (int)EnumData.Protocol_Type.MC;
            foreach (var plc in Data.Instance.PlcInfos)
            {
                if (plc.Value.ProjectIdx == _protocolType)
                {
                    cbPlcList.Items.Add(plc.Value.PointName);
                }
            }
            btnConnect.Enabled = true;
        }

        private void rbModbus_CheckedChanged(object sender, System.EventArgs e)
        {
            cbPlcList.Items.Clear();
            _protocolType = (int)EnumData.Protocol_Type.Modbus;
            foreach (var plc in Data.Instance.PlcInfos)
            {
                if (plc.Value.ProjectIdx == _protocolType)
                {
                    cbPlcList.Items.Add(plc.Value.PointName);
                }
            }
            btnConnect.Enabled = true;
        }
        #endregion

        private void cbPlcList_Click(object sender, System.EventArgs e)
        {
            if (rbMC.Checked == false && rbModbus.Checked == false)
            {
                MessageBox.Show("프로토콜 타입을 선택해주세요!", "프로토콜 타입 선택 필요", MessageBoxButtons.OK);
            }
        }
    }
}
