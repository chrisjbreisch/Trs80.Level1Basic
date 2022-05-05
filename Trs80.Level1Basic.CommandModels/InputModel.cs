using Trs80.Level1Basic.WorkflowDomain;

namespace Trs80.Level1Basic.CommandModels;

public class InputModel : IInputModel
{
    public bool WritePrompt { get; set; }
    public bool Done { get; set; }
    public string SourceLine { get; set; } = string.Empty;
}