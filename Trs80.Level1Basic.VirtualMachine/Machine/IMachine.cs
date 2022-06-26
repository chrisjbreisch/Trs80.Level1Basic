using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface IMachine
{
    bool ExecutionHalted { get;  }
    DataElements Data { get; }
    IProgram Program { get; }
    int CursorX { get; set; }
    int CursorY { get; set; }

    dynamic Set(string name, dynamic value);
    dynamic Set(string name, int index, dynamic value);
    dynamic Get(string name);
    dynamic Get(string name, int index);
    bool Exists(string name);
    void InitializeProgram();
    void ListProgram(int lineNumber);
    void SaveProgram(string path);
    void LoadProgram(string path);
    void NewProgram();
    void RunStatementList(IStatement statement, IInterpreter interpreter);
    void RunCompoundStatement(CompoundStatementList compound, IInterpreter interpreter);
    void SetNextStatement(IStatement statement);
    void HaltRun();
    IStatement GetStatementByLineNumber(int lineNumber);
    void LoadData(IInterpreter interpreter);
    IStatement GetNextStatement();
    IStatement GetNextStatement(IStatement statement);
    void Initialize();
}