@echo off
set WORK_DIR=%cd%
set DEFINE=UNITY_5_4_OR_NEWER;UNITY_STANDALONE_WIN;NET_STANDARD_2_0
set /P UNITY_HOME=<.UNITY_HOME.env

mkdir build
mkdir ..\_dist_
copy Plugin.csproj.keep build\Plugin.csproj
xcopy /S/Q Assets\Scripts\ build\
cd build
powershell -Command "(gc Plugin.csproj) -replace '{{DEFINE}}', '%DEFINE%' | Out-File Plugin.csproj"
powershell -Command "(gc Plugin.csproj) -replace '{{UNITY_HOME}}', '%UNITY_HOME%' | Out-File Plugin.csproj"
powershell -Command "(gc Plugin.csproj) -replace '{{WORK_DIR}}', '%WORK_DIR%' | Out-File Plugin.csproj"
dotnet build -c=Release 
cd ..
DEL /Q/S build\bin\Release\netstandard2.1\Unity*
move build\bin\Release\netstandard2.1\*.dll ..\_dist_\
RD /Q/S build
pause


