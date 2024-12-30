@echo off

REM Function to start a service in a new Command Prompt window
:start_service
setlocal
start cmd /k "%~1"
endlocal
exit /b

REM Start Service1 in a new Command Prompt
echo Starting Service1...
call :start_service "cd /d %~dp0Service1\Service1.Web && dotnet run --urls http://localhost:5001"

REM Start Service2 in a new Command Prompt
echo Starting Service2...
call :start_service "cd /d %~dp0Service2\Service2.Web && dotnet run --urls http://localhost:5002"

REM Start Service3 in a new Command Prompt
echo Starting Service3...
call :start_service "cd /d %~dp0Service3\Service3.Web && dotnet run --urls http://localhost:5003"

REM Wait briefly to ensure services are up
timeout /t 6 /nobreak >nul

REM Open Swagger UI for Service1 in the default browser
echo Opening Swagger UI for Service1...
start http://localhost:5001/swagger

REM Open Swagger UI for Service2 in the default browser
echo Opening Swagger UI for Service2...
start http://localhost:5002/swagger

REM Open Swagger UI for Service3 in the default browser
echo Opening Swagger UI for Service3...
start http://localhost:5003/swagger

echo All services started. Command Prompt windows are running each service, and Swagger UIs are open in the default browser.
pause
