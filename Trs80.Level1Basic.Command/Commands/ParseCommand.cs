using System.Diagnostics;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Command.Commands;

public class ParseCommand : ICommand<ParseModel>
{
    private readonly IParser _parser;
    private readonly ITrs80 _trs80;

    public ParseCommand(IParser parser, ITrs80 trs80)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
    }

    public void Execute(ParseModel parameterObject)
    {
        parameterObject.Statement = ParseTokens(parameterObject.Tokens)!;
    }

    private void WritePrompt()
    {
        _trs80.WriteLine();
        _trs80.WriteLine("READY");
    }

    private void HandleError(Exception ex)
    {
        switch (ex)
        {
            case ScanException se:
                _trs80.WriteLine(" 0 WHAT?");
                ScanError(se);
                break;
            case ParseException pe:
                _trs80.WriteLine(" 0 WHAT?");
                ParseError(pe);
                break;
            case RuntimeExpressionException ree:
                _trs80.WriteLine("HOW?");
                RuntimeExpressionError(ree);
                break;
            case RuntimeStatementException rse:
                _trs80.WriteLine("HOW?");
                RuntimeStatementError(rse);
                break;
            case ValueOutOfRangeException voore:
                _trs80.WriteLine("HOW?");
                ValueOutOfRangeError(voore);
                break;
            default:
                _trs80.WriteLine("SORRY");
                if (Debugger.IsAttached)
                {
                    _trs80.WriteLine(ex.Message);
                    _trs80.WriteLine(ex.StackTrace);
                }
                break;
        }
    }

    private void ValueOutOfRangeError(ValueOutOfRangeException voore)
    {
        _trs80.Error.WriteLine(voore.LineNumber >= 0
            ? $" {voore.LineNumber}  {voore.Statement}?\r\n[{voore.Message}]"
            : $" {voore.Statement}?\r\n[{voore.Message}]");
    }

    private IStatement? ParseTokens(List<Token>? tokens)
    {
        if (tokens == null) return null;

        IStatement? statement = null;
        try
        {
            statement = _parser.Parse(tokens);
        }
        catch (Exception ex)
        {
            HandleError(ex);
            WritePrompt();
        }
        return statement;
    }

    private void ScanError(ScanException se)
    {
        _trs80.Error.WriteLine($"{se.Message}");
    }

    private void ParseError(ParseException pe)
    {
        string statement = pe.Statement;
        int linePosition = pe.LinePosition + 1;
        if (linePosition > statement.Length || linePosition <= 0)
            statement = $"{statement}?";
        else
            statement = statement.Insert(pe.LinePosition + 1, "?");
        _trs80.Error.WriteLine(pe.LineNumber >= 0
            ? $" {pe.LineNumber}  {statement}\r\n[{pe.Message}]"
            : $" {pe.Statement}?\r\n[{pe.Message}]");
    }

    private void RuntimeExpressionError(RuntimeExpressionException ree)
    {
        _trs80.Error.WriteLine($"{ree.Message}\n[token {ree.Token}]");
    }

    public void RuntimeStatementError(RuntimeStatementException re)
    {
        _trs80.Error.WriteLine(re.LineNumber >= 0
            ? $" {re.LineNumber}  {re.Statement}?\r\n[{re.Message}]"
            : $" {re.Statement}?\r\n[{re.Message}]");
    }
}