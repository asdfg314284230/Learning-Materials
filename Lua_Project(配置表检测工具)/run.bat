@echo off
REM 打包框架Lua脚本
REM 当前盘符：%~d0
REM 当前路径：%cd%
REM 当前执行命令行：%0
REM 当前bat文件路径：%~dp0
REM 当前bat文件短路径：%~sdp0
REM 声明采用UTF-8编码
chcp 65001

REM 需要设置的绝对路径
set sourcefile_path="D:\Project_Unity\S_producer\config_library\v0.0.1\lua"
set destinationfile_path="D:\Project_Unity\S_producer\config_library\Lua_Project\config"

REM 先copy文件，好像lua脚本对文件操作复制删除操作好像很弱智，所以在这里就直接弄了，感觉复杂度高了很多，痛苦。

rd /S /Q %destinationfile_path%

mkdir config

xcopy %sourcefile_path% %destinationfile_path% /s /e /y

lua.exe D:\Project_Unity\S_producer\config_library\Lua_Project\lua_test.lua %~dp0 C:\Users\Administrator\Desktop\mLua_Project\config

pause