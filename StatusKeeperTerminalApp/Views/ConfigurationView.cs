using Terminal.Gui;
using StatusKeeperTerminalApp.Models;
using StatusKeeperTerminalApp.Services;

namespace StatusKeeperTerminalApp.Views;

public class ConfigurationView : Window
{
    private readonly IConfigurationService _configService;
    private MouseMovementProfile? _currentProfile;
    private ListView _profileListView = null!;
    private TextField[] _configFields = null!;

    public ConfigurationView(IConfigurationService configService)
    {
        _configService = configService;

        Title = "Konfiguration";
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();

        InitializeComponents();

        // ESC schließt die View
        KeyDown += (e) =>
        {
            if (e.KeyEvent.Key == Key.Esc)
            {
                Application.RequestStop();
                e.Handled = true;
            }
        };
    }

    private void InitializeComponents()
    {
        var settings = _configService.LoadSettings();
        _currentProfile = _configService.GetActiveProfile();

        // Zurück-Button oben
        var backButton = new Button("← Zurück zum Hauptmenü")
        {
            X = 1,
            Y = 0
        };
        backButton.Clicked += () => 
        {
            Application.RequestStop();
        };
        Add(backButton);

        // Profil-Auswahl (links)
        var profileFrame = new FrameView("Profile")
        {
            X = 1,
            Y = 2,
            Width = 25,
            Height = Dim.Fill() - 5
        };

        _profileListView = new ListView(settings.Profiles.Select(p => p.Name).ToList())
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 3
        };
        _profileListView.SelectedItemChanged += (args) =>
        {
            var profileName = settings.Profiles[args.Item].Name;
            _currentProfile = settings.Profiles[args.Item];
            LoadProfileIntoFields();
        };
        profileFrame.Add(_profileListView);

        var newProfileButton = new Button("Neu")
        {
            X = 0,
            Y = Pos.Bottom(_profileListView)
        };
        newProfileButton.Clicked += () => CreateNewProfile();
        profileFrame.Add(newProfileButton);

        var deleteProfileButton = new Button("Löschen")
        {
            X = Pos.Right(newProfileButton) + 1,
            Y = Pos.Bottom(_profileListView)
        };
        deleteProfileButton.Clicked += () => DeleteCurrentProfile();
        profileFrame.Add(deleteProfileButton);

        Add(profileFrame);

        // Konfigurationsfelder (rechts)
        var configFrame = new FrameView("Einstellungen")
        {
            X = Pos.Right(profileFrame) + 1,
            Y = 2,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 5  // Platz für Buttons unten lassen
        };

        _configFields = new TextField[15];
        int yPos = 0;

        // Mausbewegung
        AddLabel(configFrame, "=== Mausbewegung ===", ref yPos);
        AddField(configFrame, "Min Distanz (Pixel):", ref yPos, 0);
        AddField(configFrame, "Max Distanz (Pixel):", ref yPos, 1);

        // Zeitintervalle
        yPos++;
        AddLabel(configFrame, "=== Zeitintervalle ===", ref yPos);
        AddField(configFrame, "Min Intervall (Sek):", ref yPos, 2);
        AddField(configFrame, "Max Intervall (Sek):", ref yPos, 3);

        // Kurze Pausen
        yPos++;
        AddLabel(configFrame, "=== Kurze Pausen ===", ref yPos);
        AddField(configFrame, "Min Pause (Min):", ref yPos, 4);
        AddField(configFrame, "Max Pause (Min):", ref yPos, 5);
        AddField(configFrame, "Wahrscheinlichkeit (%):", ref yPos, 6);

        // Mittagspause
        yPos++;
        AddLabel(configFrame, "=== Mittagspause ===", ref yPos);
        AddField(configFrame, "Start (HH:MM):", ref yPos, 7);
        AddField(configFrame, "Ende (HH:MM):", ref yPos, 8);
        AddField(configFrame, "Min Dauer (Min):", ref yPos, 9);
        AddField(configFrame, "Max Dauer (Min):", ref yPos, 10);

