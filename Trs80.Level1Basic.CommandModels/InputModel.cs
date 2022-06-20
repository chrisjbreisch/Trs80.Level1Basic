using Trs80.Level1Basic.Common;

namespace Trs80.Level1Basic.CommandModels;

public class InputModel
{
    public bool WritePrompt { get; set; }
    public bool Done { get; set; }
    public SourceLine SourceLine { get; set; } = new();
}