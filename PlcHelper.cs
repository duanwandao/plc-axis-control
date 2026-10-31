using HslCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PlcAxisControl
{
    internal class PlcHelper
    {
        internal class AxisStatus
        {
            public float Position;
            public bool Negative, Origin, Positive, Done;
        }

        private bool Signal(string address)
        {
            var result = ReadBool(address);
            if (!result.IsSuccess) throw new InvalidOperationException(address + ": " + result.Message);
            return result.Content;
        }

        public AxisStatus ReadStatus(bool axis1)
        {
            lock (sync)
            {
                var position = ReadFloat(axis1 ? PlcAddr.Axis1Pos : PlcAddr.Axis2Pos);
                if (!position.IsSuccess || float.IsNaN(position.Content) || float.IsInfinity(position.Content))
                    throw new InvalidOperationException("位置读取失败：" + position.Message);
                return new AxisStatus {
                    Position = position.Content,
                    Negative = Signal(axis1 ? PlcAddr.Axis1DJogNeg : PlcAddr.Axis1DJogNeg2),
                    Origin = Signal(axis1 ? PlcAddr.Axis1Origin : PlcAddr.Axis1Origin2),
                    Positive = Signal(axis1 ? PlcAddr.Axis1DJogPos : PlcAddr.Axis1DJogPos2),
                    Done = Signal(axis1 ? PlcAddr.Axis1Done : PlcAddr.Axis2Done)
                };
            }
        }

        private void Require(OperateResult result)
        {
            if (!result.IsSuccess) throw new InvalidOperationException(result.Message);
        }

        public void Move(bool axis1, float target, float speed)
        {
            lock (sync)
            {
                if (float.IsNaN(target) || float.IsInfinity(target) || float.IsNaN(speed) || float.IsInfinity(speed) || speed <= 0)
                    throw new InvalidOperationException("目标位置或速度无效");
                var state = ReadStatus(axis1);
                if ((state.Negative && target < state.Position) || (state.Positive && target > state.Position))
                    throw new InvalidOperationException("限位已触发，禁止朝限位方向定位");
                Require(WriteFloat(axis1 ? PlcAddr.Axis1Target : PlcAddr.Axis2Target, target));
                Require(WriteFloat(axis1 ? PlcAddr.Axis1SetSpeed : PlcAddr.Axis2SetSpeed, speed));
                string trigger = axis1 ? PlcAddr.Axis1Trigger : PlcAddr.Axis2Trigger;
                Require(WriteBool(trigger, false));
                try
                {
                    Require(WriteBool(trigger, true));
                    System.Threading.Thread.Sleep(200);
                }
                finally { Require(WriteBool(trigger, false)); }
            }
        }

        public void StartJog(bool axis1, bool negative, float speed)
        {
            lock (sync)
            {
                if (float.IsNaN(speed) || float.IsInfinity(speed) || speed <= 0)
                    throw new InvalidOperationException("点动速度必须大于零");
                var state = ReadStatus(axis1);
                if (negative ? state.Negative : state.Positive)
                    throw new InvalidOperationException("该方向限位已触发，请反向退出");
                Require(WriteFloat(axis1 ? PlcAddr.Axis1ManualSpeed : PlcAddr.Axis2ManualSpeed, speed));
                Require(WriteBool(axis1 ? (negative ? PlcAddr.Axis1JogPos : PlcAddr.Axis1JogNeg)
                    : (negative ? PlcAddr.Axis2JogPos : PlcAddr.Axis2JogNeg), false));
                Require(WriteBool(axis1 ? (negative ? PlcAddr.Axis1JogNeg : PlcAddr.Axis1JogPos)
                    : (negative ? PlcAddr.Axis2JogNeg : PlcAddr.Axis2JogPos), true));
            }
        }
        private readonly object sync = new object();
        public virtual OperateResult<bool> ReadBool(string address)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) return new OperateResult<bool> { IsSuccess = false, Message = "PLC 未连接" };
                return plc.ReadBool(address);
            }
        }

        public virtual OperateResult<float> ReadFloat(string address)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) return new OperateResult<float> { IsSuccess = false, Message = "PLC 未连接" };
                return plc.ReadFloat(address);
            }
        }

        public virtual OperateResult WriteFloat(string address, float value)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) return new OperateResult("PLC 未连接");
                return plc.Write(address, value);
            }
        }
        public float ReadPosition(bool axis1)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) throw new InvalidOperationException("PLC 未连接");
                var result = ReadFloat(axis1 ? PlcAddr.Axis1Pos : PlcAddr.Axis2Pos);
                if (!result.IsSuccess) throw new InvalidOperationException(result.Message);
                return result.Content;
            }
        }

        public void Home(bool axis1)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) throw new InvalidOperationException("PLC 未连接");
                string address = axis1 ? PlcAddr.Axis1Home : PlcAddr.Axis2Home;
                var start = plc.Write(address, true);
                if (!start.IsSuccess) throw new InvalidOperationException("回零写入失败：" + start.Message + "；请检查 PLC 命令位");
                System.Threading.Thread.Sleep(200);
                var reset = plc.Write(address, false);
                if (!reset.IsSuccess) throw new InvalidOperationException("回零复位失败：" + reset.Message + "；请检查 PLC 命令位");
            }
        }
        public void StopAll()
        {
            lock (sync)
            {
                string[] addresses = {
                    PlcAddr.Axis1Home, PlcAddr.Axis1Trigger, PlcAddr.Axis1JogPos, PlcAddr.Axis1JogNeg,
                    PlcAddr.Axis2Home, PlcAddr.Axis2Trigger, PlcAddr.Axis2JogPos, PlcAddr.Axis2JogNeg
                };
                var failures = new List<string>();
                foreach (string address in addresses)
                {
                    // 单个位失败也继续复位其余命令，不改动输入信号及位置、速度寄存器。
                    try
                    {
                        var result = WriteBool(address, false);
                        if (!result.IsSuccess) failures.Add(address + ": " + result.Message);
                    }
                    catch (Exception ex) { failures.Add(address + ": " + ex.Message); }
                }
                if (failures.Count > 0) throw new InvalidOperationException(string.Join("；", failures));
            }
        }
        HslCommunication.Profinet.Siemens.SiemensS7Net plc;
        //定义一个是否连接成功的标识  定义一个变量 用来保存连接结果
        public bool isConnect = false;
        //连接PLC
        public void ConnectPLC()
        {
            plc = new HslCommunication.Profinet.Siemens.SiemensS7Net(HslCommunication.Profinet.Siemens.SiemensPLCS.S200Smart);
            plc.CommunicationPipe = new HslCommunication.Core.Pipe.PipeTcpNet("192.168.0.2", 102)
            {
                ConnectTimeOut = 5000,    // 连接超时时间，单位毫秒
                ReceiveTimeOut = 10000,    // 接收设备数据反馈的超时时间
                SleepTime = 0,
                SocketKeepAliveTime = -1,
                IsPersistentConnection = true,
            };
            //判断是否连接成功  HSL 官网 找代码
            // client为设备的连接对象，如果想知道是否连接成功
            OperateResult connect = plc.ConnectServer();
            if (connect.IsSuccess)
            {
                isConnect = true;
                MessageBox.Show("PLC 连接成功！", "连接提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                isConnect = false;
                MessageBox.Show("PLC 连接失败：" + connect.Message, "连接提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        //断开PLC
        public void DisconnectPLC()
        {
            //判断是否连接成功 
            if (isConnect)
            {
                plc.ConnectClose();
                isConnect = false;
            }
            // plc.ConnectClose();
        }

        public virtual OperateResult WriteBool(string address, bool value)
        {
            lock (sync)
            {
            if (!isConnect || plc == null)
            {
                return new OperateResult("PLC 未连接");
            }

            return plc.Write(address, value);
            }
        }
    }
}
