; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Enginette Client"
#define MyAppVersion "1.0.6"
#define MyAppPublisher "DDev"
#define MyAppURL "https://www.example.com/"
#define MyAppExeName "enginette-client.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{271537A2-2730-4088-B859-E7E4BA59584C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=E:\projects\enginette-client\LICENSE
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=E:\projects\enginette-client\setups
OutputBaseFilename=Enginette Client {#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "E:\projects\enginette-client\bin\release\net48\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\projects\enginette-client\bin\release\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
Root: HKCR; Subkey: "enginette-client"; ValueType: string; ValueName: ""; ValueData: "Enginette Patcher"
Root: HKCR; Subkey: "enginette-client"; ValueType: string; ValueName: "Directory"; ValueData: "{app}"
Root: HKCR; Subkey: "enginette-client"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""
Root: HKCR; Subkey: "enginette-client\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\enginette-client.exe"" ""%1"""

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    if RegKeyExists(HKEY_CLASSES_ROOT, 'enginette-client') then
        RegDeleteKeyIncludingSubkeys(HKEY_CLASSES_ROOT, 'enginette-client');
  end;
end;

