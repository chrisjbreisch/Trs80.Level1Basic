using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Services;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class SetupConsoleCommand : ICommand<SetupConsoleModel>
{
    private readonly ILogger _logger;
    private readonly IConsole _console;

    public SetupConsoleCommand(ILoggerFactory logFactory, IConsole console, ISharedDataModel sharedDataModel)
    {
        _logger = logFactory.CreateLogger<SetupConsoleCommand>();
        _console = console ?? throw new ArgumentNullException(nameof(console));

        if (sharedDataModel is null) throw new ArgumentNullException(nameof(sharedDataModel));
        sharedDataModel.OriginalConsoleFont = _console.GetCurrentFont();
    }

    public void Execute(SetupConsoleModel parameterObject)
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