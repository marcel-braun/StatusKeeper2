using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace StatusKeeperTerminalApp.Services;

public class MouseMovementService : IMouseMovementService
{
    private readonly ILogger<MouseMovementService> _logger;
    private readonly Random _random = new Random();

    public MouseMovementService(ILogger<MouseMovementService> logger)
    {
        _logger = logger;
    }

    public async Task StartAsync(MouseMovementConfig config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mouse Movement Service gestartet");
        _logger.LogInformation($"Arbeitszeit: {config.WorkStartTime:hh\\:mm} (±{config.WorkStartVarianceMinutes}min) bis {config.WorkEndTime?.ToString(@"hh\:mm") ?? "kein Ende"} (±{config.WorkEndVarianceMinutes}min)");
        _logger.LogInformation($"Mittagspause: {config.LunchBreakStart:hh\\:mm} - {config.LunchBreakEnd:hh\\:mm}");

        var actualWorkEnd = CalculateActualWorkEnd(config);
        _logger.LogInformation($"Heutiges Arbeitsende: {actualWorkEnd:hh\\:mm}");

        var lunchBreakTaken = false;

        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.Now.TimeOfDay;

            // Prüfen ob Arbeitsende erreicht
            if (config.WorkEndTime.HasValue && now >= actualWorkEnd)
            {
                _logger.LogInformation("Arbeitsende erreicht. Service wird beendet.");
                break;
            }

            // Prüfen ob Mittagspause
            if (!lunchBreakTaken && IsInLunchBreakWindow(now, config))
            {
                var lunchDuration = _random.Next(config.MinLunchBreakMinutes, config.MaxLunchBreakMinutes + 1);
                _logger.LogInformation($"Mittagspause startet - Dauer: {lunchDuration} Minuten");
                await Task.Delay(TimeSpan.FromMinutes(lunchDuration), cancellationToken);
                lunchBreakTaken = true;
                _logger.LogInformation("Mittagspause beendet");
                continue;
            }

            // Normale Mausbewegung
            try
            {
                MoveMouse(config);
                _logger.LogDebug("Maus bewegt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Bewegen der Maus");
            }

            // Zufällige Pause?
            if (_random.Next(100) < config.BreakProbabilityPercent)
            {
                var breakDuration = _random.Next(config.MinBreakMinutes, config.MaxBreakMinutes + 1);
                _logger.LogInformation($"Kurze Pause: {breakDuration} Minuten");
                await Task.Delay(TimeSpan.FromMinutes(breakDuration), cancellationToken);
            }
            else
            {
                // Normales Intervall mit Varianz
                var interval = _random.Next(config.MinIntervalSeconds, config.MaxIntervalSeconds + 1);
                await Task.Delay(TimeSpan.FromSeconds(interval), cancellationToken);
            }
        }

        _logger.LogInformation("Mouse Movement Service beendet");
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

    SetCursorPosPlatform(newX, newY);
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

    private void SetCursorPosPlatform(int x, int y)
    {
        if (OperatingSystem.IsWindows())
        {
            SetCursorPos(x, y);
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
