using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Machine;

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

        if (parameterObject.SourceLine.Line == "EXIT")
            parameterObject.Done = true;
    }

    private SourceLine GetInputLine()
    {
        char[] original = new char[1024];
        char[] line = new char[1024];
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
                    original[charCount--] = '\0';
                }
                else
                    _trs80.Write(">");
            }
            else
            {
                original[charCount] = key.KeyChar;
                char upper = Upper(key.KeyChar);
                line[charCount++] = upper;
                _trs80.Write($"\b{upper}");
            }
        }

        return charCount <= 0
            ? new SourceLine()
            : new SourceLine
            {
                Line = new string(line, 0, charCount), 
                Original = new string(original, 0, charCount)
            };
    }

    private char Upper(char key)
    {
        if (!char.IsLetter(key)) return key;
        if (key <= 'Z') return key;
        return (char)((byte)key - 'a' + 'A');
    }
}