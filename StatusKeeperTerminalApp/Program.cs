using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using StatusKeeperTerminalApp.Services;
using StatusKeeperTerminalApp.Views;
using Terminal.Gui;

class Program
{
	static void Main(string[] args)
	{
		using var host = Host.CreateDefaultBuilder(args)
			.ConfigureServices((_, services) =>
			{
                services.AddSingleton<IGlobalStateService, GlobalStateService>();
                services.AddSingleton<IMyService, MyService>();
                services.AddSingleton<IMouseMovementService, MouseMovementService>();
                services.AddSingleton<IConfigurationService, ConfigurationService>();
			})
			.Build();

		var globalState = host.Services.GetRequiredService<IGlobalStateService>();

        // Ausgabe der Umgebungsvariable DOTNET_ENVIRONMENT
        globalState.Environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        if (globalState.Environment == "Development")
        {
            globalState.Debug = true;
        }

        // Terminal.Gui Application starten
        Application.Init();
        
        try
        {
            var configService = host.Services.GetRequiredService<IConfigurationService>();
            var mouseService = host.Services.GetRequiredService<IMouseMovementService>();
            
            var mainView = new MainView(configService, mouseService, globalState);
            Application.Run(mainView);
        }
        finally
        {
            Application.Shutdown();
        }
	}
}