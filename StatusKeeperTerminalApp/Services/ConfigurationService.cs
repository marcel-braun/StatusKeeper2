using System.Text.Json;
using StatusKeeperTerminalApp.Models;
using StatusKeeperTerminalApp.Services;

namespace StatusKeeperTerminalApp.Services;

public interface IConfigurationService
{
    AppSettings LoadSettings();
    void SaveSettings(AppSettings settings);
    MouseMovementProfile? GetActiveProfile();
    void SetActiveProfile(string profileName);
    void SaveProfile(MouseMovementProfile profile);
    void DeleteProfile(string profileName);
}

public class ConfigurationService : IConfigurationService
{
    private readonly string _settingsPath;
    private AppSettings _settings;

    public ConfigurationService()
    {
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        _settings = LoadSettings();
    }

    public AppSettings LoadSettings()
    {
        if (!File.Exists(_settingsPath))
        {
            // Erstelle Standardkonfiguration
            var defaultSettings = CreateDefaultSettings();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        try
        {
            var json = File.ReadAllText(_settingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });
            
            _settings = settings ?? CreateDefaultSettings();
            return _settings;
        }
        catch
        {
            return CreateDefaultSettings();
        }
    }

    public void SaveSettings(AppSettings settings)
    {
        _settings = settings;
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(_settingsPath, json);
    }

    public MouseMovementProfile? GetActiveProfile()
    {
        return _settings.Profiles.FirstOrDefault(p => p.Name == _settings.ActiveProfileName);
    }

    public void SetActiveProfile(string profileName)
    {
        if (_settings.Profiles.Any(p => p.Name == profileName))
        {
            _settings.ActiveProfileName = profileName;
            SaveSettings(_settings);
        }
    }

    public void SaveProfile(MouseMovementProfile profile)
    {
        var existingProfile = _settings.Profiles.FirstOrDefault(p => p.Name == profile.Name);
        if (existingProfile != null)
        {
            _settings.Profiles.Remove(existingProfile);
        }
        _settings.Profiles.Add(profile);
        SaveSettings(_settings);
    }

    public void DeleteProfile(string profileName)
    {
        var profile = _settings.Profiles.FirstOrDefault(p => p.Name == profileName);
        if (profile != null && _settings.Profiles.Count > 1) // Mindestens ein Profil behalten
        {
            _settings.Profiles.Remove(profile);
            if (_settings.ActiveProfileName == profileName)
            {
                _settings.ActiveProfileName = _settings.Profiles.First().Name;
            }
            SaveSettings(_settings);
        }
    }

    private AppSettings CreateDefaultSettings()
    {
        return new AppSettings
        {
            ActiveProfileName = "Standard",
            Profiles = new List<MouseMovementProfile>
            {
                new MouseMovementProfile
                {
                    Name = "Standard",
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
                },
                new MouseMovementProfile
                {
                    Name = "Home Office",
                    Config = new MouseMovementConfig
                    {
                        MinMovementDistance = 1,
                        MaxMovementDistance = 5,
                        MinIntervalSeconds = 45,
                        MaxIntervalSeconds = 180,
                        MinBreakMinutes = 5,
                        MaxBreakMinutes = 15,
                        BreakProbabilityPercent = 15,
                        LunchBreakStart = new TimeSpan(12, 30, 0),
                        LunchBreakEnd = new TimeSpan(13, 30, 0),
                        MinLunchBreakMinutes = 30,
                        MaxLunchBreakMinutes = 45,
                        WorkStartTime = new TimeSpan(7, 00, 0),
                        WorkStartVarianceMinutes = 10,
                        WorkEndTime = new TimeSpan(16, 00, 0),
                        WorkEndVarianceMinutes = 20
                    }
                }
            }
        };
    }
}