        // Arbeitszeit
        yPos++;
        AddLabel(configFrame, "=== Arbeitszeit ===", ref yPos);
        AddField(configFrame, "Start (HH:MM):", ref yPos, 11);
        AddField(configFrame, "Start Varianz (Min):", ref yPos, 12);
        AddField(configFrame, "Ende (HH:MM):", ref yPos, 13);
        AddField(configFrame, "Ende Varianz (Min):", ref yPos, 14);

        Add(configFrame);

        // Buttons unten
        var saveButton = new Button("Speichern")
        {
            X = Pos.Right(profileFrame) + 1,
            Y = Pos.Bottom(configFrame) + 1
        };
        saveButton.Clicked += () => SaveCurrentProfile();
        Add(saveButton);

        var activateButton = new Button("Als aktiv setzen")
        {
            X = Pos.Right(saveButton) + 2,
            Y = Pos.Bottom(configFrame) + 1
        };
        activateButton.Clicked += () => SetActiveProfile();
        Add(activateButton);

        var closeButton = new Button("Schließen")
        {
            X = Pos.Right(activateButton) + 2,
            Y = Pos.Bottom(configFrame) + 1
        };
        closeButton.Clicked += () => 
        {
            Application.RequestStop();
            this.Running = false;
        };
        Add(closeButton);

