@echo off

copy ..\bin\debug\RomPatch.exe .
copy 2005lgt_stock_A2WC522N.bin example.bin
copy 2005lgt_stock_A2WC522N.bin stock.bin
copy A2WC540B.bin expected.bin

if -%1- == -q- goto skipdump1

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect dump without baseline data
@echo ##

rompatch dump ecuhacks.mot

:skipdump1

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect error message about missing baseline data.
@echo ##

rompatch test ecuhacks.mot example.bin

:Note that we EXPECT an error here.
@if %ERRORLEVEL% == 1 echo Good!
@if %ERRORLEVEL% == 0 goto Fail

if -%1- == -q- goto quietbaseline

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Generating baseline data...
@echo ##

:quietbaseline

copy ecuhacks.mot ecuhacksplusbaseline.mot
rompatch baseline ecuhacks.mot example.bin >> ecuhacksplusbaseline.mot

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

if -%1- == -q- goto skipdump2

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect dump WITH baseline data
@echo ##

rompatch dump ecuhacksplusbaseline.mot

:skipdump2

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect successful validation
@echo ##

rompatch test ecuhacksplusbaseline.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect a successful patch.
@echo ##

rompatch apply ecuhacksplusbaseline.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect no differences
@echo ##

fc /b example.bin expected.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect error messages about unmet expectations
@echo ##

rompatch apply ecuhacksplusbaseline.mot example.bin

:Note that we EXPECT an error here.
@if %ERRORLEVEL% == 1 echo Good!
@if %ERRORLEVEL% == 0 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect successful verification
@echo ##

rompatch applied ecuhacksplusbaseline.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect successful patch removal
@echo ##

rompatch remove ecuhacksplusbaseline.mot example.bin

@if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## Expect no differences
@echo ##

fc /b 2005lgt_stock_A2WC522N.bin example.bin

if %ERRORLEVEL% == 0 echo Good!
@if %ERRORLEVEL% == 1 goto Fail

@echo _____________________________________________________________________
@echo #####################################################################
@echo ## All tests passed!
@echo #####################################################################

goto end

:Fail
echo Failed!

:end

