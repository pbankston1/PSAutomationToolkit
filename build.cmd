@echo off
REM ============================================================
REM  PS Automation Toolkit Pro - Build Script
REM  Requires: .NET 8 SDK, Visual Studio 2022 or VS Build Tools
REM ============================================================

setlocal

set PROJECT_DIR=%~dp0PSAutomationToolkit
set OUTPUT_DIR=%~dp0Build\Release
set PUBLISH_DIR=%~dp0Build\Publish

echo.
echo  ================================================
echo   PS Automation Toolkit Pro - Build
echo  ================================================
echo.

REM Check dotnet is available
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found. Install from https://dot.net
    pause
    exit /b 1
)

echo [1/4] Restoring packages...
dotnet restore "%PROJECT_DIR%\PSAutomationToolkit.csproj"
if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Restore failed & pause & exit /b 1 )

echo [2/4] Building (Debug)...
dotnet build "%PROJECT_DIR%\PSAutomationToolkit.csproj" -c Debug -o "%OUTPUT_DIR%\Debug"
if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Build failed & pause & exit /b 1 )

echo [3/4] Building (Release)...
dotnet build "%PROJECT_DIR%\PSAutomationToolkit.csproj" -c Release -o "%OUTPUT_DIR%\Release"
if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Release build failed & pause & exit /b 1 )

echo [4/4] Publishing single-file executable...
dotnet publish "%PROJECT_DIR%\PSAutomationToolkit.csproj" ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -o "%PUBLISH_DIR%"
if %ERRORLEVEL% NEQ 0 ( echo [ERROR] Publish failed & pause & exit /b 1 )

echo.
echo  ================================================
echo   BUILD COMPLETE
echo   Executable: %PUBLISH_DIR%\PSAutomationToolkit.exe
echo  ================================================
echo.

REM Optional: open output folder
explorer "%PUBLISH_DIR%"
pause
