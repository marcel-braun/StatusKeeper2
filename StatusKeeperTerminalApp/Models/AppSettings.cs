namespace StatusKeeperTerminalApp.Models;

public class AppSettings
{
    public List<MouseMovementProfile> Profiles { get; set; } = new();
    public string ActiveProfileName { get; set; } = "Default";
}
