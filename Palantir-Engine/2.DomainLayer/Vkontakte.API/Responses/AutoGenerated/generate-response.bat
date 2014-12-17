@echo off

rem Args
rem %1 - XML file name without extension "a.xml" -> "a"
rem %2 - Namespace name, where response will be put into. Like 
rem      Ix.Palantir.Vkontakte.API.Responses.VideosFeed

rem Usually %PROGRAMFILES% or %PROGRAMFILES(x86)%
set SDK_DIR=%PROGRAMFILES%\Microsoft SDKs\Windows\v7.0A
set XSD_EXE="%SDK_DIR%\Bin\xsd.exe"

call :main "%1.xml" "%1.xsd" "%2"
pause
exit /b

:main

echo ---------------------------------------------------------------
echo Generating of xsd schema...
echo ---------------------------------------------------------------
%XSD_EXE% %1
if ERRORLEVEL 1 exit /b

echo ---------------------------------------------------------------
echo generating of C# class...
echo ---------------------------------------------------------------
%XSD_EXE% %2 /classes /namespace:%3
if ERRORLEVEL 1 exit /b

echo ---------------------------------------------------------------
echo cleaning...
del %2
echo done!
echo ---------------------------------------------------------------

goto :eof
