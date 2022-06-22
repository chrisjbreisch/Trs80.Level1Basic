using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser;

public interface IParser
{
    IStatement Parse(List<Token> tokens);
}