; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "CapsKey"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Luke Hill"
#define MyAppURL "https://computingman.visualstudio.com/CapsKey"
#define MyAppExeName "CapsKey.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{B1251296-8593-407B-9ECE-114DABB717EF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=no
DefaultGroupName=CapsKey
OutputBaseFilename={#MyAppName} version {#MyAppVersion}
SetupIconFile=..\App-Icon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\bin\Release\CapsKey.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\CapsKey.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\CapsKey.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\ToggleSwitch.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\doc\CapsKey_User_Guide.html"; DestDir: "{app}\doc"; Flags: ignoreversion
Source: "..\bin\Release\doc\*.png"; DestDir: "{app}\doc"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

