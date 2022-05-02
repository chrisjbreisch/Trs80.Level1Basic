using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Interpreter;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class ShutdownConsoleCommand : ICommand<ShutdownConsoleModel>
{
    private readonly ILogger _logger;
    private readonly IConsole _console;
    private readonly ISharedDataModel _sharedDataModel;
    public ShutdownConsoleCommand(ILoggerFactory logFactory, IConsole console, ISharedDataModel sharedDataModel)
    {
        _logger = logFactory.CreateLogger<ShutdownConsoleCommand>();
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _sharedDataModel = sharedDataModel ?? throw new ArgumentNullException(nameof(sharedDataModel));
    }

    public void Execute(ShutdownConsoleModel parameterObject)
    {
        _console.SetCurrentFont(_sharedDataModel.OriginalConsoleFont);
    }
}