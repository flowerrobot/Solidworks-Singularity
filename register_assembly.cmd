@echo off
rem 
rem Registering Solidworks Addin
rem 
echo #####################################################
echo Adding the Singularity Solidworks Addin to this system...
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

echo. 
echo ### Registering COM Assemblies
set FMWK="v4.0.30319"
set Target="C:\Users\sethr\OneDrive\Documents\Programming\GitHub\Solidworks Addin\Singularity Addin\Singularity Addin\bin\Debug\Singularity Addin.dll"
set Exec="%Windir%\Microsoft.NET\Framework64\%FMWK%\regasm.exe"
echo %Exec% %Target% /codebase
IF EXIST %Exec% %Exec% %Target% /codebase

echo. 
echo ### Done - report any errors to servicedesk
pause