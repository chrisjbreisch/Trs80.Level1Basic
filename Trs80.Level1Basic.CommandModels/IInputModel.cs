namespace Trs80.Level1Basic.CommandModels;

public interface IInputModel
{
    bool WritePrompt { get; set; }
    bool Done { get; set; }
    string SourceLine { get; set; }
}