using System;
using System.Collections.Generic;
using HslCommunication;
using PlcAxisControl;
class Fake : PlcHelper {
 public List<string> Writes = new List<string>();
 public bool Negative, Positive, FailSpeed, FailRead;
 public override OperateResult<bool> ReadBool(string a) { return new OperateResult<bool> { IsSuccess=!FailRead, Content=a==PlcAddr.Axis1DJogNeg ? Negative : a==PlcAddr.Axis1DJogPos && Positive }; }
 public override OperateResult<float> ReadFloat(string a) { return new OperateResult<float> { IsSuccess=true, Content=10 }; }
 public override OperateResult WriteFloat(string a,float v) { Writes.Add(a); return new OperateResult { IsSuccess=!(FailSpeed && a==PlcAddr.Axis1SetSpeed) }; }
 public override OperateResult WriteBool(string a,bool v) { Writes.Add(a+"="+v); return new OperateResult { IsSuccess=true }; }
}
class Test {
 static void Assert(bool b,string m) { if(!b) throw new Exception(m); }
 static void Reject(Action a) { try { a(); } catch(InvalidOperationException) { return; } throw new Exception("Expected rejection"); }
 static void Main() {
 var f=new Fake(); f.Move(true,20,5);
 Assert(string.Join(",",f.Writes)=="VD3000,VD3004,M1.0=False,M1.0=True,M1.0=False","Position/velocity then pulse");
 f=new Fake { Positive=true }; Reject(()=>f.Move(true,20,5)); Assert(f.Writes.Count==0,"Limit must prevent writes");
 f=new Fake { Positive=true }; f.Move(true,0,5); Assert(f.Writes.Count==5,"Allow retreat");
 f=new Fake { FailSpeed=true }; Reject(()=>f.Move(true,20,5)); Assert(!f.Writes.Contains("M1.0=True"),"Failed speed must not start");
 f=new Fake(); f.StopAll(); Assert(f.Writes.Count==8 && f.Writes.TrueForAll(x=>x.EndsWith("=False")),"Stop all eight commands");
 f=new Fake { Negative=true }; Reject(()=>f.Move(true,0,5)); Assert(f.Writes.Count==0,"Negative limit must prevent writes");
f=new Fake { Negative=true }; f.Move(true,20,5);
f=new Fake { FailRead=true }; Reject(()=>f.Move(true,20,5)); Assert(f.Writes.Count==0,"Read failure must prevent motion");
f=new Fake(); f.Move(false,20,5); Assert(string.Join(",",f.Writes)=="VD3008,VD3012,M1.1=False,M1.1=True,M1.1=False","Axis 2 mapping");
f=new Fake { Negative=true }; Reject(()=>f.StartJog(true,true,5)); Assert(f.Writes.Count==0,"Jog into negative limit rejected");
f=new Fake { Positive=true }; f.StartJog(true,true,5); Assert(f.Writes.Contains("M3.0=True") && f.Writes.Contains("M3.1=False"),"Left retreat uses corrected mapping");
f=new Fake(); Reject(()=>f.Move(true,20,0)); Assert(f.Writes.Count==0,"Zero velocity rejected");
Console.WriteLine("PASS: 12 offline motion scenarios");
 }
}
