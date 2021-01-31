@ECHO OFF
SETLOCAL

SET __VSCMD_ARG_NO_LOGO=1
CD "%~dp0"
CALL "%VSAPPIDDIR%\..\Tools\VsDevCmd.bat"
Powershell -NoProfile -ExecutionPolicy Bypass -File Make.ps1 -TargetName %1 -OutDir %2 -ConfigurationName %3
EXIT /B %ERRORLEVEL%

ENDLOCAL
