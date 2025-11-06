# Changelog

Alle wichtigen Änderungen an diesem Projekt werden in dieser Datei dokumentiert.

Das Format basiert auf [Keep a Changelog](https://keepachangelog.com/de/1.0.0/),
und dieses Projekt hält sich an [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.1.0] - 2025-11-06

### Hinzugefügt
- **Automatisierte Build-Scripts** für alle Plattformen
  - `build-releases.ps1` für Windows (PowerShell)
  - `build-releases.sh` für Linux/macOS (Bash)
  - Erstellt ZIP-Archive für alle Releases
  - Unterstützte Plattformen: Windows (x64/x86/ARM64), macOS (x64/ARM64)

- **GitHub Release Upload-Scripts**
  - `upload-release.ps1` für Windows (PowerShell)
  - `upload-release.sh` für Linux/macOS (Bash)
  - Automatisches Hochladen von Releases via GitHub CLI
  - Unterstützung für Draft- und Pre-Releases
  - Eigene Release-Notes konfigurierbar

- **Erweiterte Dokumentation**
  - Vollständige README mit Build- und Release-Anleitungen
  - Changelog für Feature-Tracking
  - Schritt-für-Schritt Anleitung für den Release-Prozess

## [2.0.0] - 2025-07-03

### Hauptfeatures

#### 🖱️ Intelligente Mausbewegung
- **Konfigurierbare Bewegungsdistanz**
  - Minimale und maximale Bewegungsdistanz in Pixeln einstellbar
  - Zufällige Bewegungen für natürliches Verhalten
  - Automatische Positionsermittlung

- **Plattform-Unterstützung**
  - Windows: Native Win32 API (user32.dll)
  - macOS: CoreGraphics Framework
  - Linux: Vorbereitet (aktuell nicht implementiert)

#### ⏱️ Zeitgesteuerte Funktionen
- **Flexible Intervalle**
  - Minimales und maximales Intervall zwischen Bewegungen
  - Zufällige Intervalle für natürliches Verhalten
  - Sekunden-genaue Konfiguration

- **Arbeitszeit-Management**
  - Konfigurierbarer Arbeitsbeginn (HH:MM Format)
  - Variabler Arbeitsbeginn (±X Minuten)
  - Konfigurierbares Arbeitsende (HH:MM Format)
  - Variables Arbeitsende (±X Minuten)
  - Automatisches Beenden bei Arbeitsende

- **Automatische Mittagspause**
  - Konfigurierbares Zeitfenster (Start/Ende)
  - Zufällige Pausendauer (Min/Max)
  - Automatische Erkennung und Durchführung

- **Kurze Pausen**
  - Konfigurierbare Wahrscheinlichkeit (%)
  - Zufällige Pausendauer (Min/Max)
  - Simuliert natürliches Pausenverhalten

#### 📋 Profil-Management
- **Multiple Profile**
  - Unbegrenzte Anzahl an Profilen
  - Eigene Namen für Profile
  - Jedes Profil mit vollständiger Konfiguration

- **Profil-Verwaltung**
  - Erstellen neuer Profile
  - Löschen bestehender Profile
  - Bearbeiten aller Einstellungen
  - Aktivieren des gewünschten Profils

#### 🖥️ Terminal-GUI
- **Hauptansicht**
  - Anzeige des aktiven Profils
  - Service Status (Läuft/Gestoppt) mit Farb-Indikator
  - Start/Stop Button
  - Echtzeit Aktivitäts-Log
  - Zugriff auf Konfiguration und Info

- **Konfigurations-Ansicht**
  - Übersichtliche Profil-Liste
  - Detaillierte Einstellungsfelder
  - Gruppierte Konfigurationsbereiche:
    - Mausbewegung
    - Zeitintervalle
    - Kurze Pausen
    - Mittagspause
    - Arbeitszeit
  - Speichern und Aktivieren von Profilen
  - Erstellen und Löschen von Profilen

- **Info-Ansicht**
  - Versions-Information
  - Feature-Übersicht
  - Copyright-Informationen

#### 🔧 Technische Features
- **.NET 9.0**
  - Moderne .NET Plattform
  - Cross-Platform Unterstützung
  - Dependency Injection
  - Strukturiertes Logging

- **Terminal.Gui Framework**
  - Native Terminal-Benutzeroberfläche
  - Plattform-übergreifend
  - Tastatur-Navigation
  - Professionelles Design

- **JSON-Konfiguration**
  - Persistente Speicherung in `appsettings.json`
  - Einfache Bearbeitung außerhalb der Anwendung
  - Strukturierte Datenhaltung

- **Logging**
  - Ausführliches Debug-Logging
  - Aktivitäts-Log in der GUI
  - Microsoft.Extensions.Logging Integration

- **Services-Architektur**
  - `ConfigurationService`: Profil- und Einstellungsverwaltung
  - `MouseMovementService`: Mausbewegungslogik
  - `GlobalStateService`: Anwendungszustand und Log
  - Saubere Dependency Injection

### Standard-Konfiguration
Bei Erstellung eines neuen Profils werden folgende Standardwerte verwendet:
- Mausbewegung: 1-3 Pixel
- Bewegungsintervall: 30-120 Sekunden
- Kurze Pausen: 5-10 Minuten (10% Wahrscheinlichkeit)
- Mittagspause: 12:00-14:00 Uhr (25-35 Minuten)
- Arbeitszeit: 08:00 Uhr (±15 Min) bis 18:00 Uhr (±30 Min)

### Plattformen
- Windows x64/x86/ARM64
- macOS x64 (Intel)
- macOS ARM64 (Apple Silicon)
- Linux x64/ARM64 (vorbereitet)

### Technologie-Stack
- .NET 9.0
- Terminal.Gui 1.19.0
- Microsoft.Extensions.DependencyInjection 9.0.10
- Microsoft.Extensions.Hosting 9.0.10

## [1.0.0] - Initial Release

### Hinzugefügt
- Basis-Implementierung der Mausbewegung
- Einfache Konfiguration
- Windows-Unterstützung

---

## Mitwirken

Fehler gefunden oder Feature-Wünsche? Bitte erstelle ein [Issue](https://github.com/marcel-braun/StatusKeeper2/issues) auf GitHub.

## Lizenz

Copyright © 2024-2025 Marcel Braun
