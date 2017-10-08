REM Get Git hash and set to gitversion
 FOR /F "tokens=* USEBACKQ" %%A IN (`"C:\Program Files\Git\bin\git.exe" describe --tags --long --always`) DO SET gitVersion=%%A
 ECHO %gitVersion%
 REM Replace text in temp file and rename it 
 REM %1 = $(ProjectDir)
 POWERSHELL -COMMAND "(gc %1Properties\AssemblyInfoTemplate.cs) -replace '\$GITVERSIONPLACEHOLDER\$', '%gitVersion%' | out-file %1Properties\AssemblyInfo.cs" ;
 