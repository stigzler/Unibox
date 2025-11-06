@echo off
setlocal ENABLEDELAYEDEXPANSION

set "ConfigurationName=%~1"
set "SolutionDir=%~2"

echo  -------------- Starting Unibox PLUGIN Post-build Script --------------

:: ------ COPY PLUGIN FOLDER IN DEBUG MODE ------------

if [!ConfigurationName!]==[Debug]  (
	echo Build = debug, therfore copying Unibox Plugin to LaunchBox Development Install Plugins folder...
	robocopy "C:\Users\stigz\source\repos\0.MyCode\CS\Core\Unibox\Builds\Debug\Unibox\Plugin" "C:\Users\stigz\LaunchBoxDevelopmentInstall\Plugins\Unibox"
)

exit /b 0
