using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class InputCommand : ICommand<InputModel>
{
    private readonly ITrs80 _trs80;
    private readonly LineEditorHistory _history;
    private readonly AutoLineNumbering _autoLineNumbering;
    private readonly IProgram _program;

    public InputCommand(ITrs80 trs80, LineEditorHistory history, AutoLineNumbering autoLineNumbering, IProgram program)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _history = history ?? throw new ArgumentNullException(nameof(history));
        _autoLineNumbering = autoLineNumbering ?? throw new ArgumentNullException(nameof(autoLineNumbering));
        _program = program ?? throw new ArgumentNullException(nameof(program));
    }

    public void Execute(InputModel parameterObject)
    {
        while (true)
        {
            string prompt = _autoLineNumbering.IsActive
                ? $"{_autoLineNumbering.NextLineNumber} "
                : parameterObject.WritePrompt ? ">" : string.Empty;
            SourceLine sourceLine = GetInputLine(out bool cancelled, out bool pauseRequested, prompt: prompt);

            if (pauseRequested)
            {
                if (_autoLineNumbering.IsActive)
                    _autoLineNumbering.Stop();

                continue;
            }

            if (cancelled)
                continue;

            if (!_autoLineNumbering.IsActive && TryStartAuto(sourceLine.Original))
            {
                continue;
            }

            if (!_autoLineNumbering.IsActive && TryGetEditLine(sourceLine.Original, out int editLineNumber, out string existingLine))
            {
                string editPrompt = parameterObject.WritePrompt ? ">" : string.Empty;
                SourceLine editedLine = GetInputLine(out cancelled, out _, existingLine, editPrompt);
                if (cancelled)
                    continue;

                if (string.IsNullOrWhiteSpace(editedLine.Original))
                    continue;

                string numberedLine = $"{editLineNumber} {editedLine.Original}";
                sourceLine = new SourceLine
                {
                    Original = numberedLine,
                    Line = new(numberedLine.Select(Upper).ToArray())
                };
            }

            if (_autoLineNumbering.IsActive)
            {
                if (!_autoLineNumbering.TryNumber(sourceLine.Original, out string numberedLine))
                {
                    _autoLineNumbering.Stop();
                    continue;
                }

                sourceLine = new SourceLine
                {
                    Original = numberedLine,
                    Line = new(numberedLine.Select(Upper).ToArray())
                };
            }

            parameterObject.SourceLine = sourceLine;
            break;
        }

        if (parameterObject.SourceLine.Line == "EXIT")
            parameterObject.Done = true;
    }

    private bool TryStartAuto(string sourceLine)
    {
        string command = sourceLine.Trim();
        if (!command.StartsWith("AUTO", StringComparison.OrdinalIgnoreCase) ||
            (command.Length > 4 && !char.IsWhiteSpace(command[4])))
            return false;

        string options = command[4..].Trim();
        if (options.Length == 0)
        {
            _autoLineNumbering.Start();
            return true;
        }

        string[] values = options.Split(',', StringSplitOptions.TrimEntries);
        if (values.Length > 2 || !int.TryParse(values[0], out int firstLineNumber))
            return false;

        int increment = 10;
        if (values.Length == 2 && !int.TryParse(values[1], out increment))
            return false;

        _autoLineNumbering.Start(firstLineNumber, increment);
        return true;
    }

    private bool TryGetEditLine(string sourceLine, out int lineNumber, out string existingLine)
    {
        lineNumber = 0;
        existingLine = string.Empty;
        string command = sourceLine.Trim();
        if (!command.StartsWith("EDIT", StringComparison.OrdinalIgnoreCase) ||
            (command.Length > 4 && !char.IsWhiteSpace(command[4])) ||
            !int.TryParse(command[4..].Trim(), out int parsedLineNumber))
            return false;

        IStatement? statement = _program.List().FirstOrDefault(item => item.LineNumber == parsedLineNumber);
        if (statement == null)
            return false;

        lineNumber = parsedLineNumber;
        existingLine = statement.SourceLine;
        return true;
    }

    private SourceLine GetInputLine(out bool cancelled, out bool pauseRequested,
        string initialText = "", string prompt = ">")
    {
        cancelled = false;
        pauseRequested = false;
        var buffer = new LineEditorBuffer(initialText);
        if (buffer.Length > 0)
            RedrawLine(buffer, prompt);
        else
            _trs80.Write(prompt);

        while (true)
        {
            ConsoleKeyInfo key = _trs80.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                _trs80.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.UpArrow && _history.TryGetPrevious(buffer.ToString(), out string recalledLine))
                buffer.Replace(recalledLine);
            else if (key.Key == ConsoleKey.DownArrow && _history.TryGetNext(out string nextLine))
                buffer.Replace(nextLine);
            else if (key.Key == ConsoleKey.Escape)
            {
                buffer.Clear();
                RedrawLine(buffer, prompt);
                cancelled = true;
                return new SourceLine { Line = string.Empty, Original = string.Empty };
            }
            else if (key.Key == ConsoleKey.Pause)
            {
                pauseRequested = true;
                return new SourceLine { Line = string.Empty, Original = string.Empty };
            }
            else if (key.Key == ConsoleKey.U && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                buffer.Clear();
            else if (key.Key == ConsoleKey.Backspace)
                buffer.Backspace();
            else if (key.Key == ConsoleKey.Delete)
                buffer.Delete();
            else if (key.Key == ConsoleKey.LeftArrow)
                buffer.MoveLeft();
            else if (key.Key == ConsoleKey.RightArrow)
                buffer.MoveRight();
            else if (key.Key == ConsoleKey.Home)
                buffer.MoveHome();
            else if (key.Key == ConsoleKey.End)
                buffer.MoveEnd();
            else if (!char.IsControl(key.KeyChar))
                buffer.Insert(key.KeyChar);
            else
                continue;

            RedrawLine(buffer, prompt);
        }

        string original = buffer.ToString();
        string line = new(original.Select(Upper).ToArray());
        _history.Add(original);
        return original.Length <= 0
            ? new SourceLine { Line = string.Empty, Original = string.Empty }
            : new SourceLine
            {
                Line = line,
                Original = original
            };
    }

    private void RedrawLine(LineEditorBuffer buffer, string prompt = ">")
    {
        string text = new(buffer.ToString().Select(Upper).ToArray());
        _trs80.Write($"\r{prompt}{text}\x1b[K");
        int charactersToMoveLeft = text.Length - buffer.CursorIndex;
        if (charactersToMoveLeft > 0)
            _trs80.Write(new string('\b', charactersToMoveLeft));
    }

    private char Upper(char key)
    {
        if (!char.IsLetter(key)) return key;
        if (key <= 'Z') return key;
        return (char)((byte)key - 'a' + 'A');
    }
}