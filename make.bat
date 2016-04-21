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
set Csc="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set Mgcb="C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MGCB.exe"

:: Build tool flags.

set CscFlags=   ^
    /nologo     ^
    /optimize   ^
    /target:exe

set MgcbFlags=                        ^
    /compress                         ^
    /intermediateDir:%IntermDir%\mgcb ^
    /outputDir:%OutputDir%\Content\   ^
    /quiet                            ^
    /workingDir:%ContentDir%

:: Libraries to link with when compiling the program.
set Libs= ^
    /r:"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MonoGame.Framework.dll"

::------------------------------------------------
:: SCRIPT
::------------------------------------------------

:: Path to currently executing batch.
set Self="%~dp0%~nx0"

:: This is the target we're make'ing. If no target is set, set the target to
:: "all" to make sure we do everything.
set Target=%1
if "%Target%"=="" set Target=all

:: Find content and build it with MGCB.
set Content=

for /R %ContentDir% %%Q in (*.jpg; *.png; *.spritefont) do (
    call set "Content=%%Content%% /build:"%%Q""
)

:: TODO: Figure out a way to differentiate songs and sounds effects.
for /R %ContentDir% %%Q in (*.mp3) do (
    call set "Content=%%Content%% /processor:SoundEffectProcessor /build:"%%Q""
)

:: Find source code files.
set Sources=
for /R %SourcesDir% %%Q in (*.cs) do call set "Sources=%%Sources%% "%%Q""

if "%Target%"=="all" (
    call %Self%  program
    if not "%Content%"=="" call %Self% content
) else if "%Target%"=="clean" (
    echo Cleaning...
    del %OutputDir%\%Exe%
    rmdir /S /q %IntermDir%
    rmdir /S /q %OutputDir%\Content
) else if "%Target%"=="content" (
    echo Building content...
    if not exist "%OutputDir%" mkdir %OutputDir%
    if not exist "%IntermDir%" mkdir %IntermDir%
    %Mgcb% %MgcbFlags% %Content%
) else if "%Target%"=="program" (
    echo Building program...
    if not exist "%OutputDir%" mkdir %OutputDir%
    if not exist "%IntermDir%" mkdir %IntermDir%
    %Csc% /out:"%OutputDir%\%Exe%" %CscFlags% %Libs% %Sources%
) else if "%Target%"=="run" (
    if not exist "%OutputDir%\%Exe%" call %Self%
    echo Running program...
    %OutputDir%\%Exe%
) else (
     echo Unknown target: %Target%
)
