rem @echo off
cls

SET string=everydaywriter5e:gardner10e:pocketspeak4e:reflectrelate3e:theguide10e

FOR %%x IN (%string::= %) DO (
 CALL :%%x
)

:everydaywriter5e
	SETLOCAL
	SET myType=everydaywriter5e
	SET myId=99342
	CALL :deploy
	ENDLOCAL
:gardner10e
	SETLOCAL
	SET myType=gardner10e
	SET myId=97328
	CALL :deploy
	ENDLOCAL
:pocketspeak4e
	SETLOCAL
	SET myType=pocketspeak4e
	SET myId=97620
	CALL :deploy
	ENDLOCAL
:reflectrelate3e
	SETLOCAL
	SET myType=reflectrelate3e
	SET myId=97265
	CALL :deploy
	ENDLOCAL
:theguide10e
	SETLOCAL
	SET myType=theguide10e
	SET myId=97204
	CALL :deploy
	ENDLOCAL
	echo done
	PAUSE
	Exit

:deploy

SET folder="..\..\DLAPScripts\PRODUCTS\XBook\_GLOBAL\ALL"
for /R "%folder%" %%i in (*.xml) do (..\..\Tools\Metadata\Metadata\bin\metadata.exe /a:executedlapscript /d:"%%i" /m:6650 /ti="%myType%" /c:%myId% /e:qa)

SET folder="..\..\DLAPScripts\PRODUCTS\XBook\_GLOBAL\QA"
for /R "%folder%" %%i in (*.xml) do (..\..\Tools\Metadata\Metadata\bin\metadata.exe /a:executedlapscript /d:"%%i" /m:6650 /ti="%myType%" /c:%myId% /e:qa)

SET folder="..\..\DLAPScripts\PRODUCTS\XBook\%myType%\ALL"
for /R "%folder%" %%i in (*.xml) do (..\..\Tools\Metadata\Metadata\bin\metadata.exe /a:executedlapscript /d:"%%i" /m:6650 /ti="%myType%" /c:%myId% /e:qa)

SET folder="..\..\DLAPScripts\PRODUCTS\XBook\%myType%\QA"
for /R "%folder%" %%i in (*.xml) do (..\..\Tools\Metadata\Metadata\bin\metadata.exe /a:executedlapscript /d:"%%i" /m:6650 /ti="%myType%" /c:%myId% /e:qa)

echo successfully deployed id (%myId%) for (%folder%)
	
