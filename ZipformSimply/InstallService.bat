@ECHO OFF

net stop "SimplyService"
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /U C:\Dev\Zipform\ZipformSimply\ZipformSimply\bin\Debug\ZipformSimply.exe

pause

C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\Dev\Zipform\ZipformSimply\ZipformSimply\bin\Debug\ZipformSimply.exe
net start "SimplyService"

pause

