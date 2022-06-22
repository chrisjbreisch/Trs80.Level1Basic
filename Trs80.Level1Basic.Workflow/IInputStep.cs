using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.Common;

using WorkflowCore.Interface;

namespace Trs80.Level1Basic.Workflow;

[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IInputStep : IStepBody
{
    bool WritePrompt { get; set; }
    bool Done { get; set; }
    SourceLine SourceLine { get; set; }
}