@echo off
cd /d "%~dp0"
dotnet restore WaveMotionControl.sln
if errorlevel 1 goto :error
dotnet build WaveMotionControl.sln -c Release
if errorlevel 1 goto :error
echo.
echo Build completed.
pause
exit /b 0
:error
echo.
echo Build failed. Verify .NET 9 SDK and Visual Studio Desktop Development workload.
pause
exit /b 1
