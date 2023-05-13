@echo off
rem This script creates a symbolic link from ONI mod folder to git repository folder
rem Load paths from paths.bat file
call paths.bat
rem Check if mod folder exists
if exist %MOD_FOLDER_DEV%\%MOD_NAME% (
  echo Deleting existing mod folder... %MOD_FOLDER_DEV%\%MOD_NAME%
  rd /S %MOD_FOLDER_DEV%\%MOD_NAME%
)
echo Creating symbolic link...
mklink /J %MOD_FOLDER_DEV%\%MOD_NAME% %DIST_FOLDER%
echo %MOD_FOLDER_DEV%\%MOD_NAME% %DIST_FOLDER%
echo Done.
