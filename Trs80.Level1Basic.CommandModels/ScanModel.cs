using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public class ScanModel
{
    public SourceLine SourceLine { get; set; } = new();
    public List<Token> Tokens { get; set; } = new();
}