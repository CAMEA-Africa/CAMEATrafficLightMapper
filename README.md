# CAMEA Traffic Light Mapper

Lightweight feeder service that reads WIM traffic records from a local MySQL database and sends them to the CAMEA Traffic MapperService.

```
REMOTE SERVER                         CAMEA.APP
─────────────                         ─────────
MySQL (local)
  │
LightMapper ─── POST /api/feed/records ──→ MapperService
  │ web UI on :5166                          ↓
  │ settings + status                     MongoDB
                                             ↓
                                       EvaluationService → WebApi → Frontend
```

## Installation

1. Download **`CAMEATrafficLightMapperSetup.exe`** from [Releases](https://github.com/CAMEA-Africa/CAMEATrafficLightMapper/releases)
2. Run the installer, choose install folder
3. Run `CAMEATrafficLightMapper.exe`
4. Open `http://localhost:5166/settings.html`
5. Enter MySQL connection details and feed target URL → Save

## Web UI

### Settings (`http://localhost:5166/settings.html`)

Configure MySQL connection and feed target. The connection is tested before saving.

### Status (`http://localhost:5166/status.html`)

Live view of sync state: last processed MySQL ID, total records sent, last sync time, and errors.

## How It Works

1. **Poll**: Every N seconds, reads new records from MySQL (`wims_detections` table) starting from the last processed ID
2. **Map**: Converts each `CAMEAWimMysqlRecord` to a `TrafficRecord`
3. **Send**: POSTs the batch to the MapperService's `/api/feed/records` endpoint
4. **Track**: Saves the last processed MySQL ID to a local JSON file (`sync-state.json`)

## Building the Installer

Requirements: .NET 8 SDK, [Inno Setup 6](https://jrsoftware.org/isdl.php)

```
cd installer
build-installer.bat
```

Output: `installer\output\CAMEATrafficLightMapperSetup.exe`

## Project Structure

```
CAMEATrafficLightMapper/
├── CAMEATrafficLightMapper/
│   ├── Program.cs                  # ASP.NET host + Windows Service
│   ├── Worker.cs                   # BackgroundService — poll loop
│   ├── FeedClient.cs               # HttpClient wrapper — POST batches
│   ├── SettingsStore.cs            # Encrypted credential persistence
│   ├── SyncStateStore.cs           # Sync state persistence (JSON file)
│   ├── appsettings.json            # Default configuration
│   ├── Controllers/
│   │   ├── SettingsController.cs   # GET/POST /api/settings
│   │   └── StatusController.cs     # GET /api/status
│   ├── wwwroot/
│   │   ├── settings.html           # MySQL + feed configuration UI
│   │   └── status.html             # Sync state dashboard
│   ├── Models/
│   │   └── TrafficRecord.cs
│   └── MySql/
│       ├── CAMEAWimMysqlRecord.cs
│       ├── WimMysqlReader.cs
│       └── CAMEAWimMysqlRecordToTrafficRecordMapper.cs
├── installer/
│   ├── CAMEATrafficLightMapperInstaller.iss
│   └── build-installer.bat
└── CAMEATrafficLightMapper.sln
```
