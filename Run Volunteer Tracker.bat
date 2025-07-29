@echo off
echo ========================================
echo    Volunteer Service Tracker
echo ========================================
echo.
echo Starting the application...
echo.
echo If you see any security warnings, please allow the application to run.
echo This is a safe, legitimate application for tracking volunteer hours.
echo.
pause
start "" "VolunteerTracker.exe"
if errorlevel 1 (
    echo.
    echo Error: Could not start VolunteerTracker.exe
    echo Please make sure all files are extracted properly.
    echo.
    pause
) 