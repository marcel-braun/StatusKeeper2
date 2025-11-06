using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using StatusKeeperTerminalApp.Services;

class Program
{
	static async Task Main(string[] args)
	{

		using var host = Host.CreateDefaultBuilder(args)
			.ConfigureServices((_, services) =>
			{
                services.AddSingleton<IGlobalStateService, GlobalStateService>();
                services.AddSingleton<IMyService, MyService>();
                services.AddSingleton<IMouseMovementService, MouseMovementService>();
			})
			.Build();

		var globalState = host.Services.GetRequiredService<IGlobalStateService>();

        // Ausgabe der Umgebungsvariable DOTNET_ENVIRONMENT
        globalState.Environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        //Console.WriteLine($"DOTNET_ENVIRONMENT: {globalState.Environment ?? "(nicht gesetzt)"}");

        if (globalState.Environment == "Development")
        {
            globalState.Debug = true;
        }
        
        var entry = host.Services.GetRequiredService<IMyService>();
        entry.Run();
		//Console.WriteLine($"Global Debug: {globalState.Debug}");

        // Mouse Movement Service starten
        Console.WriteLine("\n=== Mouse Movement Service ===");
        Console.WriteLine("Startet Service mit Standardkonfiguration...");
        
        var mouseService = host.Services.GetRequiredService<IMouseMovementService>();
        var config = new MouseMovementConfig
        {
            // Mausbewegung: Minimale Distanz in Pixeln (sehr klein, um natürlich zu wirken)
            MinMovementDistance = 1,
            // Mausbewegung: Maximale Distanz in Pixeln
            MaxMovementDistance = 3,
            
            // Zeitintervall: Minimale Sekunden zwischen Mausbewegungen
            MinIntervalSeconds = 30,
            // Zeitintervall: Maximale Sekunden zwischen Mausbewegungen
            MaxIntervalSeconds = 120,
            
            // Kurze Pausen: Minimale Dauer in Minuten (mind. 5 Min für Teams-Reaktion)
            MinBreakMinutes = 5,
            // Kurze Pausen: Maximale Dauer in Minuten
            MaxBreakMinutes = 10,
            // Kurze Pausen: Wahrscheinlichkeit in Prozent, dass eine Pause nach einer Bewegung eintritt (10% = jede 10. Bewegung ca.)
            BreakProbabilityPercent = 10,
            
            // Mittagspause: Frühester Startzeitpunkt (12:00 Uhr)
            LunchBreakStart = new TimeSpan(12, 0, 0),
            // Mittagspause: Spätester Startzeitpunkt (14:00 Uhr) - tatsächlicher Start wird zufällig innerhalb dieses Fensters gewählt
            LunchBreakEnd = new TimeSpan(14, 0, 0),
            // Mittagspause: Minimale Dauer in Minuten
            MinLunchBreakMinutes = 25,
            // Mittagspause: Maximale Dauer in Minuten
            MaxLunchBreakMinutes = 35,
            
            // Arbeitszeit: Arbeitsbeginn (8:00 Uhr)
            WorkStartTime = new TimeSpan(8, 0, 0),
            // Arbeitszeit: Varianz für Arbeitsbeginn in Minuten (±15 Min = zwischen 7:45 und 8:15 Uhr)
            WorkStartVarianceMinutes = 15,
            // Arbeitszeit: Arbeitsende (18:00 Uhr)
            WorkEndTime = new TimeSpan(18, 0, 0),
            // Arbeitszeit: Varianz für Arbeitsende in Minuten (±30 Min = zwischen 17:30 und 18:30 Uhr)
            WorkEndVarianceMinutes = 30
        };

        await mouseService.StartAsync(config);
	}
}