        LoadProfileIntoFields();
    }

    private void AddLabel(View parent, string text, ref int yPos)
    {
        var label = new Label(text)
        {
            X = 1,
            Y = yPos++
        };
        parent.Add(label);
    }

    private void AddField(View parent, string label, ref int yPos, int fieldIndex)
    {
        var lbl = new Label(label)
        {
            X = 1,
            Y = yPos,
            Width = 25
        };
        parent.Add(lbl);

        _configFields[fieldIndex] = new TextField("")
        {
            X = 27,
            Y = yPos++,
            Width = 20
        };
        parent.Add(_configFields[fieldIndex]);
    }

    private void LoadProfileIntoFields()
    {
        if (_currentProfile == null) return;

        var c = _currentProfile.Config;
        _configFields[0].Text = c.MinMovementDistance.ToString();
        _configFields[1].Text = c.MaxMovementDistance.ToString();
        _configFields[2].Text = c.MinIntervalSeconds.ToString();
        _configFields[3].Text = c.MaxIntervalSeconds.ToString();
        _configFields[4].Text = c.MinBreakMinutes.ToString();
        _configFields[5].Text = c.MaxBreakMinutes.ToString();
        _configFields[6].Text = c.BreakProbabilityPercent.ToString();
        _configFields[7].Text = c.LunchBreakStart.ToString(@"hh\:mm");
        _configFields[8].Text = c.LunchBreakEnd.ToString(@"hh\:mm");
        _configFields[9].Text = c.MinLunchBreakMinutes.ToString();
        _configFields[10].Text = c.MaxLunchBreakMinutes.ToString();
        _configFields[11].Text = c.WorkStartTime.ToString(@"hh\:mm");
        _configFields[12].Text = c.WorkStartVarianceMinutes.ToString();
        _configFields[13].Text = c.WorkEndTime?.ToString(@"hh\:mm") ?? "18:00";
        _configFields[14].Text = c.WorkEndVarianceMinutes.ToString();
    }

    private void SaveCurrentProfile()
    {
        if (_currentProfile == null) return;

        try
        {
            var c = _currentProfile.Config;
            c.MinMovementDistance = int.Parse(_configFields[0].Text.ToString()!);
            c.MaxMovementDistance = int.Parse(_configFields[1].Text.ToString()!);
            c.MinIntervalSeconds = int.Parse(_configFields[2].Text.ToString()!);
            c.MaxIntervalSeconds = int.Parse(_configFields[3].Text.ToString()!);
            c.MinBreakMinutes = int.Parse(_configFields[4].Text.ToString()!);
            c.MaxBreakMinutes = int.Parse(_configFields[5].Text.ToString()!);
            c.BreakProbabilityPercent = int.Parse(_configFields[6].Text.ToString()!);
            c.LunchBreakStart = TimeSpan.Parse(_configFields[7].Text.ToString()!);
            c.LunchBreakEnd = TimeSpan.Parse(_configFields[8].Text.ToString()!);
            c.MinLunchBreakMinutes = int.Parse(_configFields[9].Text.ToString()!);
            c.MaxLunchBreakMinutes = int.Parse(_configFields[10].Text.ToString()!);
            c.WorkStartTime = TimeSpan.Parse(_configFields[11].Text.ToString()!);
            c.WorkStartVarianceMinutes = int.Parse(_configFields[12].Text.ToString()!);
            c.WorkEndTime = TimeSpan.Parse(_configFields[13].Text.ToString()!);
            c.WorkEndVarianceMinutes = int.Parse(_configFields[14].Text.ToString()!);

            _configService.SaveProfile(_currentProfile);
            MessageBox.Query("Erfolg", "Profil gespeichert!", "OK");
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Fehler", $"Fehler beim Speichern: {ex.Message}", "OK");
        }
    }

    private void SetActiveProfile()
    {
        if (_currentProfile != null)
        {
            _configService.SetActiveProfile(_currentProfile.Name);
            MessageBox.Query("Erfolg", $"Profil '{_currentProfile.Name}' aktiviert!", "OK");
        }
    }

    private void CreateNewProfile()
    {
        var nameField = new TextField("")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1
        };

        var dialog = new Dialog("Neues Profil", 50, 7);
        dialog.Add(new Label("Profilname:") { X = 1, Y = 0 });
        dialog.Add(nameField);

        var okButton = new Button("OK");
        okButton.Clicked += () =>
        {
            var name = nameField.Text.ToString();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var newProfile = new MouseMovementProfile
                {
                    Name = name,
                    Config = new MouseMovementConfig
                    {
                        MinMovementDistance = 1,
                        MaxMovementDistance = 3,
                        MinIntervalSeconds = 30,
                        MaxIntervalSeconds = 120,
                        MinBreakMinutes = 5,
                        MaxBreakMinutes = 10,
                        BreakProbabilityPercent = 10,
                        LunchBreakStart = new TimeSpan(12, 0, 0),
                        LunchBreakEnd = new TimeSpan(14, 0, 0),
                        MinLunchBreakMinutes = 25,
                        MaxLunchBreakMinutes = 35,
                        WorkStartTime = new TimeSpan(8, 0, 0),
                        WorkStartVarianceMinutes = 15,
                        WorkEndTime = new TimeSpan(18, 0, 0),
                        WorkEndVarianceMinutes = 30
                    }
                };
                _configService.SaveProfile(newProfile);
                
                // Aktualisiere Liste
                var settings = _configService.LoadSettings();
                _profileListView.SetSource(settings.Profiles.Select(p => p.Name).ToList());
                
                Application.RequestStop();
            }
        };
        dialog.AddButton(okButton);

        var cancelButton = new Button("Abbrechen");
        cancelButton.Clicked += () => Application.RequestStop();
        dialog.AddButton(cancelButton);

        Application.Run(dialog);
    }

    private void DeleteCurrentProfile()
    {
        if (_currentProfile == null) return;

        var result = MessageBox.Query("Bestätigung", 
            $"Profil '{_currentProfile.Name}' wirklich löschen?", "Ja", "Nein");
        
        if (result == 0)
        {
            _configService.DeleteProfile(_currentProfile.Name);
            
            // Aktualisiere Liste
            var settings = _configService.LoadSettings();
            _profileListView.SetSource(settings.Profiles.Select(p => p.Name).ToList());
            _currentProfile = _configService.GetActiveProfile();
            LoadProfileIntoFields();
        }
    }
}
