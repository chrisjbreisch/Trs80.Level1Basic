using System.Collections.Generic;
using Trs80.Level1Basic.Common;

namespace Trs80.Level1Basic.VirtualMachine.Scanner;

public interface IScanner
{
    List<Token> ScanTokens(SourceLine source);
}