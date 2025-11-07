using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace StatusKeeperTerminalApp.Services;

public class MouseMovementService : IMouseMovementService
{
    private readonly ILogger<MouseMovementService> _logger;
    private readonly IGlobalStateService _globalState;
    private readonly Random _random = new Random();
    private CancellationTokenSource? _cancellationTokenSource;
    private DateTime _lastBreakTime = DateTime.MinValue;

    public MouseMovementService(ILogger<MouseMovementService> logger, IGlobalStateService globalState)
    {
        _logger = logger;
        _globalState = globalState;
    }

    public async Task StartAsync(MouseMovementConfig config, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _cancellationTokenSource.Token;
        
        _globalState.IsServiceRunning = true;
        _globalState.AddLog("Mouse Movement Service gestartet");
        
        if (config.DebugMode)
        {
            _globalState.AddLog("DEBUG-MODUS aktiviert - Detailliertes Logging aktiviert");
            _logger.LogInformation("DEBUG-MODUS aktiviert - Detailliertes Logging aktiviert");
            
            // Test-Bewegung ausführen
            _globalState.AddLog("Führe Test-Bewegung aus...");
            TestMouseMovement(config);
        }
        
        _logger.LogInformation("Mouse Movement Service gestartet");
        _logger.LogInformation($"Arbeitszeit: {config.WorkStartTime:hh\\:mm} (±{config.WorkStartVarianceMinutes}min) bis {config.WorkEndTime?.ToString(@"hh\:mm") ?? "kein Ende"} (±{config.WorkEndVarianceMinutes}min)");
        _logger.LogInformation($"Mittagspause: {config.LunchBreakStart:hh\\:mm} - {config.LunchBreakEnd:hh\\:mm}");
        _logger.LogInformation($"Pauseneinstellungen: {config.BreakProbabilityPercent}% Wahrscheinlichkeit, {config.MinBreakMinutes}-{config.MaxBreakMinutes} Minuten Dauer");

        var actualWorkEnd = CalculateActualWorkEnd(config);
        _globalState.AddLog($"Heutiges Arbeitsende: {actualWorkEnd:hh\\:mm}");
        _logger.LogInformation($"Heutiges Arbeitsende: {actualWorkEnd:hh\\:mm}");

        var lunchBreakTaken = false;
        var movementCounter = 0;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var now = DateTime.Now.TimeOfDay;

                // Prüfen ob Arbeitsende erreicht
                if (config.WorkEndTime.HasValue && now >= actualWorkEnd)
                {
                    _globalState.AddLog("Arbeitsende erreicht. Service wird beendet.");
                    _logger.LogInformation("Arbeitsende erreicht. Service wird beendet.");
                    break;
                }

                // Prüfen ob Mittagspause
                if (!lunchBreakTaken && IsInLunchBreakWindow(now, config))
                {
                    var lunchDuration = _random.Next(config.MinLunchBreakMinutes, config.MaxLunchBreakMinutes + 1);
                    var pauseStartMessage = $"Mittagspause startet um {DateTime.Now:HH:mm:ss} - Dauer: {lunchDuration} Minuten";
                    
                    _globalState.AddLog(pauseStartMessage);
                    _logger.LogInformation(pauseStartMessage);
                    
                    if (config.DebugMode)
                    {
                        var debugMessage = $"DEBUG: Mittagspause aktiviert - Start: {DateTime.Now:HH:mm:ss}, Ende geplant: {DateTime.Now.AddMinutes(lunchDuration):HH:mm:ss}";
                        _globalState.AddLog(debugMessage);
                        _logger.LogInformation(debugMessage);
                    }
                    
                    await Task.Delay(TimeSpan.FromMinutes(lunchDuration), token);
                    lunchBreakTaken = true;
                    
                    var pauseEndMessage = $"Mittagspause beendet um {DateTime.Now:HH:mm:ss}";
                    _globalState.AddLog(pauseEndMessage);
                    _logger.LogInformation(pauseEndMessage);
                    
                    if (config.DebugMode)
                    {
                        var debugEndMessage = $"DEBUG: Mittagspause deaktiviert um {DateTime.Now:HH:mm:ss}";
                        _globalState.AddLog(debugEndMessage);
                        _logger.LogInformation(debugEndMessage);
                    }
                    
                    continue;
                }

                // Normale Mausbewegung
                movementCounter++;
                try
                {
                    MoveMouse(config);
                    _logger.LogDebug($"Maus bewegt (#{movementCounter})");
                    
                    if (config.DebugMode)
                    {
                        if (!GetCursorPos(out POINT currentPos))
                        {
                            var debugMessage = $"DEBUG: Mausbewegung #{movementCounter} um {DateTime.Now:HH:mm:ss} - Position konnte nicht ermittelt werden";
                            _globalState.AddLog(debugMessage);
                            _logger.LogInformation(debugMessage);
                        }
                        else
                        {
                            var debugMessage = $"DEBUG: Mausbewegung #{movementCounter} um {DateTime.Now:HH:mm:ss} - Position: ({currentPos.X}, {currentPos.Y})";
                            _globalState.AddLog(debugMessage);
                            _logger.LogInformation(debugMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _globalState.AddLog($"Fehler beim Bewegen der Maus: {ex.Message}");
                    _logger.LogError(ex, "Fehler beim Bewegen der Maus");
                }

                // Prüfung auf zufällige Pause mit verbesserter Logik
                if (ShouldTakeBreak(config))
                {
                    var breakDuration = _random.Next(config.MinBreakMinutes, config.MaxBreakMinutes + 1);
                    var pauseStartMessage = $"Kurze Pause: {breakDuration} Minuten (nach {movementCounter} Bewegungen) - Start: {DateTime.Now:HH:mm:ss}";
                    
                    _globalState.AddLog(pauseStartMessage);
                    _logger.LogInformation(pauseStartMessage);
                    
                    if (config.DebugMode)
                    {
                        var debugMessage = $"DEBUG: Pause aktiviert - Start: {DateTime.Now:HH:mm:ss}, Ende geplant: {DateTime.Now.AddMinutes(breakDuration):HH:mm:ss}, Dauer: {breakDuration} min";
                        _globalState.AddLog(debugMessage);
                        _logger.LogInformation(debugMessage);
                    }
                    
                    _lastBreakTime = DateTime.Now;
                    await Task.Delay(TimeSpan.FromMinutes(breakDuration), token);
                    movementCounter = 0; // Counter zurücksetzen nach Pause
                    
                    var pauseEndMessage = $"Pause beendet um {DateTime.Now:HH:mm:ss}";
                    _globalState.AddLog(pauseEndMessage);
                    _logger.LogInformation(pauseEndMessage);
                    
                    if (config.DebugMode)
                    {
                        var debugEndMessage = $"DEBUG: Pause deaktiviert um {DateTime.Now:HH:mm:ss}";
                        _globalState.AddLog(debugEndMessage);
                        _logger.LogInformation(debugEndMessage);
                    }
                }
                else
                {
                    // Normales Intervall mit Varianz
                    var interval = _random.Next(config.MinIntervalSeconds, config.MaxIntervalSeconds + 1);
                    
                    if (config.DebugMode)
                    {
                        var debugMessage = $"DEBUG: Warte {interval} Sekunden bis zur nächsten Bewegung";
                        _logger.LogDebug(debugMessage);
                    }
                    
                    await Task.Delay(TimeSpan.FromSeconds(interval), token);
                }
            }
        }
        finally
        {
            _globalState.IsServiceRunning = false;
            _globalState.AddLog("Mouse Movement Service beendet");
            _logger.LogInformation("Mouse Movement Service beendet");
        }
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _globalState.AddLog("Mouse Movement Service Stop angefordert");
        _logger.LogInformation("Mouse Movement Service Stop angefordert");
    }

    private TimeSpan CalculateActualWorkEnd(MouseMovementConfig config)
    {
        if (!config.WorkEndTime.HasValue)
            return TimeSpan.MaxValue;

        var variance = _random.Next(-config.WorkEndVarianceMinutes, config.WorkEndVarianceMinutes + 1);
        var actualEnd = config.WorkEndTime.Value.Add(TimeSpan.FromMinutes(variance));

        // Sicherstellen dass nicht vor Mitternacht
        if (actualEnd < TimeSpan.Zero)
            actualEnd = TimeSpan.Zero;

        return actualEnd;
    }

    private bool IsInLunchBreakWindow(TimeSpan now, MouseMovementConfig config)
    {
        return now >= config.LunchBreakStart && now <= config.LunchBreakEnd;
    }

    private bool ShouldTakeBreak(MouseMovementConfig config)
    {
        // Mindestens 45 Minuten seit der letzten Pause warten (realistischer)
        var timeSinceLastBreak = DateTime.Now - _lastBreakTime;
        if (timeSinceLastBreak.TotalMinutes < 45)
        {
            if (config.DebugMode)
            {
                var debugMessage = $"DEBUG: Keine Pause - erst {timeSinceLastBreak.TotalMinutes:F1} Min seit letzter Pause (benötigt: 45 Min)";
                _logger.LogDebug(debugMessage);
            }
            return false;
        }

        // Reduzierte Wahrscheinlichkeit für realistischere Pausen
        // Ursprüngliche Wahrscheinlichkeit wird durch 3 geteilt für weniger häufige Pausen
        var adjustedProbability = Math.Max(1, config.BreakProbabilityPercent / 3);
        var randomValue = _random.Next(100);
        var shouldBreak = randomValue < adjustedProbability;
        
        if (config.DebugMode)
        {
            var debugMessage = $"DEBUG: Pausencheck - Zufallswert: {randomValue}/100, Schwellenwert: {adjustedProbability}% → Pause: {(shouldBreak ? "JA" : "NEIN")}";
            _logger.LogDebug(debugMessage);
        }
        
        if (shouldBreak)
        {
            _logger.LogDebug($"Pause ausgelöst mit {adjustedProbability}% Wahrscheinlichkeit");
        }
        
        return shouldBreak;
    }

    private void TestMouseMovement(MouseMovementConfig config)
    {
        try
        {
            _logger.LogInformation("Starte Test-Bewegung...");
            _globalState.AddLog("Starte Test-Bewegung...");

            // Aktuelle Position holen
            if (!GetCursorPos(out POINT startPos))
            {
                _globalState.AddLog("FEHLER: Konnte Start-Position nicht ermitteln");
                return;
            }

            _globalState.AddLog($"Start-Position: ({startPos.X}, {startPos.Y})");

            // Große, sichtbare Bewegung (50 Pixel nach rechts)
            var testX = startPos.X + 50;
            var testY = startPos.Y;

            _globalState.AddLog($"Bewege zu: ({testX}, {testY})");
            SetCursorPosPlatform(testX, testY, config);

            // Kurz warten
            System.Threading.Thread.Sleep(1000);

            // Position zurück bewegen
            _globalState.AddLog($"Bewege zurück zu: ({startPos.X}, {startPos.Y})");
            SetCursorPosPlatform(startPos.X, startPos.Y, config);

            _globalState.AddLog("Test-Bewegung abgeschlossen");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Fehler bei Test-Bewegung: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            _globalState.AddLog(errorMsg);
        }
    }

    private void MoveMouse(MouseMovementConfig config)
    {
        // Aktuelle Position holen
        if (!GetCursorPos(out POINT currentPos))
        {
            _logger.LogWarning("Konnte aktuelle Mausposition nicht ermitteln");
            return;
        }

        // Zufällige kleine Bewegung
        var deltaX = _random.Next(-config.MaxMovementDistance, config.MaxMovementDistance + 1);
        var deltaY = _random.Next(-config.MaxMovementDistance, config.MaxMovementDistance + 1);

        // Sicherstellen dass mindestens minimale Bewegung
        if (Math.Abs(deltaX) < config.MinMovementDistance)
            deltaX = config.MinMovementDistance * (deltaX >= 0 ? 1 : -1);
        if (Math.Abs(deltaY) < config.MinMovementDistance)
            deltaY = config.MinMovementDistance * (deltaY >= 0 ? 1 : -1);

        var newX = currentPos.X + deltaX;
        var newY = currentPos.Y + deltaY;

        if (config.DebugMode)
        {
            var debugMessage = $"DEBUG: Mausbewegung - Von ({currentPos.X}, {currentPos.Y}) zu ({newX}, {newY}), Delta: ({deltaX:+#;-#;0}, {deltaY:+#;-#;0})";
            _logger.LogDebug(debugMessage);
        }

        SetCursorPosPlatform(newX, newY, config);
        
        // Verifikation: Position nach der Bewegung prüfen
        if (config.DebugMode)
        {
            System.Threading.Thread.Sleep(10); // Kurz warten für die Bewegung
            if (GetCursorPos(out POINT verifyPos))
            {
                var actualDeltaX = verifyPos.X - currentPos.X;
                var actualDeltaY = verifyPos.Y - currentPos.Y;
                var verifyMessage = $"DEBUG: Verifikation - Tatsächliche Position: ({verifyPos.X}, {verifyPos.Y}), Tatsächliches Delta: ({actualDeltaX:+#;-#;0}, {actualDeltaY:+#;-#;0})";
                _logger.LogDebug(verifyMessage);
                _globalState.AddLog(verifyMessage);
                
                if (verifyPos.X == currentPos.X && verifyPos.Y == currentPos.Y)
                {
                    var warningMessage = "WARNUNG: Maus hat sich nicht bewegt!";
                    _logger.LogWarning(warningMessage);
                    _globalState.AddLog(warningMessage);
                }
            }
        }
    }

    #region Platform-specific Mouse Movement

    private bool GetCursorPos(out POINT lpPoint)
    {
        if (OperatingSystem.IsWindows())
        {
            return GetCursorPosWindows(out lpPoint);
        }
        else if (OperatingSystem.IsMacOS())
        {
            return GetCursorPosMacOS(out lpPoint);
        }
        else if (OperatingSystem.IsLinux())
        {
            _logger.LogWarning("Linux wird aktuell nicht unterstützt");
            lpPoint = new POINT();
            return false;
        }
        else
        {
            _logger.LogWarning("Betriebssystem wird nicht unterstützt");
            lpPoint = new POINT();
            return false;
        }
    }

    private void SetCursorPosPlatform(int x, int y, MouseMovementConfig config)
    {
        if (OperatingSystem.IsWindows())
        {
            var success = SetCursorPos(x, y);
            if (config.DebugMode)
            {
                var debugMessage = $"DEBUG: SetCursorPos({x}, {y}) → Erfolg: {success}";
                _logger.LogDebug(debugMessage);
                _globalState.AddLog(debugMessage);
            }
            
            // Wenn SetCursorPos fehlschlägt, versuche alternative Methode
            if (!success)
            {
                var errorMessage = "SetCursorPos fehlgeschlagen, versuche alternative Methode";
                _logger.LogWarning(errorMessage);
                _globalState.AddLog(errorMessage);
                
                var alternativeSuccess = SetCursorPosAlternative(x, y);
                if (config.DebugMode)
                {
                    var altDebugMessage = $"DEBUG: Alternative mouse_event({x}, {y}) → Erfolg: {alternativeSuccess}";
                    _logger.LogDebug(altDebugMessage);
                    _globalState.AddLog(altDebugMessage);
                }
            }
        }
        else if (OperatingSystem.IsMacOS())
        {
            SetCursorPosMacOS(x, y);
        }
        else if (OperatingSystem.IsLinux())
        {
            _logger.LogWarning("Linux wird aktuell nicht unterstützt");
        }
        else
        {
            _logger.LogWarning("Betriebssystem wird nicht unterstützt");
        }
    }

    #endregion

    #region Windows API

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
    private static extern bool GetCursorPosWindows(out POINT lpPoint);

    // Alternative Windows API für Mausbewegung
    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    private const int SM_CXSCREEN = 0; // Bildschirmbreite
    private const int SM_CYSCREEN = 1; // Bildschirmhöhe

    // Alternative Methode mit mouse_event
    private bool SetCursorPosAlternative(int x, int y)
    {
        try
        {
            // Bildschirmauflösung holen
            var screenWidth = GetSystemMetrics(SM_CXSCREEN);
            var screenHeight = GetSystemMetrics(SM_CYSCREEN);
            
            // Zu absolute Koordinaten konvertieren (0-65535)
            uint absoluteX = (uint)((x * 65535) / screenWidth);
            uint absoluteY = (uint)((y * 65535) / screenHeight);
            
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, absoluteX, absoluteY, 0, UIntPtr.Zero);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler bei alternativer Mausbewegung");
            return false;
        }
    }

    #endregion

    #region macOS API

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static extern void CGWarpMouseCursorPosition(CGPoint newCursorPosition);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static extern CGPoint CGEventGetLocation(IntPtr eventRef);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static extern IntPtr CGEventCreate(IntPtr source);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFRelease(IntPtr cf);

    [StructLayout(LayoutKind.Sequential)]
    private struct CGPoint
    {
        public double X;
        public double Y;
    }

    private bool GetCursorPosMacOS(out POINT lpPoint)
    {
        try
        {
            IntPtr eventRef = CGEventCreate(IntPtr.Zero);
            if (eventRef == IntPtr.Zero)
            {
                lpPoint = new POINT();
                return false;
            }

            CGPoint location = CGEventGetLocation(eventRef);
            CFRelease(eventRef);

            lpPoint = new POINT
            {
                X = (int)location.X,
                Y = (int)location.Y
            };
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Abrufen der Mausposition auf macOS");
            lpPoint = new POINT();
            return false;
        }
    }

    private void SetCursorPosMacOS(int x, int y)
    {
        try
        {
            CGWarpMouseCursorPosition(new CGPoint { X = x, Y = y });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Setzen der Mausposition auf macOS");
        }
    }

    #endregion

    #region Shared Structures

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    #endregion
}
