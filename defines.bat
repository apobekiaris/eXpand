@echo off
set configuration=Debug
set vsver=vs2015
rem uncomment the line bellow if you not use vs2010
rem set vsver=vs2008
rem uncomment the line bellow if you not use vs2012
rem set vsver=vs2012
rem set vsver=vs2013
set ProgramFiles=%ProgramFiles(x86)%

set GACPATH="%WinDir%\assembly\GAC_MSIL\"
set Gac4path="%WinDir%\Microsoft.NET\assembly\GAC_MSIL\"

if '%vsver%'=='vs2008' goto vs2008
if '%vsver%'=='vs2010' goto vs2010
if '%vsver%'=='vs2012' goto vs2012
if '%vsver%'=='vs2013' goto vs2013
if '%vsver%'=='vs2015' goto vs2015

:vs2015
set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"

IF NOT EXIST %msbuild% goto VS2013Tools

set sn="%ProgramFiles%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 14.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 14.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 14.0\Common7\IDE\"
goto end
goto end

:vs2013
set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\IDE\"
goto end
goto end

:vs2012
set msbuild="%ProgramFiles%\MSBuild\10.0\Bin\MSBuild.exe"
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\IDE\"
goto end
goto end

:vs2008
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\"

goto end

:vs2010
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\"
goto end

:VS2013Tools
echo "You need to download Build Tools 2013 http://www.microsoft.com/en-us/download/details.aspx?id=40760"
pause
exit
:end