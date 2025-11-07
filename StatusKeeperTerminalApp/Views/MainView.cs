using Terminal.Gui;
using StatusKeeperTerminalApp.Services;

namespace StatusKeeperTerminalApp.Views;

public class MainView : Window
{
    private readonly IConfigurationService _configService;
    private readonly IMouseMovementService _mouseService;
    private readonly IGlobalStateService _globalState;
    private Label _statusLabel = null!;
    private Button _startStopButton = null!;
    private TextView _logView = null!;

    public MainView(IConfigurationService configService, IMouseMovementService mouseService, IGlobalStateService globalState)
    {
        _configService = configService;
        _mouseService = mouseService;
        _globalState = globalState;

        Title = "Status Keeper - Hauptmenü";
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();

        InitializeComponents();
        StartStatusUpdateTimer();
    }

    private void InitializeComponents()
    {
        var activeProfile = _configService.GetActiveProfile();
        var profileLabel = new Label($"Aktives Profil: {activeProfile?.Name ?? "Keins"}")
        {
            X = 2,
            Y = 1
        };
        Add(profileLabel);

        _statusLabel = new Label("Service Status: Gestoppt")
        {
            X = 2,
            Y = 2,
            ColorScheme = Colors.Base
        };
        Add(_statusLabel);

        var configButton = new Button("Konfiguration bearbeiten")
        {
            X = 2,
            Y = 4
        };
        configButton.Clicked += () =>
        {
            var configView = new ConfigurationView(_configService);
            configView.Closed += (args) =>
            {
                // Aktualisiere Profil-Label
                var newActiveProfile = _configService.GetActiveProfile();
                profileLabel.Text = $"Aktives Profil: {newActiveProfile?.Name ?? "Keins"}";
            };
            Application.Run(configView);
        };
        Add(configButton);

        _startStopButton = new Button("Service starten")
        {
            X = 2,
            Y = 6
        };
        _startStopButton.Clicked += async () =>
        {
            if (!_globalState.IsServiceRunning)
            {
                var profile = _configService.GetActiveProfile();
                if (profile != null)
                {
                    _ = Task.Run(async () => await _mouseService.StartAsync(profile.Config));
                    _globalState.AddLog($"Service wird gestartet mit Profil '{profile.Name}'");
                }
                else
                {
                    MessageBox.ErrorQuery("Fehler", "Kein aktives Profil ausgewählt!", "OK");
                }
            }
            else
            {
                _mouseService.Stop();
            }
        };
        Add(_startStopButton);

        // Log-View hinzufügen
        var logFrame = new FrameView("Aktivitäts-Log")
        {
            X = 2,
            Y = 8,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 12
        };

        _logView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true,
            Text = string.Join("\n", _globalState.Logs)
        };
        
        logFrame.Add(_logView);
        Add(logFrame);

        var aboutButton = new Button("Über")
        {
            X = 2,
            Y = Pos.Bottom(logFrame) + 1
        };
        aboutButton.Clicked += () =>
        {
            var aboutView = new AboutView();
            Application.Run(aboutView);
        };
        Add(aboutButton);

        var exitButton = new Button("Beenden")
        {
            X = Pos.Right(aboutButton) + 2,
            Y = Pos.Bottom(logFrame) + 1
        };
        exitButton.Clicked += () => Application.RequestStop();
        Add(exitButton);
    }

    private void StartStatusUpdateTimer()
    {
        // Timer für Status-Updates (alle 500ms)
        var timer = Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(500), (_) =>
        {
            UpdateStatus();
            return true; // true = Timer wiederholen
        });
    }

    private void UpdateStatus()
    {
        if (_globalState.IsServiceRunning)
        {
            _statusLabel.Text = "Service Status: Läuft ✓";
            _statusLabel.ColorScheme = new ColorScheme
            {
                Normal = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black)
            };
            _startStopButton.Text = "Service stoppen";
        }
        else
        {
            _statusLabel.Text = "Service Status: Gestoppt";
            _statusLabel.ColorScheme = Colors.Base;
            _startStopButton.Text = "Service starten";
        }

        // Log-View aktualisieren
        var logText = string.Join("\n", _globalState.Logs);
        if (_logView.Text.ToString() != logText)
        {
            _logView.Text = logText;
            
            // Automatisch zum Ende scrollen, wenn neue Logs hinzugefügt wurden
            if (_globalState.Logs.Count > 0)
            {
                _logView.MoveEnd();
            }
        }

        Application.Refresh();
    }
}
