; -- BitDeskInstaller.iss --
; http://www.jrsoftware.org/isinfo.php

[Setup]
AppName=BitDesk
AppVersion=0.6.1.0  
VersionInfoVersion=0.6.1.0  
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
Source: "..\BitDesk\bin\Debug\BitDesk.exe"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\BitDesk.exe.config"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\BitDesk.pdb"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\LiveCharts.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\LiveCharts.pdb"; DestDir: "{app}" 
Source: "..\BitDesk\bin\Debug\LiveCharts.Wpf.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\LiveCharts.Wpf.pdb"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\LiveCharts.Wpf.xml"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\LiveCharts.xml"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "..\BitDesk\bin\Debug\Newtonsoft.Json.xml"; DestDir: "{app}"

;Source: "Readme.txt"; DestDir: "{app}"; Flags: isreadme

[Icons]
Name: "{group}\BitDesk"; Filename: "{app}\BitDesk.exe"

[Languages]
Name: en; MessagesFile: "compiler:Default.isl"
Name: ja; MessagesFile: "compiler:Languages\Japanese.isl"