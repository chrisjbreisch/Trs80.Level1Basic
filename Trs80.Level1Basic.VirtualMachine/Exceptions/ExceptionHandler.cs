using System;
using System.Diagnostics;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public static class ExceptionHandler
{
    public static void HandleError(ITrs80 trs80, IAppSettings settings, Exception ex)
    {

        switch (ex)
        {
            case ScanException se:
                trs80.WriteLine("WHAT?");
                ScanError(trs80, se, settings.DetailedErrors);
                break;
            case ParseException pe:
                trs80.WriteLine("WHAT?");
                BaseError(trs80, pe, settings.DetailedErrors);
                break;
            case RuntimeExpressionException ree:
                trs80.WriteLine("WHAT?");
                BaseError(trs80, ree, settings.DetailedErrors);
                break;
            case RuntimeStatementException rse:
                trs80.WriteLine("HOW?");
                BaseError(trs80, rse, settings.DetailedErrors);
                break;
            case ValueOutOfRangeException voore:
                trs80.WriteLine("HOW?");
                ValueOutOfRangeError(trs80, voore, settings.DetailedErrors);
                break;
            case ProgramTooLargeException ptle:
                trs80.WriteLine("SORRY");
                BaseError(trs80, ptle, settings.DetailedErrors);
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

    private static void ValueOutOfRangeError(ITrs80 trs80, ValueOutOfRangeException voore, bool detailedErrors)
    {
        if (voore.LineNumber >= 0)
            trs80.Error.WriteLine($" {voore.LineNumber}  {voore.Statement}?");
        if (detailedErrors)
            trs80.Error.WriteLine($"[{voore.Message}]");
    }

    private static void ScanError(ITrs80 trs80, ScanException se, bool detailedErrors)
    {
        if (detailedErrors)
            trs80.Error.WriteLine($"[{se.Message}]");
    }

    private static void BaseError(ITrs80 trs80, BaseException be, bool detailedErrors)
    {
        string statement = be.Statement;
        int linePosition = be.LinePosition + 1;
        if (linePosition > statement.Length || linePosition < 0)
            statement = $"{statement}?";
        else
            statement = statement.Insert(be.LinePosition, "?");

        if (be.LineNumber >= 0)
            trs80.Error.WriteLine($" {be.LineNumber}  {statement}");
        if (detailedErrors)
            trs80.Error.WriteLine($"[{be.Message}]");
    }
}