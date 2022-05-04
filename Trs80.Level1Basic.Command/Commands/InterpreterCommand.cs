using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.Interpreter.Exceptions;
using Trs80.Level1Basic.Interpreter.Interpreter;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class InterpreterCommand : ICommand<InterpreterModel>
{
    private readonly IScanner _scanner;
    private readonly IParser _parser;
    private readonly IBasicInterpreter _interpreter;
    private readonly IConsole _console;

    public InterpreterCommand(IScanner scanner, IParser parser,
        IBasicInterpreter interpreter, IConsole console)
    {
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Execute(InterpreterModel parameterObject)
    {
        bool done = false;
        while (!done)
        {
            _console.Write(">");
            string inputLine = GetInputLine();
            done = ExecuteInput(inputLine);
        }
    }

    private void WritePrompt()
    {
        _console.WriteLine();
        _console.WriteLine("READY");
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

    private bool ExecuteInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;
        if (input.ToLower() == "exit") return true;

        List<Token>? tokens = ScanLine(input);
        ParsedLine? parsedLine = ParseTokens(tokens);
        InterpretParsedLine(parsedLine);

        return false;
    }

    private void InterpretParsedLine(ParsedLine? parsedLine)
    {
        if (parsedLine == null) return;

        try
        {
            _interpreter.Interpret(parsedLine);
        }
        catch (Exception ex)
        {
            HandleError(ex);
            WritePrompt();
        }
    }

    private void HandleError(Exception ex)
    {
        switch (ex)
        {
            case ScanException se:
                _console.WriteLine("WHAT?");
                ScanError(se);
                break;
            case ParseException pe:
                _console.WriteLine("WHAT?");
                ParseError(pe);
                break;
            case RuntimeExpressionException ree:
                _console.WriteLine("HOW?");
                RuntimeExpressionError(ree);
                break;
            case RuntimeStatementException rse:
                _console.WriteLine("HOW?");
                RuntimeStatementError(rse);
                break;
            case ValueOutOfRangeException voore:
                _console.WriteLine("HOW?");
                ValueOutOfRangeError(voore);
                break;
            default:
                _console.WriteLine("SORRY");
                if (Debugger.IsAttached)
                {
                    _console.WriteLine(ex.Message);
                    _console.WriteLine(ex.StackTrace);
                }
                break;
        }
    }

    private void ValueOutOfRangeError(ValueOutOfRangeException voore)
    {
        _console.Error.WriteLine(voore.LineNumber > 0
            ? $" {voore.LineNumber}  {voore.Statement}?\r\n[{voore.Message}]"
            : $" {voore.Statement}?\r\n[{voore.Message}]");
    }

    private ParsedLine? ParseTokens(List<Token>? tokens)
    {
        if (tokens == null) return null;

        ParsedLine? parsedLine = null;
        try
        {
            parsedLine = _parser.Parse(tokens);
        }
        catch (Exception ex)
        {
            HandleError(ex);
            WritePrompt();
        }
        return parsedLine;
    }

    private List<Token>? ScanLine(string sourceLine)
    {
        List<Token>? tokens = null;
        try
        {
            tokens = _scanner.ScanTokens(sourceLine);
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }

        return tokens;
    }

    private void ScanError(ScanException se)
    {
        _console.Error.WriteLine($"{se.Message}");
    }

    private void ParseError(ParseException pe)
    {
        _console.Error.WriteLine(pe.LineNumber > 0
            ? $" {pe.LineNumber}  {pe.Statement}?\r\n[{pe.Message}]"
            : $" {pe.Statement}?\r\n[{pe.Message}]");
    }

    private void RuntimeExpressionError(RuntimeExpressionException ree)
    {
        _console.Error.WriteLine($"{ree.Message}\n[token {ree.Token}]");
    }

    public void RuntimeStatementError(RuntimeStatementException re)
    {
        _console.Error.WriteLine(re.LineNumber > 0
            ? $" {re.LineNumber}  {re.Statement}?\r\n[{re.Message}]"
            : $" {re.Statement}?\r\n[{re.Message}]");
    }
}