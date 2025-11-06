# Status Keeper 2

Eine Terminal-basierte Anwendung, um den Computer-Status aktiv zu halten durch automatische Mausbewegungen.

## 🎯 Überblick

Status Keeper 2 ist eine intelligente Cross-Platform-Anwendung, die durch automatisierte Mausbewegungen verhindert, dass Ihr Computer in den Ruhezustand wechselt oder als inaktiv markiert wird. Die Anwendung simuliert natürliches Benutzerverhalten mit konfigurierbaren Bewegungsmustern, Pausen und Arbeitszeiten.

### Hauptmerkmale

- 🖱️ **Intelligente Mausbewegung** mit natürlichen, zufälligen Bewegungsmustern
- ⏱️ **Zeitgesteuertes Management** mit konfigurierbaren Arbeitszeiten und Pausen
- 📋 **Unbegrenzte Profile** für verschiedene Arbeitsszenarien
- 🖥️ **Moderne Terminal-GUI** mit Echtzeit-Statusanzeige
- 🌍 **Cross-Platform** Unterstützung für Windows und macOS
- 🔧 **Vollständig konfigurierbar** ohne Code-Änderungen

## ✨ Features

### 🖱️ Intelligente Mausbewegung

- **Konfigurierbare Bewegungsdistanz**
  - Minimale und maximale Bewegungsdistanz in Pixeln
  - Zufällige Bewegungen für natürliches Verhalten
  - Automatische Positionsermittlung und sanfte Bewegungen

- **Plattform-Unterstützung**
  - ✅ Windows: Native Win32 API (user32.dll)
  - ✅ macOS: CoreGraphics Framework
  - 🔄 Linux: Vorbereitet (in Entwicklung)

### ⏱️ Zeitgesteuerte Funktionen

#### Flexible Intervalle
- Minimales und maximales Intervall zwischen Bewegungen
- Zufällige Intervalle für natürliches Verhalten
- Sekunden-genaue Konfiguration (z.B. 30-120 Sekunden)

#### Arbeitszeit-Management
- Konfigurierbarer **Arbeitsbeginn** im HH:MM Format
- **Variabler Arbeitsbeginn** (±X Minuten für natürliche Variation)
- Konfigurierbares **Arbeitsende** im HH:MM Format
- **Variables Arbeitsende** (±X Minuten)
- Automatisches Beenden des Services bei Arbeitsende

#### Automatische Mittagspause
- Konfigurierbares Zeitfenster (z.B. 12:00-14:00 Uhr)
- Zufällige Pausendauer zwischen Min und Max
- Automatische Erkennung und Durchführung
- Nur einmalige Ausführung pro Tag

#### Kurze Pausen
- Konfigurierbare Wahrscheinlichkeit in Prozent
- Zufällige Pausendauer (z.B. 5-10 Minuten)
- Simuliert natürliche Kaffeepausen oder kurze Unterbrechungen

### 📋 Profil-Management

- **Unbegrenzte Anzahl an Profilen** erstellen
- Jedes Profil mit **eigenem Namen** und vollständiger Konfiguration
- **Schnelles Wechseln** zwischen Profilen
- **Aktives Profil** wird automatisch beim Start verwendet

**Profil-Operationen:**
- ➕ Neue Profile erstellen
- ✏️ Bestehende Profile bearbeiten
- 🗑️ Profile löschen
- ✅ Profil als aktiv setzen

### 🖥️ Terminal-GUI

#### Hauptansicht
- 📊 Anzeige des aktuell aktiven Profils
- 🚦 Service Status mit Farb-Indikator (Läuft ✓ / Gestoppt)
- ▶️ Start/Stop Button für Service-Steuerung
- 📝 Echtzeit Aktivitäts-Log mit automatischem Scrolling
- 🔧 Schneller Zugriff auf Konfiguration
- ℹ️ Info-Dialog mit Versions- und Feature-Übersicht

#### Konfigurations-Ansicht
Übersichtlich gruppierte Einstellungen:

- **Mausbewegung**
  - Min/Max Distanz in Pixeln

- **Zeitintervalle**
  - Min/Max Intervall in Sekunden

- **Kurze Pausen**
  - Min/Max Dauer in Minuten
  - Wahrscheinlichkeit in Prozent

