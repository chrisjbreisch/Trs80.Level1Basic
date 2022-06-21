using System;
using System.Diagnostics;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public static class ExceptionHandler
{
    public static void HandleError(IHost host, Exception ex)
    {
        switch (ex)
        {
            case ScanException se:
                host.WriteLine(" 0 WHAT?");
                ScanError(host, se);
                break;
            case ParseException pe:
                host.WriteLine(" 0 WHAT?");
                ParseError(host, pe);
                break;
            case RuntimeExpressionException ree:
                host.WriteLine("HOW?");
                RuntimeExpressionError(host, ree);
                break;
            case RuntimeStatementException rse:
                host.WriteLine("HOW?");
                RuntimeStatementError(host, rse);
                break;
            case ValueOutOfRangeException voore:
                host.WriteLine("HOW?");
                ValueOutOfRangeError(host, voore);
                break;
            default:
                host.WriteLine("SORRY");
                if (Debugger.IsAttached)
                {
                    host.WriteLine(ex.Message);
                    host.WriteLine(ex.StackTrace);
                }
                break;
        }

        WritePrompt(host);
    }

    private static void WritePrompt(IHost host)
    {
        host.WriteLine();
        host.WriteLine("READY");
    }

    private static void ValueOutOfRangeError(IHost host, ValueOutOfRangeException voore)
    {
        host.Error.WriteLine(voore.LineNumber >= 0
            ? $" {voore.LineNumber}  {voore.Statement}?\r\n[{voore.Message}]"
            : $" {voore.Statement}?\r\n[{voore.Message}]");
    }

    private static void ScanError(IHost host, ScanException se)
    {
        host.Error.WriteLine($"{se.Message}");
    }

    private static void ParseError(IHost host, ParseException pe)
    {
        string statement = pe.Statement;
        int linePosition = pe.LinePosition + 1;
        if (linePosition > statement.Length || linePosition <= 0)
            statement = $"{statement}?";
        else
            statement = statement.Insert(pe.LinePosition + 1, "?");
        host.Error.WriteLine(pe.LineNumber >= 0
            ? $" {pe.LineNumber}  {statement}\r\n[{pe.Message}]"
            : $" {pe.Statement}?\r\n[{pe.Message}]");
    }

    private static void RuntimeExpressionError(IHost host, RuntimeExpressionException ree)
    {
        host.Error.WriteLine($"{ree.Message}\n[token {ree.Token}]");
    }

    private static void RuntimeStatementError(IHost host, RuntimeStatementException re)
    {
        host.Error.WriteLine(re.LineNumber >= 0
            ? $" {re.LineNumber}  {re.Statement}?\r\n[{re.Message}]"
            : $" {re.Statement}?\r\n[{re.Message}]");
    }
}