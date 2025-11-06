public interface IGlobalStateService
{
    bool Debug { get; set; }
    string? Environment { get; set; }
}

public class GlobalStateService : IGlobalStateService
{
    public bool Debug { get; set; }
    public string? Environment { get; set; }
}
