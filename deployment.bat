@echo off

REM Set variables
set PROJECT_PATH=".\src"
set BUILD_CONFIG=Release
set DEPLOYMENT_PATH="E:\Project\deploy"
set APP_SETTINGS_FILE_PATH="E:\Project\isoft\2-file deploy\appsettings.Development.json"

echo
echo
echo ==============================================================================
echo Navigate to project directory
echo ==============================================================================
cd %PROJECT_PATH%

REM Check status and pull commits

echo
echo
echo ==============================================================================
echo Clean previous build artifacts
echo ==============================================================================
dotnet clean

echo
echo
echo ==============================================================================
echo Restore dependencies
echo ==============================================================================
dotnet restore

echo
echo
echo ==============================================================================
echo Build the project
echo ==============================================================================
dotnet build --configuration %BUILD_CONFIG%

echo
echo
echo ==============================================================================
echo Optionally, run tests
echo ==============================================================================
dotnet test --configuration %BUILD_CONFIG%

echo
echo
echo ==============================================================================
echo Publish the project
echo ==============================================================================
dotnet publish --no-restore --configuration %BUILD_CONFIG% --output %DEPLOYMENT_PATH%

echo
echo
echo ==============================================================================
echo Copy appsettings.json to deployment folder
echo ==============================================================================
copy %APP_SETTINGS_FILE_PATH% %DEPLOYMENT_PATH%\appsettings.json

echo
echo
echo Deployment completed.

pause
