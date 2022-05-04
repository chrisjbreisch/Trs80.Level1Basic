using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Interpreter;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class SetupConsoleCommand : ICommand<SetupConsoleModel>
{
    private readonly IConsole _console;

    public SetupConsoleCommand(IConsole console, IConsoleDataModel consoleDataModel)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));

        if (consoleDataModel is null) throw new ArgumentNullException(nameof(consoleDataModel));
        consoleDataModel.OriginalConsoleFont = _console.GetCurrentFont();
    }

    public void Execute(SetupConsoleModel model)
    {
        InitializeWindow();
        WritePrompt();
    }

    private void InitializeWindow()
    {
        _console.InitializeWindow();
    }

    private void WritePrompt()
    {
        _console.WriteLine();
        _console.WriteLine("READY");
    }
}