@ECHO OFF

SET FAKE_CLI_FOLDER=.fake
SET FAKE_CLI="%FAKE_CLI_FOLDER%/fake.exe"

IF EXIST %FAKE_CLI% (
  ECHO "Deleting '%FAKE_CLI_FOLDER%' folder"
  RMDIR /Q /S "%FAKE_CLI_FOLDER%"
)

IF NOT EXIST %FAKE_CLI% (
  dotnet tool install fake-cli --tool-path %FAKE_CLI_FOLDER%
)

%FAKE_CLI% run build.fsx --target "Done"