using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Command.Commands;

public class ScanCommand : ICommand<ScanModel>
{
    private readonly IScanner _scanner;

    public ScanCommand(IScanner scanner)
    {
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
    }

    public void Execute(ScanModel parameterObject)
    {
        parameterObject.Tokens = ScanLine(parameterObject.SourceLine)!;
    }

    private List<Token>? ScanLine(SourceLine sourceLine)
    {
        return _scanner.ScanTokens(sourceLine);
    }
}