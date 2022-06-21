using System;
using System.Diagnostics;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public static class ExceptionHandler
{
    public static void HandleError(ITrs80 trs80, Exception ex)
    {
        switch (ex)
        {
            case ScanException se:
                trs80.WriteLine(" 0 WHAT?");
                ScanError(trs80, se);
                break;
            case ParseException pe:
                trs80.WriteLine(" 0 WHAT?");
                ParseError(trs80, pe);
                break;
            case RuntimeExpressionException ree:
                trs80.WriteLine("HOW?");
                RuntimeExpressionError(trs80, ree);
                break;
            case RuntimeStatementException rse:
                trs80.WriteLine("HOW?");
                RuntimeStatementError(trs80, rse);
                break;
            case ValueOutOfRangeException voore:
                trs80.WriteLine("HOW?");
                ValueOutOfRangeError(trs80, voore);
                break;
            default:
                trs80.WriteLine("SORRY");
                if (Debugger.IsAttached)
                {
                    trs80.WriteLine(ex.Message);
                    trs80.WriteLine(ex.StackTrace);
                }
                break;
        }

        WritePrompt(trs80);
    }

    private static void WritePrompt(ITrs80 trs80)
    {
        trs80.WriteLine();
        trs80.WriteLine("READY");
    }

    private static void ValueOutOfRangeError(ITrs80 trs80, ValueOutOfRangeException voore)
    {
        trs80.Error.WriteLine(voore.LineNumber >= 0
            ? $" {voore.LineNumber}  {voore.Statement}?\r\n[{voore.Message}]"
            : $" {voore.Statement}?\r\n[{voore.Message}]");
    }

    private static void ScanError(ITrs80 trs80, ScanException se)
    {
        trs80.Error.WriteLine($"{se.Message}");
    }

    private static void ParseError(ITrs80 trs80, ParseException pe)
    {
        string statement = pe.Statement;
        int linePosition = pe.LinePosition + 1;
        if (linePosition > statement.Length || linePosition <= 0)
            statement = $"{statement}?";
        else
            statement = statement.Insert(pe.LinePosition + 1, "?");
        trs80.Error.WriteLine(pe.LineNumber >= 0
            ? $" {pe.LineNumber}  {statement}\r\n[{pe.Message}]"
            : $" {pe.Statement}?\r\n[{pe.Message}]");
    }

    private static void RuntimeExpressionError(ITrs80 trs80, RuntimeExpressionException ree)
    {
        trs80.Error.WriteLine($"{ree.Message}\n[token {ree.Token}]");
    }

    private static void RuntimeStatementError(ITrs80 trs80, RuntimeStatementException re)
    {
        trs80.Error.WriteLine(re.LineNumber >= 0
            ? $" {re.LineNumber}  {re.Statement}?\r\n[{re.Message}]"
            : $" {re.Statement}?\r\n[{re.Message}]");
    }
}