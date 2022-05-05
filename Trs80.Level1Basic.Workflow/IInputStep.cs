using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

public interface IInputStep : IStepBody
{
    bool WritePrompt { get; set; }
    bool Done { get; set; }
    string SourceLine { get; set; }
}