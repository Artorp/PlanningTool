@echo off
rem This file contains the paths for the Oxygen Not Included mod folder(s)

rem Do not use trailing backslashes in paths
set DIST_FOLDER="..\Dist"
set MOD_FOLDER_DEV="%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Dev"

rem Set name to parent folder of parent folder of current script
for %%A in ("%~dp0..") do set "MOD_NAME=%%~nxA"

echo %MOD_NAME%
