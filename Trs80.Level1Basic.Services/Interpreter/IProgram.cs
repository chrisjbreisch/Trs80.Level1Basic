using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter;

public interface IProgram
{
    void Initialize();
    Statement GetExecutableStatement(int lineNumber);
    List<ParsedLine> List();
    void Clear();
    void RemoveLine(ParsedLine line);
    int Size();
    void AddLine(ParsedLine line);
    void ReplaceLine(ParsedLine line);
}