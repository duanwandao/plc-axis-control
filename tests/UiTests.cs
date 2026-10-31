using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using PlcAxisControl;
class UiTest {
 static object Field(Form1 f,string name) { return typeof(Form1).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(f); }
 static void Check(bool b,string text) { if(!b) throw new Exception(text); }
 static void Refresh(Form1 f,string expected) {
 typeof(Form1).GetMethod("RefreshTick",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(f,new object[]{null,EventArgs.Empty});
 var clock=Stopwatch.StartNew();
 while(clock.ElapsedMilliseconds<3000) { Application.DoEvents(); if(!(bool)Field(f,"refreshing") && ((Label)Field(f,"lblRunState")).Text==expected) return; System.Threading.Thread.Sleep(10); }
 throw new Exception("Refresh did not finish: "+expected);
 }
 [STAThread] static void Main() { try { Run(); } catch(Exception ex) { Console.WriteLine(ex.GetType().FullName); Console.WriteLine(ex.Message); Console.WriteLine(ex.StackTrace); Environment.Exit(1); } } static void Run() {
 Application.EnableVisualStyles();
 { var f=new Form1();
 f.ShowInTaskbar=false; f.StartPosition=FormStartPosition.Manual; f.Location=new Point(-2000,-2000); f.Show();
 Check(!((Button)Field(f,"btnMove")).Enabled,"Disconnected move disabled");
 Check(((Label)Field(f,"lblNegative")).Text.Contains("未知"),"Disconnected signals unknown");
 var fake=new Fake(); fake.isConnect=true;
 typeof(Form1).GetField("plcHelper",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(f,fake);
 Refresh(f,"通信正常");
 Check(((Label)Field(f,"lblPosition")).Text=="10.00 mm","Live position");
 Check(((Button)Field(f,"btnMove")).Enabled,"Connected move enabled");
 fake.Positive=true; Refresh(f,"限位已触发 · 可反向退出");
 Check(!((Button)Field(f,"btnJogPositive")).Enabled && ((Button)Field(f,"btnJogNegative")).Enabled,"Directional limit gating");
 Check(((Button)Field(f,"btnJogPositive")).BackColor != ((Button)Field(f,"btnJogNegative")).BackColor,"Disabled direction visually distinct");
 Check(((Label)Field(f,"lblActionHint")).Text.Contains("右移已禁用"),"Single-limit explanation");
 fake.Negative=true; Refresh(f,"双侧限位同时触发 · 请检查信号");
 Check(!((Button)Field(f,"btnJogPositive")).Enabled && !((Button)Field(f,"btnJogNegative")).Enabled,"Both limits disable both directions");
 Check(((Label)Field(f,"lblActionHint")).Text.Contains("双侧限位"),"Both-limit explanation");
 fake.Positive=false; fake.Negative=false; Refresh(f,"通信正常");
 Check(((Button)Field(f,"btnJogPositive")).Enabled && ((Button)Field(f,"btnJogNegative")).Enabled,"Cleared limits restore directions");
 fake.Positive=true; fake.Negative=true; Refresh(f,"双侧限位同时触发 · 请检查信号");
 using(var bitmap=new Bitmap(f.Width,f.Height)) { f.DrawToBitmap(bitmap,new Rectangle(0,0,f.Width,f.Height)); bitmap.Save("obj/ui-preview.png"); }
 f.Close();
 }
 Console.WriteLine("PASS: UI disconnected state, position refresh, live signals, directional gating");
 }
}
