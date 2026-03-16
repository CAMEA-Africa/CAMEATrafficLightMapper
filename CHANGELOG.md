# Changelog

All notable changes to this project will be documented in this file.

## [0.2.0] — 2026-03-16

### Added
- Built-in web UI for configuration (`/settings.html`) and status monitoring (`/status.html`)
- Encrypted credential storage via ASP.NET Data Protection API
- Settings and status API endpoints (`/api/settings`, `/api/status`)
- Installer opens browser to settings page after install

### Changed
- Switched from Worker SDK to Web SDK (ASP.NET) to serve settings UI on port 5166
- Configuration via web UI instead of installer prompts
- Worker reads settings from encrypted store (supports runtime reconfiguration)

## [0.1.0] — 2026-03-16

### Changed
- Replaced Docker/WSL2 deployment with native Windows Service + Inno Setup installer
- Single self-contained executable — no .NET SDK or Docker required on target machine
- Service auto-starts on boot, auto-restarts on failure

### Added
- Inno Setup installer (`CAMEALightMapperSetup.exe`)
- `build-installer.bat` for building the installer from source
- Windows Service support via `Microsoft.Extensions.Hosting.WindowsServices`

### Removed
- Dockerfile, docker-compose.yml
- install-lightmapper.sh, install-lightmapper.ps1, install-lightmapper.bat

## [0.0.1] — 2026-03-13

### Added
- Initial release
- Reads WIM records from local MySQL (`wims_detections` table)
- Maps records to TrafficRecord format (same mapping as MapperService)
- POSTs batches to remote MapperService feed endpoint
- Tracks sync progress in local JSON file (`sync-state.json`)
