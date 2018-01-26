SET LogDir=.\MSBuild
IF EXIST %LogDir% (
	RD /S /Q %LogDir%
)
MD %LogDir%
@set Configuration=Release
C:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe buildVS2013.proj /t:Build @build.rsp

pause
