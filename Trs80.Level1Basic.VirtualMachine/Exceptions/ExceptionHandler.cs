using System;
using System.Diagnostics;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Exceptions;

public static class ExceptionHandler
{
    public static void HandleError(ITrs80 trs80, IAppSettings settings, Exception ex,
        PendingEditRequest pendingEdit = null)
    {
        switch (ex)
        {
            case ScanException se:
                trs80.WriteLine("?SN ERROR");
                ScanError(trs80, se, settings.DetailedErrors);
                break;
            case ParseException pe:
                trs80.WriteLine(pe.LineNumber >= 0 ? $"?SN ERROR IN {pe.LineNumber}" : "?SN ERROR");
                BaseError(trs80, pe, settings.DetailedErrors);
                if (pe.LineNumber >= 0)
                    pendingEdit?.Request(pe.LineNumber);
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
                trs80.WriteLine(IsNumericOverflow(voore) ? "?OV ERROR" : "HOW?");
                ValueOutOfRangeError(trs80, voore, settings.DetailedErrors);
                break;
            case ProgramTooLargeException ptle:
                trs80.WriteLine("SORRY");
                BaseError(trs80, ptle, settings.DetailedErrors);
                break;
            case LoopAfterNext lan:
                trs80.WriteLine("WHAT?");
                string statement = lan.Next.SourceLine;
                int linePosition = lan.Next.Identifier.LinePosition;
                if (linePosition > statement.Length || linePosition < 0)
                    statement = $"{statement}?";
                else
                    statement = statement.Insert(linePosition, "?");

                if (lan.Next.LineNumber >= 0)
                    trs80.Error.WriteLine($" {lan.Next.LineNumber}  {statement}");
                if (settings.DetailedErrors)
                    trs80.Error.WriteLine("'NEXT' variable mismatch with 'FOR'.");
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

    private static bool IsNumericOverflow(ValueOutOfRangeException exception)
    {
        return exception.Message.Contains("Integer value out of range", StringComparison.Ordinal)
            || exception.Message.Contains("Single value out of range", StringComparison.Ordinal)
            || exception.Message.Contains("Double value out of range", StringComparison.Ordinal);
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
        int linePosition = be.LinePosition;
        if (linePosition > statement.Length || linePosition < 0)
            statement = $"{statement}?";
        else
            statement = statement.Insert(linePosition, "?");

        if (be.LineNumber >= 0)
            trs80.Error.WriteLine($" {be.LineNumber}  {statement}");
        if (detailedErrors)
            trs80.Error.WriteLine($"[{be.Message}]");
    }
}