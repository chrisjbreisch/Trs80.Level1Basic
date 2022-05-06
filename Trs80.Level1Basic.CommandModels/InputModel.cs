namespace Trs80.Level1Basic.CommandModels;

public class InputModel
{
    public bool WritePrompt { get; set; }
    public bool Done { get; set; }
    public string SourceLine { get; set; } = string.Empty;
}