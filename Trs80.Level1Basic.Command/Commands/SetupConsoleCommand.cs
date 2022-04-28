using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Exceptions;
using Trs80.Level1Basic.Services;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class SetupConsoleCommand : ICommand<SetupConsoleModel>
{
    private readonly ILogger _logger;
    private readonly ITrs80Console _console;
    private readonly ISharedDataModel _sharedDataModel;
    public SetupConsoleCommand(ILoggerFactory logFactory, ITrs80Console console, ISharedDataModel sharedDataModel)
    {
        _logger = logFactory.CreateLogger<SetupConsoleCommand>();
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _sharedDataModel = sharedDataModel ?? throw new ArgumentNullException(nameof(sharedDataModel));
        _sharedDataModel.OriginalConsoleFont = _console.GetCurrentFont();
    }

    public void Execute(SetupConsoleModel parameterObject)
    {
        InitializeWindow();

        _console.WriteLine("Enter TRS-80 LEVEL 1 BASIC Commands or Type EXIT to Exit.");
        WritePrompt();
    }
    
    private void InitializeWindow()
    {
        _console.SetCurrentFont(new ConsoleFont { FontName = "Another Mans Treasure MIB 64C 2X3Y", FontSize = 48 });
        _console.DisableCursorBlink();
        _console.SetWindowSize(64, 16);
        _console.SetBufferSize(64, 160);
    }

    private void WritePrompt()
    {
        _console.WriteLine();
        _console.WriteLine("READY");
    }
}