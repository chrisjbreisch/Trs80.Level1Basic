using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Parser;

public interface IProgram
{
    void Initialize();
    Statement GetStatement(int lineNumber);
    List<ParsedLine> List();
    void Clear();
    void RemoveLine(ParsedLine line);
    int Size();
    void AddLine(ParsedLine line);
    void ReplaceLine(ParsedLine line);
    void Sort();
}