
set LUBAN_DLL=../Tools/Luban/Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=../Assets/Game/Tables/Gen ^
    -x outputDataDir=../Assets/Resource/Tables/

pause