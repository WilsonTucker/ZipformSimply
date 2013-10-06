@ECHO OFF

del C:\Dev\Zipform\D\Prod\Simply_Energy\MISCLetter\ftp\dummy.txt

net stop "SimplyService"
echo BUILD NOW

pause

net start "SimplyService"
echo ATTACH TO PROCESS

pause

echo text >> C:\Dev\Zipform\D\Prod\Simply_Energy\MISCLetter\ftp\churn.csv

