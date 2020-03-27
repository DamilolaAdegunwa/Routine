@ echo off 
set _name=丝路智旅-自助机API
set _nod=EPT.SelfAPI

@ echo off
echo 正在启用管理员权限... 
%1 %2
ver|find "5.">nul&&goto :st
mshta vbscript:createobject("shell.application").shellexecute("%~s0","goto :st","","runas",1)(window.close)&goto :eof
 
:st
copy "%~0" "%windir%\system32\"
echo 启用管理员权限成功
echo=
echo 启动%_name%服务
sc start %_nod%
echo=
echo 查询%_name%服务状态……
echo ===== STATE : 1 已停止
echo ===== STATE : 2 正在启动
echo ===== STATE : 3 正在停止
echo ===== STATE : 4 正在运行
echo=
echo 当前状态:
choice /t 2 /d y /n >nul
sc query %_nod%
echo. & pause 