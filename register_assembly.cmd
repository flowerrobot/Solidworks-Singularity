@echo off
rem 
rem Registering Solidworks Addin
rem 
echo #####################################################
echo Adding the Outotec Solidworks Addin to this system...
echo #####################################################
echo. 
:runCheck
echo Is Solidworks running?
tasklist /FI "IMAGENAME eq sldworks.exe" 2>NUL | find /I /N "sldworks.exe">NUL
if "%ERRORLEVEL%"=="0" (
	echo Programm sldworks.exe is running - close Solidworks please
	pause
	GOTO runCheck
) ELSE (
	echo Solidworks not running - lets start!
)

echo. 
rem echo ### Importing Registery keys 
rem reg IMPORT "C:\EDM_QA\11 Document Templates\SolidWorks\OutotecAddin\OSwAddin.reg"

echo. 
echo ### Registering COM Assemblies
set FMWK="v4.0.30319"
set Target="C:\Users\setruh\OneDrive\Documents\Programming\BitBucket\SW Addin\Outotec SW Addin\bin\Debug\Outotec SW Addin.dll"
set Exec="%Windir%\Microsoft.NET\Framework64\%FMWK%\regasm.exe"
echo %Exec% %Target% /codebase
IF EXIST %Exec% %Exec% %Target% /codebase

echo. 
echo ### Done - report any errors to servicedesk
pause