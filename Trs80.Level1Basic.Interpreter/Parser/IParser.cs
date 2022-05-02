using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Parser;

public interface IParser
{
    ParsedLine Parse(List<Token> tokens);
}