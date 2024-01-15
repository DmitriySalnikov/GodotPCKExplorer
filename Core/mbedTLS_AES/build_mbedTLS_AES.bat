@echo off


for /f "tokens=* USEBACKQ" %%F IN (`"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) DO (
    set "VS_PATH=%%F"
)
if exist "%VS_PATH%" (
    call "%VS_PATH%\VC\Auxiliary\Build\vcvarsall.bat" x64
)

set TargetPlatformVersion=10.0
set PlatformToolset=v143

echo "------- Build x64 -------"
msbuild mbedTLS_AES.sln /t:mbedTLS_AES /p:Configuration=Release;Platform=x64;WindowsTargetPlatformVersion=%TargetPlatformVersion%;PlatformToolset=%PlatformToolset% /m:4
if errorlevel 1 ( echo Failed to compile x64. Code: %errorlevel% && exit /b %errorlevel% )

echo "------- Build x86 -------"
msbuild mbedTLS_AES.sln /t:mbedTLS_AES /p:Configuration=Release;Platform=x86;WindowsTargetPlatformVersion=%TargetPlatformVersion%;PlatformToolset=%PlatformToolset% /m:4
if errorlevel 1 ( echo Failed to compile x86. Code: %errorlevel% && exit /b %errorlevel% )
