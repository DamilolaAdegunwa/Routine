@ echo off 
set _name=˿·����-������API
set _nod=EPT.SelfAPI

@ echo off
echo �������ù���ԱȨ��... 
%1 %2
ver|find "5.">nul&&goto :st
mshta vbscript:createobject("shell.application").shellexecute("%~s0","goto :st","","runas",1)(window.close)&goto :eof
 
:st
copy "%~0" "%windir%\system32\"
echo ���ù���ԱȨ�޳ɹ�
echo=
echo ����%_name%����
sc start %_nod%
echo=
echo ��ѯ%_name%����״̬����
echo ===== STATE : 1 ��ֹͣ
echo ===== STATE : 2 ��������
echo ===== STATE : 3 ����ֹͣ
echo ===== STATE : 4 ��������
echo=
echo ��ǰ״̬:
choice /t 2 /d y /n >nul
sc query %_nod%
echo. & pause 