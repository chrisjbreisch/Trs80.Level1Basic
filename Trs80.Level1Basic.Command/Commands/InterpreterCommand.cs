using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Exceptions;
using Trs80.Level1Basic.Services;
using Trs80.Level1Basic.Services.Interpreter;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class InterpreterCommand : ICommand<InterpreterModel>
{
    private readonly ILogger _logger;
    private readonly IBasicInterpreter _interpreter;
    private readonly ITrs80Console _console;
    private readonly ConsoleFont _originalConsoleFont;
    public InterpreterCommand(ILoggerFactory logFactory, IBasicInterpreter interpreter, ITrs80Console console)
    {
        _logger = logFactory.CreateLogger<InterpreterCommand>();
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _originalConsoleFont = _console.GetCurrentFont();
    }

    public void Execute(InterpreterModel parameterObject)
    {
        InitializeWindow();

        _console.WriteLine("Enter TRS-80 LEVEL 1 BASIC Commands or Type EXIT to Exit.");
        WritePrompt();
        
        while (true)
        {
            _console.Write(">");

            string inputLine = GetInputLine();

            if (string.IsNullOrEmpty(inputLine)) continue;
            if (inputLine.ToLower() == "exit") break;

            ExecuteLine(inputLine);
        }

        _console.SetCurrentFont(_originalConsoleFont);
    }


    private void InitializeWindow()
    {
        _console.SetCurrentFont(new ConsoleFont { FontName = "Another Mans Treasure MIB 64C 2X3Y", FontSize = 36 });
        _console.DisableCursorBlink();
        _console.SetWindowSize(64, 16);
        _console.SetBufferSize(64, 160);
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
            var key = _console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                _console.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                _console.Write(" \b");
                input[charCount--] = '\0';
                continue;
            }

            input[charCount++] = key.KeyChar;
        }

        return charCount <= 0 ? string.Empty : new string(input, 0, charCount);
    }

    private void ExecuteLine(string inputLine)
    {
        bool errorOccurred = true;
        try
        {
            _interpreter.Interpret(inputLine);
            errorOccurred = false;
        }
        catch (ScanException se)
        {
            _console.WriteLine("WHAT?");
            ScanError(se);
        }
        catch (ParseException pe)
        {
            _console.WriteLine("WHAT?");
            ParseError(pe);
        }
        catch (RuntimeExpressionException ree)
        {
            _console.WriteLine("HOW?");
            RuntimeExpressionError(ree);
        }
        catch (RuntimeStatementException rse)
        {
            _console.WriteLine("HOW?");
            RuntimeStatementError(rse);
        }
        catch (ValueOutOfRangeException)
        {
            _console.WriteLine("HOW?");
            //RuntimeStatementError(rse);
        }
        catch (Exception ex)
        {
            _console.WriteLine("SORRY");
            if (Debugger.IsAttached)
            {
                _console.WriteLine(ex.Message);
                _console.WriteLine(ex.StackTrace);
            }
        }
        if (errorOccurred)
            WritePrompt();
    }

    private void ScanError(ScanException se)
    {
        _console.Error.WriteLine($"{se.Message}");
    }

    private void ParseError(ParseException pe)
    {
        _console.Error.WriteLine(pe.LineNumber > 0
            ? $" {pe.LineNumber}  {pe.Statement}?\r\n[{pe.Message}]"
            : $" {pe.Statement}\r\n[{pe.Message}]");
    }

    private void RuntimeExpressionError(RuntimeExpressionException ree)
    {
        _console.Error.WriteLine($"{ree.Message}\n[token {ree.Token}]");
    }

    public void RuntimeStatementError(RuntimeStatementException re)
    {
        _console.Error.WriteLine(re.LineNumber > 0
            ? $" {re.LineNumber}  {re.Statement}?\r\n[{re.Message}]"
            : $" {re.Statement}\r\n[{re.Message}]");
    }
}