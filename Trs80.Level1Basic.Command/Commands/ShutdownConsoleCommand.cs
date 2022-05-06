using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Console;

namespace Trs80.Level1Basic.Command.Commands;

public class ShutdownConsoleCommand : ICommand<ShutdownConsoleModel>
{
    private readonly IConsole _console;
    private readonly IConsoleDataModel _sharedDataModel;
    public ShutdownConsoleCommand(IConsole console, IConsoleDataModel sharedDataModel)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _sharedDataModel = sharedDataModel ?? throw new ArgumentNullException(nameof(sharedDataModel));
    }

    public void Execute(ShutdownConsoleModel parameterObject)
    {
        _console.SetCurrentFont(_sharedDataModel.OriginalConsoleFont);
    }
}