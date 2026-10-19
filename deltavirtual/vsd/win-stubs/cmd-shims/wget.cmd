@echo off
rem wget shim for zephyr sdk setup.cmd. forwards to curl.
setlocal enabledelayedexpansion
set OUT=
set URL=
:loop
if "%~1"=="" goto run
if /i "%~1"=="-O" (
  set OUT=%~2
  shift
  shift
  goto loop
)
if /i "%~1"=="-q" (shift & goto loop)
if /i "%~1"=="-N" (shift & goto loop)
if "%~1"=="--show-progress" (shift & goto loop)
set URL=%~1
shift
goto loop
:run
if "%OUT%"=="" set OUT=%URL%
if "%URL%"=="" (
  echo wget-shim: no url 1>&2
  exit /b 1
)
curl -L --retry 3 -o "%OUT%" "%URL%"
exit /b %ERRORLEVEL%
