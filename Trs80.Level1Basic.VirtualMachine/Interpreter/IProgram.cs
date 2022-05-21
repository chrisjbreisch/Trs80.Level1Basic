using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public interface IProgram
{
    void Initialize();
    IStatement GetExecutableStatement(int lineNumber);
    LineList List();
    void Clear();
    void Load(string path);
    void RemoveStatement(IStatement statement);
    int Size();
    void ReplaceStatement(IStatement statement);
    IStatement GetFirstStatement();
    IStatement CurrentStatement { get; set; }

}