using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Environment;

namespace Trs80.Level1Basic.Command.Commands;

public class InputCommand : ICommand<InputModel>
{
    private readonly ITrs80 _trs80;

    public InputCommand(ITrs80 trs80)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
    }

    public void Execute(InputModel parameterObject)
    {
        _trs80.Write(">");
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
            ConsoleKeyInfo key = _trs80.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                _trs80.WriteLine();
                break;
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (charCount > 0)
                {
                    _trs80.Write(" \b");
                    input[charCount--] = '\0';
                }
                else
                    _trs80.Write(">");
            }
            else
                input[charCount++] = key.KeyChar;
        }

        return charCount <= 0 ? string.Empty : new string(input, 0, charCount);
    }
}