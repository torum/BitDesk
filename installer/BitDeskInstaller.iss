; -- BitDeskInstaller.iss --
; http://www.jrsoftware.org/isinfo.php

[Setup]
AppName=BitDesk
AppVersion=0.0.0.2  
VersionInfoVersion=0.0.0.2
DefaultDirName={pf}\BitDesk
DefaultGroupName=BitDesk
UninstallDisplayIcon={app}\BitDesk.exe
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
MinVersion=10.0
SetupIconFile=..\BitDesk\App.ico
OutputBaseFilename=BitDeskSetup
VersionInfoProductName=BitDesk
VersionInfoCompany=
VersionInfoCopyright=
VersionInfoDescription=

[Files]
Source: "..\BitDesk\bin\Release\BitDesk.exe"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\BitDesk.exe.config"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\BitDesk.pdb"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\FluentWPF.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\LiveCharts.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\LiveCharts.pdb"; DestDir: "{app}" 
Source: "..\BitDesk\bin\Release\LiveCharts.Wpf.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\LiveCharts.Wpf.pdb"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\LiveCharts.Wpf.xml"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\LiveCharts.xml"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Release\Newtonsoft.Json.xml"; DestDir: "{app}"

;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\BitDes"; Filename: "{app}\BitDes.exe"

[Languages]
Name: en; MessagesFile: "compiler:Default.isl"
Name: ja; MessagesFile: "compiler:Languages\Japanese.isl"