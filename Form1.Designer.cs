using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace PlcAxisControl
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblAxisSelectCaption;
        private RadioButton rdoAxis1;
        private RadioButton rdoAxis2;
        private Label lblPositionCaption;
        private Label lblPosition;
        private Label lblSignalsCaption;
        private GroupBox grpSignals;
        private UILight uiLight1;
        private UILight uiLight2;
        private UILight uiLight3;
        private UILight uiLight4;
        private Label lblNegative;
        private Label lblOrigin;
        private Label lblPositive;
        private Label lblDone;
        private Label lblRunStateCaption;
        private Label lblRunState;
        private Label lblTargetCaption;
        private NumericUpDown numTarget;
        private Label lblSpeedCaption;
        private NumericUpDown numSpeed;
        private Button btnMove;
        private Button btnJogNegative;
        private Button btnJogPositive;
        private Button btnHome;
        private Button btnStop;
        private Timer tmr_Refresh;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblAxisSelectCaption = new System.Windows.Forms.Label();
            this.rdoAxis1 = new System.Windows.Forms.RadioButton();
            this.rdoAxis2 = new System.Windows.Forms.RadioButton();
            this.lblPositionCaption = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.lblSignalsCaption = new System.Windows.Forms.Label();
            this.grpSignals = new System.Windows.Forms.GroupBox();
            this.uiLight1 = new Sunny.UI.UILight();
            this.lblNegative = new System.Windows.Forms.Label();
            this.uiLight2 = new Sunny.UI.UILight();
            this.lblOrigin = new System.Windows.Forms.Label();
            this.uiLight3 = new Sunny.UI.UILight();
            this.lblPositive = new System.Windows.Forms.Label();
            this.uiLight4 = new Sunny.UI.UILight();
            this.lblDone = new System.Windows.Forms.Label();
            this.lblRunStateCaption = new System.Windows.Forms.Label();
            this.lblRunState = new System.Windows.Forms.Label();
            this.lblTargetCaption = new System.Windows.Forms.Label();
            this.numTarget = new System.Windows.Forms.NumericUpDown();
            this.lblSpeedCaption = new System.Windows.Forms.Label();
            this.numSpeed = new System.Windows.Forms.NumericUpDown();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnJogNegative = new System.Windows.Forms.Button();
            this.btnJogPositive = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tmr_Refresh = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.grpSignals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(14, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(204, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "步进轴 1 控制";
            // 
            // lblAxisSelectCaption
            // 
            this.lblAxisSelectCaption.AutoSize = true;
            this.lblAxisSelectCaption.Location = new System.Drawing.Point(14, 76);
            this.lblAxisSelectCaption.Name = "lblAxisSelectCaption";
            this.lblAxisSelectCaption.Size = new System.Drawing.Size(62, 18);
            this.lblAxisSelectCaption.TabIndex = 1;
            this.lblAxisSelectCaption.Text = "选择轴";
            // 
            // rdoAxis1
            // 
            this.rdoAxis1.AutoSize = true;
            this.rdoAxis1.Checked = true;
            this.rdoAxis1.Location = new System.Drawing.Point(147, 74);
            this.rdoAxis1.Name = "rdoAxis1";
            this.rdoAxis1.Size = new System.Drawing.Size(105, 22);
            this.rdoAxis1.TabIndex = 2;
            this.rdoAxis1.TabStop = true;
            this.rdoAxis1.Text = "控制轴 1";
            // 
            // rdoAxis2
            // 
            this.rdoAxis2.AutoSize = true;
            this.rdoAxis2.Location = new System.Drawing.Point(256, 74);
            this.rdoAxis2.Name = "rdoAxis2";
            this.rdoAxis2.Size = new System.Drawing.Size(105, 22);
            this.rdoAxis2.TabIndex = 3;
            this.rdoAxis2.Text = "控制轴 2";
            // 
            // lblPositionCaption
            // 
            this.lblPositionCaption.AutoSize = true;
            this.lblPositionCaption.Location = new System.Drawing.Point(14, 128);
            this.lblPositionCaption.Name = "lblPositionCaption";
            this.lblPositionCaption.Size = new System.Drawing.Size(80, 18);
            this.lblPositionCaption.TabIndex = 4;
            this.lblPositionCaption.Text = "当前位置";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblPosition.Location = new System.Drawing.Point(147, 120);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(249, 34);
            this.lblPosition.TabIndex = 5;
            this.lblPosition.Text = "当前位置: 0.00 mm";
            // 
            // lblSignalsCaption
            // 
            this.lblSignalsCaption.AutoSize = true;
            this.lblSignalsCaption.Location = new System.Drawing.Point(14, 165);
            this.lblSignalsCaption.Name = "lblSignalsCaption";
            this.lblSignalsCaption.Size = new System.Drawing.Size(44, 18);
            this.lblSignalsCaption.TabIndex = 6;
            this.lblSignalsCaption.Text = "状态";
            // 
            // grpSignals
            // 
            this.grpSignals.Controls.Add(this.uiLight1);
            this.grpSignals.Controls.Add(this.lblNegative);
            this.grpSignals.Controls.Add(this.uiLight2);
            this.grpSignals.Controls.Add(this.lblOrigin);
            this.grpSignals.Controls.Add(this.uiLight3);
            this.grpSignals.Controls.Add(this.lblPositive);
            this.grpSignals.Controls.Add(this.uiLight4);
            this.grpSignals.Controls.Add(this.lblDone);
            this.grpSignals.Location = new System.Drawing.Point(147, 159);
            this.grpSignals.Name = "grpSignals";
            this.grpSignals.Padding = new System.Windows.Forms.Padding(14, 15, 14, 15);
            this.grpSignals.Size = new System.Drawing.Size(855, 147);
            this.grpSignals.TabIndex = 7;
            this.grpSignals.TabStop = false;
            this.grpSignals.Text = "实时信号";
            // 
            // uiLight1
            // 
            this.uiLight1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLight1.Location = new System.Drawing.Point(15, 51);
            this.uiLight1.MinimumSize = new System.Drawing.Size(2, 2);
            this.uiLight1.Name = "uiLight1";
            this.uiLight1.OnColor = System.Drawing.Color.Red;
            this.uiLight1.Radius = 32;
            this.uiLight1.Size = new System.Drawing.Size(32, 33);
            this.uiLight1.TabIndex = 0;
            // 
            // lblNegative
            // 
            this.lblNegative.AutoSize = true;
            this.lblNegative.Location = new System.Drawing.Point(54, 57);
            this.lblNegative.Name = "lblNegative";
            this.lblNegative.Size = new System.Drawing.Size(134, 18);
            this.lblNegative.TabIndex = 1;
            this.lblNegative.Text = "负限位：未触发";
            // 
            // uiLight2
            // 
            this.uiLight2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLight2.Location = new System.Drawing.Point(220, 51);
            this.uiLight2.MinimumSize = new System.Drawing.Size(2, 2);
            this.uiLight2.Name = "uiLight2";
            this.uiLight2.OnColor = System.Drawing.Color.Lime;
            this.uiLight2.Radius = 32;
            this.uiLight2.Size = new System.Drawing.Size(32, 33);
            this.uiLight2.TabIndex = 2;
            // 
            // lblOrigin
            // 
            this.lblOrigin.AutoSize = true;
            this.lblOrigin.Location = new System.Drawing.Point(258, 57);
            this.lblOrigin.Name = "lblOrigin";
            this.lblOrigin.Size = new System.Drawing.Size(116, 18);
            this.lblOrigin.TabIndex = 3;
            this.lblOrigin.Text = "原点：未触发";
            // 
            // uiLight3
            // 
            this.uiLight3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLight3.Location = new System.Drawing.Point(426, 51);
            this.uiLight3.MinimumSize = new System.Drawing.Size(2, 2);
            this.uiLight3.Name = "uiLight3";
            this.uiLight3.OnColor = System.Drawing.Color.Red;
            this.uiLight3.Radius = 32;
            this.uiLight3.Size = new System.Drawing.Size(32, 33);
            this.uiLight3.TabIndex = 4;
            // 
            // lblPositive
            // 
            this.lblPositive.AutoSize = true;
            this.lblPositive.Location = new System.Drawing.Point(464, 57);
            this.lblPositive.Name = "lblPositive";
            this.lblPositive.Size = new System.Drawing.Size(134, 18);
            this.lblPositive.TabIndex = 5;
            this.lblPositive.Text = "正限位：未触发";
            // 
            // uiLight4
            // 
            this.uiLight4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLight4.Location = new System.Drawing.Point(630, 51);
            this.uiLight4.MinimumSize = new System.Drawing.Size(2, 2);
            this.uiLight4.Name = "uiLight4";
            this.uiLight4.OnColor = System.Drawing.Color.Lime;
            this.uiLight4.Radius = 32;
            this.uiLight4.Size = new System.Drawing.Size(32, 33);
            this.uiLight4.TabIndex = 6;
            // 
            // lblDone
            // 
            this.lblDone.AutoSize = true;
            this.lblDone.Location = new System.Drawing.Point(669, 57);
            this.lblDone.Name = "lblDone";
            this.lblDone.Size = new System.Drawing.Size(152, 18);
            this.lblDone.TabIndex = 7;
            this.lblDone.Text = "定位完成：未触发";
            // 
            // lblRunStateCaption
            // 
            this.lblRunStateCaption.AutoSize = true;
            this.lblRunStateCaption.Location = new System.Drawing.Point(14, 324);
            this.lblRunStateCaption.Name = "lblRunStateCaption";
            this.lblRunStateCaption.Size = new System.Drawing.Size(80, 18);
            this.lblRunStateCaption.TabIndex = 8;
            this.lblRunStateCaption.Text = "运行状态";
            // 
            // lblRunState
            // 
            this.lblRunState.AutoSize = true;
            this.lblRunState.Location = new System.Drawing.Point(147, 324);
            this.lblRunState.Name = "lblRunState";
            this.lblRunState.Size = new System.Drawing.Size(116, 18);
            this.lblRunState.TabIndex = 9;
            this.lblRunState.Text = "状态: 未连接";
            // 
            // lblTargetCaption
            // 
            this.lblTargetCaption.AutoSize = true;
            this.lblTargetCaption.Location = new System.Drawing.Point(14, 375);
            this.lblTargetCaption.Name = "lblTargetCaption";
            this.lblTargetCaption.Size = new System.Drawing.Size(107, 18);
            this.lblTargetCaption.TabIndex = 10;
            this.lblTargetCaption.Text = "目标位置 mm";
            // 
            // numTarget
            // 
            this.numTarget.DecimalPlaces = 2;
            this.numTarget.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numTarget.Location = new System.Drawing.Point(147, 369);
            this.numTarget.Maximum = new decimal(new int[] {
            85,
            0,
            0,
            0});
            this.numTarget.Minimum = new decimal(new int[] {
            85,
            0,
            0,
            -2147483648});
            this.numTarget.Name = "numTarget";
            this.numTarget.Size = new System.Drawing.Size(180, 28);
            this.numTarget.TabIndex = 11;
            // 
            // lblSpeedCaption
            // 
            this.lblSpeedCaption.AutoSize = true;
            this.lblSpeedCaption.Location = new System.Drawing.Point(14, 424);
            this.lblSpeedCaption.Name = "lblSpeedCaption";
            this.lblSpeedCaption.Size = new System.Drawing.Size(125, 18);
            this.lblSpeedCaption.TabIndex = 12;
            this.lblSpeedCaption.Text = "运行速度 mm/s";
            // 
            // numSpeed
            // 
            this.numSpeed.DecimalPlaces = 1;
            this.numSpeed.Location = new System.Drawing.Point(147, 420);
            this.numSpeed.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSpeed.Name = "numSpeed";
            this.numSpeed.Size = new System.Drawing.Size(180, 28);
            this.numSpeed.TabIndex = 13;
            this.numSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(14, 480);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(94, 36);
            this.btnMove.TabIndex = 14;
            this.btnMove.Text = "定位启动";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnJogNegative
            // 
            this.btnJogNegative.Location = new System.Drawing.Point(117, 480);
            this.btnJogNegative.Name = "btnJogNegative";
            this.btnJogNegative.Size = new System.Drawing.Size(94, 36);
            this.btnJogNegative.TabIndex = 15;
            this.btnJogNegative.Text = "左移";
            this.btnJogNegative.UseVisualStyleBackColor = true;
            this.btnJogNegative.Click += new System.EventHandler(this.btnJogNegative_Click);
            // 
            // btnJogPositive
            // 
            this.btnJogPositive.Location = new System.Drawing.Point(220, 480);
            this.btnJogPositive.Name = "btnJogPositive";
            this.btnJogPositive.Size = new System.Drawing.Size(94, 36);
            this.btnJogPositive.TabIndex = 16;
            this.btnJogPositive.Text = "右移";
            this.btnJogPositive.UseVisualStyleBackColor = true;
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(324, 480);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(94, 36);
            this.btnHome.TabIndex = 17;
            this.btnHome.Text = "回零";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.MistyRose;
            this.btnStop.Location = new System.Drawing.Point(428, 480);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(94, 36);
            this.btnStop.TabIndex = 18;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = false;
            // 
            // tmr_Refresh
            // 
            this.tmr_Refresh.Interval = 200;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Lime;
            this.button1.Location = new System.Drawing.Point(480, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(171, 59);
            this.button1.TabIndex = 20;
            this.button1.Text = "连接PLC";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1283, 700);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblAxisSelectCaption);
            this.Controls.Add(this.rdoAxis1);
            this.Controls.Add(this.rdoAxis2);
            this.Controls.Add(this.lblPositionCaption);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.lblSignalsCaption);
            this.Controls.Add(this.grpSignals);
            this.Controls.Add(this.lblRunStateCaption);
            this.Controls.Add(this.lblRunState);
            this.Controls.Add(this.lblTargetCaption);
            this.Controls.Add(this.numTarget);
            this.Controls.Add(this.lblSpeedCaption);
            this.Controls.Add(this.numSpeed);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.btnJogNegative);
            this.Controls.Add(this.btnJogPositive);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnStop);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "步进轴控制";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.grpSignals.ResumeLayout(false);
            this.grpSignals.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private Button button1;
    }
}
