using StatusKeeperTerminalApp.Services;

namespace StatusKeeperTerminalApp.Models;

public class MouseMovementProfile
{
    public string Name { get; set; } = "Default";
    public MouseMovementConfig Config { get; set; } = new();
}
