::------------------------------------------------
:: Make script by Philip Arvidsson
::     <contact@philiparvidsson.com>
::------------------------------------------------

@echo off & setlocal

::------------------------------------------------
:: CONSTANTS
::------------------------------------------------

:: Base directory.
set BaseDir=%cd%

:: Directory where game content is located.
set ContentDir=%BaseDir%\src\Starburst\Content

:: Intermediate directory.
set IntermDir=%BaseDir%\obj

:: Output directory.
set OutputDir=%BaseDir%\bin

:: Source file directory.
set SourcesDir=%BaseDir%\src

:: Output executable name.
set Exe=Program.exe

:: Build tool paths.
set Csc="C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\csc.exe"
set Mgcb="C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MGCB.exe"

:: Build tool flags.

set CscFlags=   ^
    /define:GAMEPAD_VIBRATION ^
    /nologo     ^
    /optimize   ^
    /target:exe ^
    /warn:4

set MgcbFlags=                        ^
    /compress                         ^
    /intermediateDir:%IntermDir%\mgcb ^
    /outputDir:%OutputDir%\Content\   ^
    /quiet                            ^
    /workingDir:%ContentDir%

:: Libraries to link with when compiling the program.
set Libs= ^
    /r:"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MonoGame.Framework.dll" ^
    /r:"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\SharpDX.dll"            ^
    /r:"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\SharpDX.XInput.dll"

::------------------------------------------------
:: SCRIPT
::------------------------------------------------

:: Path to currently executing batch.
set Self="%~dp0%~nx0"

:: This is the target we're make'ing. If no target is set, set the target to
:: "all" to make sure we do everything.
set Target=%1
if "%Target%"=="" set Target=all

if "%Target%"=="all" (
    call %Self% program
    call %Self% content
) else if "%Target%"=="clean" (
    echo Cleaning...
    del %OutputDir%\%Exe%
    rmdir /S /q %IntermDir%
    rmdir /S /q %OutputDir%\Content
) else if "%Target%"=="content" (
    echo Building content...
    if not exist "%OutputDir%" mkdir %OutputDir%
    if not exist "%IntermDir%" mkdir %IntermDir%

    echo #mgcb temp file>content.mgcb
    for %%Q in (%MgcbFlags%) do (
        echo %%Q>>content.mgcb
    )

    for /R %ContentDir% %%Q in (*.fx; *.jpg; *.png; *.spritefont) do (
        echo /build:%%Q>>content.mgcb
    )

    for /R %ContentDir%\sound\effects %%Q in (*.mp3; *.wav) do (
        echo /processor:SoundEffectProcessor>>content.mgcb
        echo /build:%%Q>>content.mgcb
    )

    for %%Q in (%ContentDir%\sound\*.mp3; %ContentDir%\sound\*.wav) do (
        echo /processor:SongProcessor>>content.mgcb
        echo /build:%%Q>>content.mgcb
    )

    %Mgcb% /@:content.mgcb>nul
    del content.mgcb
) else if "%Target%"=="program" (
    echo Building program...
    if not exist "%OutputDir%" mkdir %OutputDir%
    if not exist "%IntermDir%" mkdir %IntermDir%

    echo #csc temp file>program.rsp

    echo /out:"%OutputDir%\%Exe%">>program.rsp

    for %%Q in (%CscFlags%) do (
        echo %%Q>>program.rsp
    )

    for %%Q in (%Libs%) do (
        echo %%Q>>program.rsp
    )

    for /R %SourcesDir% %%Q in (*.cs) do (
        echo %%Q>>program.rsp
    )

    %Csc% @program.rsp
    del program.rsp
) else if "%Target%"=="run" (
    if not exist "%OutputDir%\%Exe%" call %Self%
    echo Running program...
    %OutputDir%\%Exe%
) else (
     echo Unknown target: %Target%
)
