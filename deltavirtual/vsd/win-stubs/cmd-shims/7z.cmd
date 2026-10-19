@echo off
rem 7z shim for zephyr sdk setup.cmd. windows tar (bsdtar) can unpack 7z.
setlocal enabledelayedexpansion
set ARGS=%*
set ARGS=%ARGS:x =%
rem setup.cmd calls: 7z x -o. filename.7z
rem bsdtar wants:    tar xf filename.7z
set FILE=
:loop
if "%~1"=="" goto run
set ARG=%~1
if /i "!ARG!"=="x" (shift & goto loop)
if "!ARG:~0,3!"=="-o." (shift & goto loop)
if "!ARG:~0,2!"=="-o" (shift & goto loop)
set FILE=%~1
shift
goto loop
:run
if "%FILE%"=="" (
  echo 7z-shim: no archive 1>&2
  exit /b 1
)
C:\Windows\System32\tar.exe xf "%FILE%"
exit /b %ERRORLEVEL%
