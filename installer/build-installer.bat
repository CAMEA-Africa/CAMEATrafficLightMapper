@echo off
setlocal

echo === Building CAMEA Traffic Light Mapper Installer ===
echo.

:: Clean previous build
if exist "_temp" rmdir /s /q "_temp"
if exist "publish" rmdir /s /q "publish"

:: Step 1: Publish to temp folder
echo [1/3] Publishing .NET application...
dotnet publish "..\CAMEATrafficLightMapper\CAMEATrafficLightMapper.csproj" -c Release -r win-x64 --self-contained -o "_temp"
if errorlevel 1 (
    echo ERROR: dotnet publish failed!
    pause
    exit /b 1
)

:: Step 2: Compile Inno Setup installer
echo [2/3] Compiling installer...
where iscc >nul 2>&1
if errorlevel 1 (
    if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" CAMEATrafficLightMapperInstaller.iss
    ) else (
        echo ERROR: Inno Setup compiler (ISCC.exe) not found!
        echo Install Inno Setup 6 from https://jrsoftware.org/isdl.php
        rmdir /s /q "_temp"
        pause
        exit /b 1
    )
) else (
    iscc CAMEATrafficLightMapperInstaller.iss
)

if errorlevel 1 (
    echo ERROR: Inno Setup compilation failed!
    rmdir /s /q "_temp"
    pause
    exit /b 1
)

:: Step 3: Clean up temp folder
echo [3/3] Cleaning up...
rmdir /s /q "_temp"

echo.
echo === Done ===
echo.
echo   publish\CAMEATrafficLightMapperSetup.exe
echo.
pause
