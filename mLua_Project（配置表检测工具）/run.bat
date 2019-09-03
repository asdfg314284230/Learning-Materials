@echo off
REM 打包框架Lua脚本
REM 当前盘符：%~d0
REM 当前路径：%cd%
REM 当前执行命令行：%0
REM 当前bat文件路径：%~dp0
REM 当前bat文件短路径：%~sdp0
REM 声明采用UTF-8编码
chcp 65001

lua.exe C:\Users\Administrator\Desktop\mLua_Project\lua_test.lua %~dp0 C:\Users\Administrator\Desktop\mLua_Project\config
pause