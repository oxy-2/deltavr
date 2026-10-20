@echo off
setlocal
cd /d "%~dp0"

where powershell  1>nul 2>nul
if errorlevel 1 (
    echo powershell not found
    pause
    exit /b 1
)

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0start-vsd1.ps1"
set EXITCODE=%ERRORLEVEL%

echo.
if not "%EXITCODE%" == "0" (
    echo exited with code: %EXITCODE%
)
pause
exit /b %EXITCODE%