- **Mittagspause**
  - Start- und Endzeit (HH:MM)
  - Min/Max Dauer in Minuten

- **Arbeitszeit**
  - Startzeit mit Varianz
  - Endzeit mit Varianz

### 🔧 Technische Features

- **.NET 9.0**
  - Modernste .NET Plattform
  - Cross-Platform Unterstützung
  - Dependency Injection Pattern
  - Strukturiertes Logging (Microsoft.Extensions.Logging)

- **Terminal.Gui Framework (v1.19.0)**
  - Native Terminal-Benutzeroberfläche
  - Vollständige Tastatur-Navigation
  - Responsives Layout
  - Cross-Platform kompatibel

- **JSON-Konfiguration**
  - Persistente Speicherung in `appsettings.json`
  - Einfache manuelle Bearbeitung möglich
  - Strukturierte Datenhaltung

- **Services-Architektur**
  - `ConfigurationService`: Profil- und Einstellungsverwaltung
  - `MouseMovementService`: Mausbewegungslogik und Plattform-Abstraktion
  - `GlobalStateService`: Anwendungszustand und Log-Management
  - Saubere Dependency Injection über Microsoft.Extensions.DependencyInjection

### 📦 Standard-Konfiguration

Bei Erstellung eines neuen Profils werden folgende **Standardwerte** verwendet:

```
Mausbewegung:        1-3 Pixel
Bewegungsintervall:  30-120 Sekunden
Kurze Pausen:        5-10 Minuten (10% Wahrscheinlichkeit)
Mittagspause:        12:00-14:00 Uhr, 25-35 Minuten Dauer
Arbeitszeit:         08:00 Uhr (±15 Min) bis 18:00 Uhr (±30 Min)
```

Diese Werte können für jedes Profil individuell angepasst werden.

## 🚀 Schnellstart

1. **Download** des entsprechenden Release für Ihre Plattform
2. **Entpacken** des ZIP-Archives
3. **Ausführen** der Anwendung:
   - Windows: `StatusKeeperTerminalApp.exe`
   - macOS/Linux: `./StatusKeeperTerminalApp`
4. **Konfiguration** über die GUI anpassen (optional)
5. **Service starten** mit dem Start-Button

## 💻 Entwicklung

### Debugging

```bash
cd StatusKeeperTerminalApp
dotnet run --launch-profile "StatusKeeperTerminalApp"
# oder einfach
dotnet run
```

### Einzelne Plattform bauen

**Windows (x64)**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/windows
```

**Apple Intel**
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/macos-x64
```

**Apple Silicon**
```bash
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o ./publish/macos-arm64
```

## Release-Prozess

### 1. Releases für alle Plattformen bauen

Das Build-Script erstellt automatisch Releases für alle unterstützten Plattformen (Windows x64/x86/ARM64, macOS x64/ARM64) als ZIP-Archive.

#### PowerShell (Windows):

```powershell
# Mit Standard-Version 2.1.0
.\build-releases.ps1

# Mit spezifischer Version
.\build-releases.ps1 -Version "2.1.0"
```

#### Bash (Linux/macOS):

```bash
# Skript ausführbar machen
chmod +x build-releases.sh

# Mit Standard-Version 1.0.0
./build-releases.sh

# Mit spezifischer Version
./build-releases.sh 2.1.0
```

Die fertigen Releases werden im Ordner `releases/` als ZIP-Dateien abgelegt.

### 2. Release auf GitHub hochladen

Das Upload-Script verwendet die GitHub CLI, um automatisch ein Release mit allen ZIP-Dateien zu erstellen.

#### Voraussetzungen:

