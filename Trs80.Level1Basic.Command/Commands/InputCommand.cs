using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Console;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class InputCommand : ICommand<InputModel>
{
    private readonly IConsole _console;

    public InputCommand(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Execute(InputModel parameterObject)
    {
        _console.Write(">");
        parameterObject.SourceLine = GetInputLine();

        if (parameterObject.SourceLine.ToLower() == "exit")
            parameterObject.Done = true;
    }

    private string GetInputLine()
    {
        char[] input = new char[1024];
        int charCount = 0;

        while (true)
        {
            ConsoleKeyInfo key = _console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                _console.WriteLine();
                break;
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (charCount > 0)
                {
                    _console.Write(" \b");
                    input[charCount--] = '\0';
                }
                else
                    _console.Write(">");
            }
            else
                input[charCount++] = key.KeyChar;
        }

        return charCount <= 0 ? string.Empty : new string(input, 0, charCount);
    }
}