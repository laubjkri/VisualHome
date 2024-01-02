@echo off
setlocal

set "processName=Docker Desktop.exe"
set "programPath=C:\Program Files\Docker\Docker\%processName%"

::Check if docker is running
tasklist /FI "IMAGENAME eq %processName%" 2>NUL | find /I /N "%processName%" >NUL

if "%ERRORLEVEL%"=="1" (
    echo %processName% is not running. Starting %processName%...
    start "" "%programPath%"
	
	:WaitForStart
	timeout /t 2 /nobreak >NUL
	tasklist /FI "IMAGENAME eq %processName%" 2>NUL | find /I "%processName%" >NUL
    if "%ERRORLEVEL%"=="1" (
        goto WaitForStart
	) else (
		echo %processName% has started.
	)
	
) else (
    echo %processName% is already running.
)

:: Start container
docker run --rm --name timescaledb -p 5432:5432 -v /c/Temp/VisualHome/db:/var/lib/postgresql/data -e POSTGRES_PASSWORD=hestehest -e POSTGRES_USER=kristian -e POSTGRES_DB=postgres timescale/timescaledb:latest-pg15
cmd /k

endlocal