1. **GitHub CLI installieren:**
   - Windows: `winget install GitHub.cli`
   - macOS: `brew install gh`
   - Linux: [Installationsanleitung](https://github.com/cli/cli#installation)

2. **GitHub CLI authentifizieren:**
   ```bash
   gh auth login
   ```

#### PowerShell (Windows):

```powershell
# Standard-Release hochladen
.\upload-release.ps1 -Version "2.1.0"

# Als Draft (Entwurf) hochladen
.\upload-release.ps1 -Version "2.1.0" -Draft

# Als Pre-Release markieren
.\upload-release.ps1 -Version "2.1.0" -Prerelease

# Mit eigenen Release-Notes
.\upload-release.ps1 -Version "2.1.0" -ReleaseNotes "Neue Features: XYZ, Bugfixes: ABC"

# Eigener Release-Name
.\upload-release.ps1 -Version "2.1.0" -ReleaseName "Status Keeper Beta v2.1.0"
```

#### Bash (Linux/macOS):

```bash
# Skript ausführbar machen
chmod +x upload-release.sh

# Standard-Release hochladen
./upload-release.sh 2.1.0

# Als Draft hochladen
DRAFT=true ./upload-release.sh 2.1.0

# Als Pre-Release markieren
PRERELEASE=true ./upload-release.sh 2.1.0

# Mit eigenen Release-Notes
./upload-release.sh 2.1.0 "Neue Features: XYZ, Bugfixes: ABC"
```

### Kompletter Release-Workflow:

```powershell
# Windows - Build und Upload in einem Durchgang
.\build-releases.ps1 -Version "2.1.0"
.\upload-release.ps1 -Version "2.1.0" -ReleaseNotes "Release v2.1.0 mit neuen Features"
```

```bash
# Linux/macOS - Build und Upload in einem Durchgang
./build-releases.sh 2.1.0
./upload-release.sh 2.1.0 "Release v2.1.0 mit neuen Features"
```

## 🌍 Unterstützte Plattformen

- ✅ Windows x64
- ✅ Windows x86
- ✅ Windows ARM64
- ✅ macOS x64 (Intel)
- ✅ macOS ARM64 (Apple Silicon)
- 🔄 Linux x64 (auskommentiert, bei Bedarf aktivierbar)
- 🔄 Linux ARM64 (auskommentiert, bei Bedarf aktivierbar)

## 🛠️ Technologie-Stack

- [.NET 9.0](https://dotnet.microsoft.com/)
- [Terminal.Gui 1.19.0](https://github.com/gui-cs/Terminal.Gui) - Cross-platform Terminal UI
- [Microsoft.Extensions.DependencyInjection 9.0.10](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/)
- [Microsoft.Extensions.Hosting 9.0.10](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/)

## 🤝 Mitwirken

Beiträge sind willkommen! Wenn Sie Fehler finden oder neue Features vorschlagen möchten:

1. Erstellen Sie ein [Issue](https://github.com/marcel-braun/StatusKeeper2/issues)
2. Forken Sie das Repository
3. Erstellen Sie einen Feature-Branch (`git checkout -b feature/AmazingFeature`)
4. Committen Sie Ihre Änderungen (`git commit -m 'Add some AmazingFeature'`)
5. Pushen Sie zum Branch (`git push origin feature/AmazingFeature`)
6. Öffnen Sie einen Pull Request

## 📋 Changelog

Alle wichtigen Änderungen werden in der [CHANGELOG.md](CHANGELOG.md) dokumentiert.

## 📄 Lizenz

Copyright © 2024-2025 Marcel Braun

Dieses Projekt ist für den persönlichen und kommerziellen Gebrauch frei verfügbar.

### Haftungsausschluss

Diese Software wird "wie besehen" ohne jegliche ausdrückliche oder stillschweigende Gewährleistung zur Verfügung gestellt. Die Verwendung erfolgt auf eigenes Risiko. Der Autor übernimmt keine Haftung für Schäden, die durch die Verwendung dieser Software entstehen.

**Hinweis:** Bitte beachten Sie die Richtlinien Ihres Arbeitgebers oder Ihrer Organisation bezüglich der Verwendung von Automatisierungstools. Die Verwendung dieser Software in Umgebungen, in denen dies gegen Richtlinien verstößt, erfolgt auf eigene Verantwortung.

## 📞 Kontakt

Marcel Braun - [@marcel-braun](https://github.com/marcel-braun)

Projekt Link: [https://github.com/marcel-braun/StatusKeeper2](https://github.com/marcel-braun/StatusKeeper2)

## 🙏 Danksagungen

- [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui) für das hervorragende Terminal UI Framework
- [.NET](https://dotnet.microsoft.com/) für die moderne Cross-Platform Entwicklung
- Alle Contributors und User, die Feedback geben

---

**⭐ Wenn Ihnen dieses Projekt gefällt, geben Sie ihm einen Stern auf GitHub!**