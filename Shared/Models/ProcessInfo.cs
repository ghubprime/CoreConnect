namespace CoreConnect.Shared.Models;

public class ProcessInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double WorkingSetBytes { get; set; }
    public string MainWindowTitle { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}
