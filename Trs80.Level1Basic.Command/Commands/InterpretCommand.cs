using System.Diagnostics;
using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.Interpreter.Exceptions;
using Trs80.Level1Basic.Interpreter.Interpreter;
using Trs80.Level1Basic.Interpreter.Parser;

namespace Trs80.Level1Basic.Command.Commands;

public class InterpretCommand : ICommand<InterpretModel>
{
    private readonly IBasicInterpreter _interpreter;
    private readonly IConsole _console;

    public InterpretCommand(IBasicInterpreter interpreter, IConsole console)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Execute(InterpretModel parameterObject)
    {
        InterpretParsedLine(parameterObject.ParsedLine);
    }

    private void WritePrompt()
    {
        _console.WriteLine();
        _console.WriteLine("READY");
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