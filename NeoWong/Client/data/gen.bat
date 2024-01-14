set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
	-x outputCodeDir=%WORKSPACE%\Unity\NeoWong\Assets\GameScripts\HotFix\Logic\Config\Tables ^
    -x outputDataDir=%WORKSPACE%\Unity\NeoWong\Assets\AssetRaw\Configs\bytes

pause