Versioning follows semvar: Major.Minor.Release.Debug
File Versions updated automatically during build process via respective .proj files
Current version is stored in [solutionDir]/Unibox.version.txt - you can alter this on event of a mis-build
You can also edit [solutionDir]/Unibox.Plugin.version.txt for same effect as above

TRY TO AVOID "REBUILD PROJECT/SOLUTION" where possible as this may update the Plugin Dll unnecessarily, forcing user to go through plugin update install process unnecessarily.
Build solution is OK, as will only update the Plugin Dll if version if the plugin code has changed.

Major, Minor, Release built to:
[solutionDir]/Builds/Release

Also produces Unibox.zip - which is the deployment package