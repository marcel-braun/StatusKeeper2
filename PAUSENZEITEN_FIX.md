# Fix für Pausenzeiten-Problem

## Problem
Teams wurde zu häufig auf "Abwesend" gesetzt, weil die Pausenlogik unrealistisch häufige Pausen erzeugte.

## Ursache
1. **Zu hohe Pausenwahrscheinlichkeit**: 15% bei jeder Mausbewegung (alle 45-180 Sek)
2. **Fehlende Cooldown-Zeit**: Sofort nach einer Pause war wieder die volle Wahrscheinlichkeit aktiv
3. **Mathematisches Problem**: Bei 15% Wahrscheinlichkeit und ~112 Sek Durchschnittsintervall ergab sich alle 12-13 Minuten eine Pause

## Lösung
### Code-Änderungen in `MouseMovementService.cs`:

1. **Neue Cooldown-Logik**: 
   - Mindestens 45 Minuten zwischen Pausen
   - Tracking der letzten Pausenzeit

2. **Reduzierte Pausenwahrscheinlichkeit**:
   - Konfigurierte Wahrscheinlichkeit wird durch 3 geteilt
   - Beispiel: 9% wird zu 3% effektiver Wahrscheinlichkeit

3. **Verbessertes Logging**:
   - Anzahl Bewegungen seit letzter Pause
   - Grund für Pausenverweigerung
   - Bessere Übersicht über Pausenhäufigkeit

### Konfiguration-Änderungen:
- **Standard-Profil**: `BreakProbabilityPercent` von 10% auf 6%
- **Home Office-Profil**: `BreakProbabilityPercent` von 15% auf 9%

## Neue Pausenhäufigkeit (geschätzt)
- **Vorher**: Alle 12-15 Minuten eine Pause
- **Nachher**: Alle 2-3 Stunden eine Pause (realistischer)

## Berechnung der neuen Wahrscheinlichkeiten:
- Home Office: 9% ÷ 3 = 3% pro Zyklus, mit 45-Min-Cooldown
- Bei 112 Sek Durchschnittsintervall: ~125 Zyklen pro Stunde
- Effektive Pausenwahrscheinlichkeit: Etwa alle 2-3 Stunden

Diese Änderungen sollten das Problem mit zu häufigen Teams-Status-Änderungen erheblich reduzieren.