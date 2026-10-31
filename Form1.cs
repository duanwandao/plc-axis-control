using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlcAxisControl
{
    public partial class Form1 : Form
    {
        PlcHelper plcHelper = new PlcHelper(); 
        private Label lblActionHint;

        private void StyleButton(object sender, EventArgs e)
        {
            var button = (Button)sender;
            bool primary = button == button1 || button == btnMove;
            Color active = button == btnStop ? Color.FromArgb(194, 51, 58)
                : primary ? Color.FromArgb(37, 99, 190) : Color.White;
            button.BackColor = button.Enabled ? active : Color.FromArgb(227, 231, 237);
            button.ForeColor = button.Enabled ? (primary || button == btnStop ? Color.White : ForeColor)
                : Color.FromArgb(135, 144, 156);
            button.FlatAppearance.BorderColor = button.Enabled ? Color.FromArgb(190, 201, 216) : Color.FromArgb(215, 221, 229);
            button.Cursor = button.Enabled ? Cursors.Hand : Cursors.Default;
            button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(30, 83, 163)
                : button == btnStop ? Color.FromArgb(165, 39, 46) : Color.FromArgb(232, 239, 249);
            button.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(24, 66, 130)
                : button == btnStop ? Color.FromArgb(140, 30, 37) : Color.FromArgb(213, 226, 244);
        }
        private void ApplyLayout()
        {
            SuspendLayout();
            Font = new Font("Microsoft YaHei UI", 10F);
            BackColor = Color.FromArgb(245, 247, 250);
            ForeColor = Color.FromArgb(35, 47, 62);
            ClientSize = new Size(900, 570);
            MinimumSize = Size;
            lblTitle.SetBounds(28, 22, 320, 38);
            lblTitle.Font = new Font(Font.FontFamily, 18F, FontStyle.Bold);
            lblAxisSelectCaption.Location = new Point(28, 82);
            rdoAxis1.Location = new Point(120, 80);
            rdoAxis2.Location = new Point(260, 80);
            button1.SetBounds(680, 25, 188, 42);
            lblPositionCaption.Location = new Point(28, 135);
            lblPosition.Location = new Point(28, 162);
            lblPosition.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            lblPosition.ForeColor = Color.FromArgb(37, 99, 190);
            lblSignalsCaption.Visible = false;
            grpSignals.SetBounds(28, 240, 840, 100);
            grpSignals.Text = "实时信号";
            grpSignals.BackColor = Color.White;
            var lights = new[] { uiLight1, uiLight2, uiLight3, uiLight4 };
            var labels = new[] { lblNegative, lblOrigin, lblPositive, lblDone };
            for (int i = 0; i < 4; i++)
            {
                lights[i].SetBounds(16 + i * 207, 44, 20, 20);
                labels[i].Location = new Point(42 + i * 207, 43);
                labels[i].Font = new Font(Font.FontFamily, 9F);
            }
            lblTargetCaption.Text = "目标位置 (mm)";
            lblSpeedCaption.Text = "运行速度 (mm/s)";
            lblTargetCaption.Location = new Point(28, 365);
            numTarget.SetBounds(28, 395, 190, 34);
            lblSpeedCaption.Location = new Point(255, 365);
            numSpeed.SetBounds(255, 395, 190, 34);
            lblRunStateCaption.Visible = false;
            lblRunState.Location = new Point(28, 529);
            lblRunState.Font = new Font(Font.FontFamily, 9F);
            lblActionHint = new Label {
                AutoSize = false, Location = new Point(28, 437), Size = new Size(840, 24),
                Font = new Font(Font.FontFamily, 9F), ForeColor = Color.FromArgb(95, 108, 125)
            };
            Controls.Add(lblActionHint);
            var buttons = new[] { btnMove, btnJogNegative, btnJogPositive, btnHome, btnStop };
            for (int i = 0; i < buttons.Length; i++) buttons[i].SetBounds(28 + i * 172, 465, 152, 44);
            btnJogNegative.Text = "◀  左移";
            btnJogPositive.Text = "右移  ▶";
            foreach (Button b in new[] { button1, btnMove, btnJogNegative, btnJogPositive, btnHome, btnStop })
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderColor = Color.FromArgb(213, 220, 230);
                b.BackColor = Color.White;
                b.ForeColor = ForeColor;
                b.UseVisualStyleBackColor = false;
                b.Cursor = Cursors.Hand;
                b.EnabledChanged += StyleButton;
                StyleButton(b, EventArgs.Empty);
            }
            button1.BackColor = btnMove.BackColor = Color.FromArgb(37, 99, 190);
            button1.ForeColor = btnMove.ForeColor = Color.White;
            btnStop.BackColor = Color.FromArgb(194, 51, 58);
            btnStop.ForeColor = Color.White;
            ResumeLayout(true);
        }
        public Form1()
        {
            InitializeComponent();
            ApplyLayout();
            tmr_Refresh.Tick += RefreshTick;
            btnStop.Click += StopClick;
            rdoAxis1.CheckedChanged += AxisChanged;
            btnJogNegative.MouseDown += JogMouseDown;
            btnJogPositive.MouseDown += JogMouseDown;
            btnJogNegative.MouseUp += JogMouseUp;
            btnJogPositive.MouseUp += JogMouseUp;
            btnJogNegative.MouseCaptureChanged += JogCaptureChanged;
            btnJogPositive.MouseCaptureChanged += JogCaptureChanged;
            Deactivate += JogDeactivate;
            FormClosing += JogFormClosing;
            btnHome.Enabled = false;
            lblPosition.Text = "-- mm";
            SetSignals(null);
            UpdateControls();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            plcHelper.ConnectPLC();
            if (plcHelper.isConnect)
            {
                button1.Enabled = false;
                btnHome.Enabled = true;
                tmr_Refresh.Start();
                button1.Text = "PLC 已连接";
                RefreshTick(this, EventArgs.Empty);
            }
        }

        private bool refreshing;
        private TaskCompletionSource<bool> jogRelease;
        private Button jogButton;
        private bool jogFault;
        private bool stopping;
        private TaskCompletionSource<bool> jogFinished;
        private Task homeTask;
        private Task moveTask;
        private PlcHelper.AxisStatus lastStatus;
        private bool? jogNegative;

        private void UpdateControls()
        {
            bool ready = plcHelper.isConnect && lastStatus != null && !jogFault && !stopping;
            bool idle = jogRelease == null && homeTask == null && moveTask == null;
            btnMove.Enabled = btnHome.Enabled = ready && idle;
            btnJogNegative.Enabled = ready && idle && !lastStatus.Negative;
            btnJogPositive.Enabled = ready && idle && !lastStatus.Positive;
            if (jogButton != null) jogButton.Enabled = ready;
            rdoAxis1.Enabled = rdoAxis2.Enabled = idle && !stopping;
            numTarget.Enabled = numSpeed.Enabled = idle && !stopping;
            btnStop.Enabled = plcHelper.isConnect && !stopping;
            lblActionHint.ForeColor = Color.FromArgb(95, 108, 125);
            if (!plcHelper.isConnect) lblActionHint.Text = "请先连接 PLC，灰色按钮暂不可操作。";
            else if (stopping) lblActionHint.Text = "正在清除运动命令，请稍候…";
            else if (jogFault) lblActionHint.Text = "运动操作已锁定，请检查故障，并使用停止按钮复位命令。";
            else if (lastStatus == null) lblActionHint.Text = "尚未取得有效状态，运动按钮暂不可用。";
            else if (!idle) lblActionHint.Text = jogRelease != null ? "正在点动，松开按钮即停止。" : "正在发送运动命令，请稍候；可按停止。";
            else if (lastStatus.Negative && lastStatus.Positive)
            {
                lblActionHint.Text = "左右移动均已禁用：双侧限位同时触发，请检查限位信号及触发电平。";
                lblActionHint.ForeColor = Color.FromArgb(175, 49, 56);
            }
            else if (lastStatus.Negative) lblActionHint.Text = "左移已禁用：负限位触发；可按住右移退出限位。";
            else if (lastStatus.Positive) lblActionHint.Text = "右移已禁用：正限位触发；可按住左移退出限位。";
            else lblActionHint.Text = "按住左移 / 右移进行点动，松开即停止；回零需单独操作。";
        }

        private void SetSignals(PlcHelper.AxisStatus state)
        {
            var lights = new[] { uiLight1, uiLight2, uiLight3, uiLight4 };
            var labels = new[] { lblNegative, lblOrigin, lblPositive, lblDone };
            var names = new[] { "负限位", "原点", "正限位", "定位完成" };
            bool[] values = state == null ? new bool[4] : new[] { state.Negative, state.Origin, state.Positive, state.Done };
            for (int i = 0; i < 4; i++)
            {
                lights[i].OnColor = state == null || !values[i] ? Color.LightGray : (i == 0 || i == 2 ? Color.IndianRed : Color.SeaGreen);
                labels[i].Text = names[i] + "：" + (state == null ? "未知" : values[i] ? "已触发" : "未触发");
            }
        }

        private void AxisChanged(object sender, EventArgs e)
        {
            lblTitle.Text = rdoAxis1.Checked ? "步进轴 1 控制" : "步进轴 2 控制";
            lblPosition.Text = "-- mm";
            lastStatus = null;
            SetSignals(null);
            UpdateControls();
            if (plcHelper.isConnect) RefreshTick(this, EventArgs.Empty);
        }

        private async void StopClick(object sender, EventArgs e)
        {
            if (stopping || !plcHelper.isConnect) return;
            stopping = true;
            UpdateControls();
            ReleaseJog();
            try
            {
                // 等待已经排队的点动写入和松开复位结束，防止停止后再写入 true。
                if (jogFinished != null) await jogFinished.Task;
                if (homeTask != null)
                {
                    try { await homeTask; }
                    catch { /* 回零失败也必须继续清除所有命令。 */ }
                }
                if (moveTask != null) { try { await moveTask; } catch { } }
                await Task.Run(() => plcHelper.StopAll());
                jogFault = false;
                tmr_Refresh.Start();
                lblRunState.Text = "状态: 所有运动命令已复位";
            }
            catch (Exception ex)
            {
                jogFault = true;
                lblRunState.Text = "停止失败: " + ex.Message;
                MessageBox.Show(ex.Message, "停止失败");
            }
            finally
            {
                stopping = false;
                btnStop.Enabled = true;
                btnHome.Enabled = !jogFault && tmr_Refresh.Enabled;
                UpdateControls();
            }
        }

        private void ReleaseJog()
        {
            if (jogRelease != null) jogRelease.TrySetResult(true);
        }

        private void JogMouseUp(object sender, MouseEventArgs e) { if (sender == jogButton) ReleaseJog(); }
        private void JogCaptureChanged(object sender, EventArgs e) { if (sender == jogButton && !jogButton.Capture) ReleaseJog(); }
        private void JogDeactivate(object sender, EventArgs e) { ReleaseJog(); }
        private void JogFormClosing(object sender, FormClosingEventArgs e) { ReleaseJog(); }

        private async void JogMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || stopping || moveTask != null || homeTask != null || jogRelease != null || jogFault || lastStatus == null || !plcHelper.isConnect || !tmr_Refresh.Enabled) return;
            bool axis1 = rdoAxis1.Checked;
            bool negative = sender == btnJogNegative;
            jogNegative = negative;
            string address = axis1 ? (negative ? PlcAddr.Axis1JogNeg : PlcAddr.Axis1JogPos)
                                   : (negative ? PlcAddr.Axis2JogNeg : PlcAddr.Axis2JogPos);
            float speed = (float)numSpeed.Value;
            jogButton = (Button)sender;
            jogRelease = new TaskCompletionSource<bool>();
            jogFinished = new TaskCompletionSource<bool>();
            var finished = jogFinished;
            var release = jogRelease;
            rdoAxis1.Enabled = rdoAxis2.Enabled = btnHome.Enabled = false;
            bool attemptedStart = false;
            try
            {
                if (release.Task.IsCompleted) return;
                attemptedStart = true;
                await Task.Run(() => plcHelper.StartJog(axis1, negative, speed));
                await release.Task;
            }
            catch (Exception ex)
            {
                jogFault = true;
                if (!IsDisposed) lblRunState.Text = "点动失败: " + ex.Message;
            }
            finally
            {
                if (attemptedStart)
                {
                    try
                    {
                        var stop = await Task.Run(() => plcHelper.WriteBool(address, false));
                        if (!stop.IsSuccess) throw new InvalidOperationException(stop.Message);
                    }
                    catch (Exception ex)
                    {
                        jogFault = true;
                        if (!IsDisposed) lblRunState.Text = "点动复位失败，请使用设备停止装置: " + ex.Message;
                    }
                }
                jogRelease = null;
                jogButton = null;
                jogNegative = null;
                jogFinished = null;
                finished.TrySetResult(true);
                if (!IsDisposed)
                {
                    rdoAxis1.Enabled = rdoAxis2.Enabled = true;
                    if (!jogFault) { btnHome.Enabled = !stopping && tmr_Refresh.Enabled; }
                    else { btnHome.Enabled = false; lblRunState.Text = "状态: 点动异常，请检查 PLC"; }
                    UpdateControls();
                }
            }
        }
        private async void RefreshTick(object sender, EventArgs e)
        {
            if (refreshing || !plcHelper.isConnect) return;
            refreshing = true;
            bool axis1 = rdoAxis1.Checked;
            try
            {
                var state = await Task.Run(() => plcHelper.ReadStatus(axis1));
                if (!IsDisposed && axis1 == rdoAxis1.Checked)
                {
                    lastStatus = state;
                    lblPosition.Text = state.Position.ToString("0.00") + " mm";
                    SetSignals(state);
                    if (jogNegative.HasValue && (jogNegative.Value ? state.Negative : state.Positive)) ReleaseJog();
                    if (!jogFault && !stopping) lblRunState.Text = state.Negative && state.Positive ? "双侧限位同时触发 · 请检查信号" : state.Negative || state.Positive ? "限位已触发 · 可反向退出" : "通信正常";
                    UpdateControls();
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    tmr_Refresh.Stop();
                    btnHome.Enabled = false;
                    lblPosition.Text = "-- mm";
                    lblRunState.Text = "通信失败: " + ex.Message;
                    lastStatus = null;
                    SetSignals(null);
                    ReleaseJog();
                    UpdateControls();
                }
            }
            finally { refreshing = false; }
        }

        private async void btnHome_Click(object sender, EventArgs e)
        {
            if (stopping || moveTask != null || homeTask != null || jogRelease != null || jogFault || lastStatus == null || !plcHelper.isConnect) return;
            bool axis1 = rdoAxis1.Checked;
            btnHome.Enabled = false;
            try
            {
                homeTask = Task.Run(() => plcHelper.Home(axis1));
                UpdateControls();
                await homeTask;
            }
            catch (Exception ex) { if (!IsDisposed) MessageBox.Show(ex.Message, "回零失败"); }
            finally
            {
                homeTask = null;
                if (!IsDisposed && !stopping && !jogFault && tmr_Refresh.Enabled) btnHome.Enabled = true;
                if (!IsDisposed) UpdateControls();
            }
        }

        private async void btnMove_Click(object sender, EventArgs e)
        {
            if (!btnMove.Enabled || stopping || moveTask != null) return;
            bool axis1 = rdoAxis1.Checked;
            float target = (float)numTarget.Value, speed = (float)numSpeed.Value;
            try
            {
                moveTask = Task.Run(() => plcHelper.Move(axis1, target, speed));
                UpdateControls();
                await moveTask;
                lblRunState.Text = "定位命令已发送";
            }
            catch (Exception ex)
            {
                jogFault = true;
                lblRunState.Text = "定位失败: " + ex.Message;
            }
            finally { moveTask = null; if (!IsDisposed) UpdateControls(); }
        }

        private void btnJogNegative_Click(object sender, EventArgs e)
        {

        }
    }
}
