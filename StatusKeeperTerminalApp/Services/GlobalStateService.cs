public interface IGlobalStateService
{
    bool Debug { get; set; }
    string? Environment { get; set; }
    bool IsServiceRunning { get; set; }
    List<string> Logs { get; }
    void AddLog(string message);
}

public class GlobalStateService : IGlobalStateService
{
    public bool Debug { get; set; }
    public string? Environment { get; set; }
    public bool IsServiceRunning { get; set; }
    public List<string> Logs { get; } = new List<string>();

    public void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        Logs.Add($"[{timestamp}] {message}");
        
        // Begrenze Log-Größe auf die letzten 100 Einträge
        if (Logs.Count > 100)
        {
            Logs.RemoveAt(0);
        }
    }
}
