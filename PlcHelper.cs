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
        private readonly object sync = new object();
        public OperateResult<bool> ReadBool(string address)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) return new OperateResult<bool> { IsSuccess = false, Message = "PLC 未连接" };
                return plc.ReadBool(address);
            }
        }

        public OperateResult<float> ReadFloat(string address)
        {
            lock (sync)
            {
                if (!isConnect || plc == null) return new OperateResult<float> { IsSuccess = false, Message = "PLC 未连接" };
                return plc.ReadFloat(address);
            }
        }

        public OperateResult WriteFloat(string address, float value)
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

        public OperateResult WriteBool(string address, bool value)
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
