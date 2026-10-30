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
        public Form1()
        {
            InitializeComponent();
            tmr_Refresh.Tick += RefreshTick;
            btnJogNegative.MouseDown += JogMouseDown;
            btnJogPositive.MouseDown += JogMouseDown;
            btnJogNegative.MouseUp += JogMouseUp;
            btnJogPositive.MouseUp += JogMouseUp;
            btnJogNegative.MouseCaptureChanged += JogCaptureChanged;
            btnJogPositive.MouseCaptureChanged += JogCaptureChanged;
            Deactivate += JogDeactivate;
            FormClosing += JogFormClosing;
            btnHome.Enabled = false;
            lblPosition.Text = "当前位置: --";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("324436");
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
            }
        }

        private bool refreshing;
        private TaskCompletionSource<bool> jogRelease;
        private Button jogButton;
        private bool jogFault;

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
            if (e.Button != MouseButtons.Left || jogRelease != null || jogFault || !plcHelper.isConnect || !tmr_Refresh.Enabled) return;
            bool axis1 = rdoAxis1.Checked;
            bool negative = sender == btnJogNegative;
            string address = axis1 ? (negative ? PlcAddr.Axis1JogNeg : PlcAddr.Axis1JogPos)
                                   : (negative ? PlcAddr.Axis2JogNeg : PlcAddr.Axis2JogPos);
            string speedAddress = axis1 ? PlcAddr.Axis1ManualSpeed : PlcAddr.Axis2ManualSpeed;
            float speed = (float)numSpeed.Value;
            jogButton = (Button)sender;
            jogRelease = new TaskCompletionSource<bool>();
            var release = jogRelease;
            tmr_Refresh.Stop();
            rdoAxis1.Enabled = rdoAxis2.Enabled = btnHome.Enabled = false;
            bool attemptedStart = false;
            try
            {
                var speedResult = await Task.Run(() => plcHelper.WriteFloat(speedAddress, speed));
                if (!speedResult.IsSuccess) throw new InvalidOperationException("速度写入失败：" + speedResult.Message);
                if (release.Task.IsCompleted) return;
                attemptedStart = true;
                var start = await Task.Run(() => plcHelper.WriteBool(address, true));
                if (!start.IsSuccess) throw new InvalidOperationException("点动命令写入失败：" + start.Message);
                await release.Task;
            }
            catch (Exception ex)
            {
                jogFault = true;
                if (!IsDisposed) MessageBox.Show(ex.Message, "点动异常");
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
                        if (!IsDisposed) MessageBox.Show("点动复位失败，请立即使用设备停止装置并检查 PLC 命令位：" + ex.Message, "点动异常");
                    }
                }
                jogRelease = null;
                jogButton = null;
                if (!IsDisposed)
                {
                    rdoAxis1.Enabled = rdoAxis2.Enabled = true;
                    if (!jogFault) { btnHome.Enabled = true; tmr_Refresh.Start(); }
                    else { btnHome.Enabled = false; lblRunState.Text = "状态: 点动异常，请检查 PLC"; }
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
                float position = await Task.Run(() => plcHelper.ReadPosition(axis1));
                if (!IsDisposed && axis1 == rdoAxis1.Checked)
                {
                    lblPosition.Text = "当前位置: " + position.ToString("0.00") + " mm";
                    lblRunState.Text = "状态: 通信正常";
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    tmr_Refresh.Stop();
                    btnHome.Enabled = false;
                    lblPosition.Text = "当前位置: --";
                    lblRunState.Text = "通信失败: " + ex.Message;
                }
            }
            finally { refreshing = false; }
        }

        private async void btnHome_Click(object sender, EventArgs e)
        {
            bool axis1 = rdoAxis1.Checked;
            btnHome.Enabled = false;
            try { await Task.Run(() => plcHelper.Home(axis1)); }
            catch (Exception ex) { if (!IsDisposed) MessageBox.Show(ex.Message, "回零失败"); }
            finally { if (!IsDisposed && tmr_Refresh.Enabled) btnHome.Enabled = true; }
        }

    }
}
