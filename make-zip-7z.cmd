@echo off
setlocal
where /q 7z
IF ERRORLEVEL 1 (
    set ZIP_BIN="C:\Program Files\7-Zip\7z.exe"
) ELSE (
    set ZIP_BIN=7z
)
set ZIP_NAME=Updater.zip
echo %ZIP_NAME%
rem using 7-Zip (https://www.7-zip.org)
cd "%~dp0\Translation"
ren zh_TW zh
%ZIP_BIN% a -tzip %ZIP_NAME% * -mx=7 -xr0!*.log -xr0!*.pdb
ren zh zh_TW
cd "%~dp0"
move "%~dp0\Translation\%ZIP_NAME%" "%~dp0"
pause
