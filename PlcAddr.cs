using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcAxisControl
{
    internal class PlcAddr
    {
        #region 轴1的地址
        //常量的关键字 const  Π  重力加速度 G = 9.8;


        public const string Axis1Home = "M0.0";    // 回零指令
                                                   //  public const string Axis1HomeDone = "M10.0";   // 回零完成
        public const string Axis1Trigger = "M1.0";    // 绝对定位启动（按 1 松 0）
        public const string Axis1JogPos = "M3.0";    // 手动正（右移）
        public const string Axis1JogNeg = "M3.1";    // 手动负（左移）
        public const string Axis1Pos = "VD100";   // 当前位置
        public const string Axis1Speed = "VD104";   // 当前速度
        public const string Axis1ManualSpeed = "VD2108";  // 手动速度
        public const string Axis1Target = "VD3000";  // 设定位置
        public const string Axis1SetSpeed = "VD3004";  // 设定速度
        public const string Axis1Done = "V0.2";    // 定位完成
        //左限位
        public const string Axis1DJogNeg = "I0.2";
        //原点
        public const string Axis1Origin = "I0.3";
        //右限位
        public const string Axis1DJogPos = "I0.4";
        //
        //  PLC_ADDR=20;
        #endregion
        #region 步进轴 2
        public const string Axis2Home = "M0.1";    // 回零指令
        public const string Axis2HomeDone = "M10.1";   // 回零完成
        public const string Axis2Trigger = "M1.1";    // 绝对定位启动
        public const string Axis2JogPos = "M3.4";    // 手动正
        public const string Axis2JogNeg = "M3.5";    // 手动负
        public const string Axis2Pos = "VD2024";  // 当前位置
        public const string Axis2Speed = "VD2028";  // 当前速度
        public const string Axis2ManualSpeed = "VD2112";  // 手动速度
        public const string Axis2Target = "VD3008";  // 设定位置
        public const string Axis2SetSpeed = "VD3012";  // 设定速度
        public const string Axis2Done = "V0.6";    // 定位完成

        public const string Axis1DJogNeg2 = "I0.5";
        //原点
        public const string Axis1Origin2 = "I0.6";
        //右限位
        public const string Axis1DJogPos2 = "I0.7";
        #endregion
    }
}
