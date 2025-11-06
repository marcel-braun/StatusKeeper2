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

        SetCursorPos(newX, newY);
    }

    #region Windows API

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    #endregion
}
