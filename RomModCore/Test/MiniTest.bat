@echo off
copy ..\bin\debug\RomPatch.exe .
copy 2005lgt_stock_A2WC522N.bin example.bin
copy 2005lgt_stock_A2WC522N.bin stock.bin
copy A2WC540B.bin expected.bin
copy EcuHacks.mot patch.mot
rompatch baseline ecuhacks.mot stock.bin >> patch.mot

echo --------
echo COMPARE
echo --------
fc /b example.bin stock.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

echo --------
echo TEST
echo --------
RomPatch.exe test patch.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

echo --------
echo APPLY
echo --------
RomPatch.exe apply patch.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

echo --------
echo COMPARE
echo --------
fc /b example.bin expected.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

echo --------
echo APPLIED?
echo --------
RomPatch.exe applied patch.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

echo --------
echo REMOVE
echo --------
RomPatch.exe remove patch.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

echo --------
fc /b example.bin stock.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail