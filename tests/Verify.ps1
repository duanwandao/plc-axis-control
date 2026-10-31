$ErrorActionPreference = 'Stop'
$compiler = 'C:\Program Files\dotnet\sdk\9.0.320\Roslyn\bincore\csc.dll'
$framework = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319'
$refs = @('mscorlib','System','System.Core','System.Data','System.Drawing','System.Windows.Forms') | ForEach-Object { '/reference:' + $framework + '\' + $_ + '.dll' }
& 'C:\Program Files\dotnet\dotnet.exe' $compiler /nologo /out:obj\MotionTests.exe /reference:libs\HslCommunication.dll $refs tests\MotionTests.cs PlcHelper.cs PlcAddr.cs
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed' }
Copy-Item libs\HslCommunication.dll obj\HslCommunication.dll -Force
& .\obj\MotionTests.exe
if ($LASTEXITCODE -ne 0) { throw 'Tests failed' }
& 'C:\Program Files\dotnet\dotnet.exe' $compiler /nologo /target:winexe /out:obj\PlcAxisControl.Verified.exe /reference:libs\HslCommunication.dll /reference:libs\SunnyUI.dll /reference:libs\SunnyUI.Common.dll $refs Form1.cs Form1.Designer.cs PlcAddr.cs PlcHelper.cs Program.cs Properties\AssemblyInfo.cs
if ($LASTEXITCODE -ne 0) { throw 'App compilation failed' }
Copy-Item libs\SunnyUI.dll,libs\SunnyUI.Common.dll obj -Force
& 'C:\Program Files\dotnet\dotnet.exe' $compiler /nologo /main:UiTest /out:obj\UiTests.exe /reference:libs\HslCommunication.dll /reference:libs\SunnyUI.dll /reference:libs\SunnyUI.Common.dll $refs tests\MotionTests.cs tests\UiTests.cs Form1.cs Form1.Designer.cs PlcHelper.cs PlcAddr.cs
if ($LASTEXITCODE -ne 0) { throw 'UI test compilation failed' }
& .\obj\UiTests.exe
if ($LASTEXITCODE -ne 0) { throw 'UI tests failed' }
