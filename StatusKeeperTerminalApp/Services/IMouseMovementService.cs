namespace StatusKeeperTerminalApp.Services;

public interface IMouseMovementService
{
    /// <summary>
    /// Startet den Mouse Movement Service mit den angegebenen Parametern
    /// </summary>
    Task StartAsync(MouseMovementConfig config, CancellationToken cancellationToken = default);
}

public class MouseMovementConfig
{
    /// <summary>
    /// Minimale Bewegungsdistanz in Pixeln (Standard: 1)
    /// </summary>
    public int MinMovementDistance { get; set; } = 1;

    /// <summary>
    /// Maximale Bewegungsdistanz in Pixeln (Standard: 3)
    /// </summary>
    public int MaxMovementDistance { get; set; } = 3;

    /// <summary>
    /// Minimales Intervall zwischen Mausbewegungen in Sekunden (Standard: 30)
    /// </summary>
    public int MinIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Maximales Intervall zwischen Mausbewegungen in Sekunden (Standard: 120)
    /// </summary>
    public int MaxIntervalSeconds { get; set; } = 120;

    /// <summary>
    /// Minimale Pausendauer in Minuten (Standard: 5)
    /// </summary>
    public int MinBreakMinutes { get; set; } = 5;

    /// <summary>
    /// Maximale Pausendauer in Minuten (Standard: 10)
    /// </summary>
    public int MaxBreakMinutes { get; set; } = 10;

    /// <summary>
    /// Wahrscheinlichkeit einer Pause in Prozent (Standard: 10)
    /// </summary>
    public int BreakProbabilityPercent { get; set; } = 10;

    /// <summary>
    /// Start der Mittagspause (Standard: 12:00)
    /// </summary>
    public TimeSpan LunchBreakStart { get; set; } = new TimeSpan(12, 0, 0);

    /// <summary>
    /// Ende der Mittagspause (Standard: 14:00)
    /// </summary>
    public TimeSpan LunchBreakEnd { get; set; } = new TimeSpan(14, 0, 0);

    /// <summary>
    /// Minimale Mittagspausendauer in Minuten (Standard: 25)
    /// </summary>
    public int MinLunchBreakMinutes { get; set; } = 25;

    /// <summary>
    /// Maximale Mittagspausendauer in Minuten (Standard: 35)
    /// </summary>
    public int MaxLunchBreakMinutes { get; set; } = 35;

    /// <summary>
    /// Arbeitsende (z.B. 18:00). Null = kein Arbeitsende
    /// </summary>
    public TimeSpan? WorkEndTime { get; set; } = new TimeSpan(18, 0, 0);

    /// <summary>
    /// Varianz für Arbeitsende in Minuten (Standard: ±30 Minuten)
    /// </summary>
    public int WorkEndVarianceMinutes { get; set; } = 30;

    /// <summary>
    /// Arbeitsbeginn (Standard: 8:00)
    /// </summary>
    public TimeSpan WorkStartTime { get; set; } = new TimeSpan(8, 0, 0);

    /// <summary>
    /// Varianz für Arbeitsbeginn in Minuten (Standard: ±15 Minuten)
    /// </summary>
    public int WorkStartVarianceMinutes { get; set; } = 15;
}
