::Error Thrown by Post-Build Script
@echo off
setlocal ENABLEDELAYEDEXPANSION

:: USER VARS
set logfile=BuildLog.txt
set IgnoreConfig=Debug

:: ===================================================================================

echo.
echo  -------------- Starting Unibox Post-build Script --------------

set "ConfigurationName=%~1"
set "SolutionDir=%~2"

:: Get in right Dir (Config Build Folder root)
cd ../Builds/Release
if [!ConfigurationName!] == [Debug] (cd ../Debug)

type NUL > %logfile%
call :log "Started: !DATE! !TIME!"
call :log  "Working Dir: !cd!"
call :log "Build Config Name: [!ConfigurationName!]"

set "version=0.0.0.0"

if exist "!SolutionDir!\Unibox.version.txt" (
    for /f "usebackq delims=" %%i in ("!SolutionDir!\Unibox.version.txt") do (
        set "version=%%i"
     
    )
) else (
    echo Version file not found: "!SolutionDir!\Unibox.version.txt"
)

call :log "Built Version: [!version!]"

call :log "Doing any Deployment Archive File operations..."

:: ------ PRODUCE DEPLOYMENT ARCHIVE ------------

if [!ConfigurationName!] == [%IgnoreConfig%] (
    call :log "Not produced for debug builds. Skipping."
    goto Skip_DeploymentAchive
) 

powershell -command "Compress-Archive -Path 'Unibox\*' -DestinationPath 'Unibox.zip' -Force"

call :log  "Archive created"

:Skip_DeploymentAchive

echo  -------------- Unibox Post-build Script Complete --------------
echo.

exit /b 0

REM --- Subroutine to Log to Console AND File ---
:Log
    REM %~1 gets the argument passed to the subroutine, removing quotes
    SET "MSG=%~1"

    REM Echo to the console
    ECHO !MSG!

    REM Append to the log file
    ECHO !MSG! >> %logfile%

    GOTO :EOF