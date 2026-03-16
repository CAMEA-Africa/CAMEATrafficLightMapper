; CAMEA Traffic Light Mapper Installer
; Inno Setup Script

#define MyAppName "CAMEA Traffic Light Mapper"
#define MyAppVersion "0.0.1"
#define MyAppPublisher "CAMEA Africa"
#define MyAppExeName "CAMEATrafficLightMapper.exe"
#define MyServiceName "CAMEALightMapper"

[Setup]
AppId={{B1C2D3E4-F5A6-4B7C-8D9E-0F1A2B3C4D5E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\CAMEATrafficLightMapper
DisableProgramGroupPage=yes
OutputDir=publish
OutputBaseFilename=CAMEATrafficLightMapperSetup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
WizardStyle=modern
SetupIconFile=compiler:SetupClassicIcon.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "_temp\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "_temp\wwwroot\*"; DestDir: "{app}\wwwroot"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "_temp\appsettings.json"; DestDir: "{app}"; Flags: onlyifdoesntexist

[Run]
Filename: "sc.exe"; Parameters: "create {#MyServiceName} binPath= ""{app}\{#MyAppExeName}"" start= auto"; Flags: runhidden waituntilterminated
Filename: "sc.exe"; Parameters: "start {#MyServiceName}"; Flags: runhidden waituntilterminated
Filename: "{sys}\cmd.exe"; Parameters: "/c start http://localhost:5166/settings.html"; Flags: runhidden nowait; Description: "Open settings page"

[UninstallRun]
Filename: "sc.exe"; Parameters: "stop {#MyServiceName}"; Flags: runhidden waituntilterminated
Filename: "sc.exe"; Parameters: "delete {#MyServiceName}"; Flags: runhidden waituntilterminated

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
