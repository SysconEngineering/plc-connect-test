namespace PLC_Connect_Test
{
    partial class FrmPlcMonitoring
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvMonitoring = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rbModbus = new System.Windows.Forms.RadioButton();
            this.rbMC = new System.Windows.Forms.RadioButton();
            this.lbConnectYn = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbReadLoc = new System.Windows.Forms.TextBox();
            this.labelReadLoc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbPlcType = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbIp = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbPlcList = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbViewCount = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonitoring)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(29, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(534, 375);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Monitoring";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvMonitoring);
            this.panel1.Location = new System.Drawing.Point(7, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(521, 346);
            this.panel1.TabIndex = 0;
            // 
            // dgvMonitoring
            // 
            this.dgvMonitoring.AllowUserToAddRows = false;
            this.dgvMonitoring.AllowUserToDeleteRows = false;
            this.dgvMonitoring.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonitoring.Location = new System.Drawing.Point(-1, 0);
            this.dgvMonitoring.Name = "dgvMonitoring";
            this.dgvMonitoring.ReadOnly = true;
            this.dgvMonitoring.RowTemplate.Height = 25;
            this.dgvMonitoring.Size = new System.Drawing.Size(522, 346);
            this.dgvMonitoring.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.rbModbus);
            this.groupBox2.Controls.Add(this.rbMC);
            this.groupBox2.Controls.Add(this.lbConnectYn);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbPlcList);
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.groupBox2.Location = new System.Drawing.Point(579, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 440);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Protocol";
            // 
            // rbModbus
            // 
            this.rbModbus.AutoSize = true;
            this.rbModbus.Location = new System.Drawing.Point(134, 29);
            this.rbModbus.Name = "rbModbus";
            this.rbModbus.Size = new System.Drawing.Size(73, 19);
            this.rbModbus.TabIndex = 6;
            this.rbModbus.TabStop = true;
            this.rbModbus.Text = "Modbus";
            this.rbModbus.UseVisualStyleBackColor = true;
            this.rbModbus.CheckedChanged += new System.EventHandler(this.rbModbus_CheckedChanged);
            // 
            // rbMC
            // 
            this.rbMC.AutoSize = true;
            this.rbMC.Location = new System.Drawing.Point(78, 29);
            this.rbMC.Name = "rbMC";
            this.rbMC.Size = new System.Drawing.Size(45, 19);
            this.rbMC.TabIndex = 5;
            this.rbMC.TabStop = true;
            this.rbMC.Text = "MC";
            this.rbMC.UseVisualStyleBackColor = true;
            this.rbMC.CheckedChanged += new System.EventHandler(this.rbMC_CheckedChanged);
            // 
            // lbConnectYn
            // 
            this.lbConnectYn.AutoSize = true;
            this.lbConnectYn.BackColor = System.Drawing.Color.Red;
            this.lbConnectYn.ForeColor = System.Drawing.Color.Red;
            this.lbConnectYn.Location = new System.Drawing.Point(264, 66);
            this.lbConnectYn.Name = "lbConnectYn";
            this.lbConnectYn.Size = new System.Drawing.Size(15, 15);
            this.lbConnectYn.TabIndex = 4;
            this.lbConnectYn.Text = "  ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbReadLoc);
            this.groupBox3.Controls.Add(this.labelReadLoc);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.tbPlcType);
            this.groupBox3.Controls.Add(this.tbPort);
            this.groupBox3.Controls.Add(this.tbIp);
            this.groupBox3.Controls.Add(this.tbName);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(11, 112);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(283, 311);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PLC Info";
            // 
            // tbReadLoc
            // 
            this.tbReadLoc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbReadLoc.Location = new System.Drawing.Point(73, 149);
            this.tbReadLoc.Name = "tbReadLoc";
            this.tbReadLoc.ReadOnly = true;
            this.tbReadLoc.Size = new System.Drawing.Size(91, 23);
            this.tbReadLoc.TabIndex = 13;
            this.tbReadLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelReadLoc
            // 
            this.labelReadLoc.AutoSize = true;
            this.labelReadLoc.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelReadLoc.Location = new System.Drawing.Point(17, 158);
            this.labelReadLoc.Name = "labelReadLoc";
            this.labelReadLoc.Size = new System.Drawing.Size(52, 15);
            this.labelReadLoc.TabIndex = 12;
            this.labelReadLoc.Text = "ReadLoc";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(14, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "PLCType";
            // 
            // tbPlcType
            // 
            this.tbPlcType.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbPlcType.Location = new System.Drawing.Point(71, 119);
            this.tbPlcType.Name = "tbPlcType";
            this.tbPlcType.ReadOnly = true;
            this.tbPlcType.Size = new System.Drawing.Size(91, 23);
            this.tbPlcType.TabIndex = 7;
            this.tbPlcType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbPort
            // 
            this.tbPort.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbPort.Location = new System.Drawing.Point(167, 84);
            this.tbPort.Name = "tbPort";
            this.tbPort.ReadOnly = true;
            this.tbPort.Size = new System.Drawing.Size(70, 23);
            this.tbPort.TabIndex = 5;
            this.tbPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbIp
            // 
            this.tbIp.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbIp.Location = new System.Drawing.Point(71, 84);
            this.tbIp.Name = "tbIp";
            this.tbIp.ReadOnly = true;
            this.tbIp.Size = new System.Drawing.Size(91, 23);
            this.tbIp.TabIndex = 4;
            this.tbIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbName.Location = new System.Drawing.Point(70, 44);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(91, 23);
            this.tbName.TabIndex = 3;
            this.tbName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(20, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "IP/Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(24, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "PLC List";
            // 
            // cbPlcList
            // 
            this.cbPlcList.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbPlcList.FormattingEnabled = true;
            this.cbPlcList.Location = new System.Drawing.Point(73, 63);
            this.cbPlcList.Name = "cbPlcList";
            this.cbPlcList.Size = new System.Drawing.Size(105, 23);
            this.cbPlcList.TabIndex = 1;
            this.cbPlcList.SelectedIndexChanged += new System.EventHandler(this.cbPlcList_SelectedIndexChanged);
            this.cbPlcList.Click += new System.EventHandler(this.cbPlcList_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnConnect.Location = new System.Drawing.Point(182, 60);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(79, 28);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cbViewCount
            // 
            this.cbViewCount.FormattingEnabled = true;
            this.cbViewCount.Items.AddRange(new object[] {
            "10",
            "15",
            "20",
            "All"});
            this.cbViewCount.Location = new System.Drawing.Point(442, 58);
            this.cbViewCount.Name = "cbViewCount";
            this.cbViewCount.Size = new System.Drawing.Size(121, 23);
            this.cbViewCount.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(406, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 15);
            this.label6.TabIndex = 3;
            this.label6.Text = "View";
            // 
            // FrmPlcMonitoring
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 473);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbViewCount);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmPlcMonitoring";
            this.Text = "PLC 연동 테스트";
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonitoring)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbPlcList;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbPlcType;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbIp;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbConnectYn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbModbus;
        private System.Windows.Forms.RadioButton rbMC;
        private System.Windows.Forms.ComboBox cbViewCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvMonitoring;
        private System.Windows.Forms.TextBox tbReadLoc;
        private System.Windows.Forms.Label labelReadLoc;
    }
}

