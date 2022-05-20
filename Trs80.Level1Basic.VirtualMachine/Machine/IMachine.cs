using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface IMachine
{
    Stack<ForCondition> ForConditions { get; }
    DataElements Data { get; }
    IProgram Program { get; }
    int CursorX { get; set;  }
    int CursorY { get; set;  }
    public bool ExecutionHalted { get; set; }

    dynamic Assign(string name, dynamic value);
    dynamic Assign(string name, int index, dynamic value);
    dynamic Get(string name);
    dynamic Get(string name, int index);
    List<Callable> Function(string name);
    bool Exists(string name);
    void InitializeProgram();
    void ListProgram(int lineNumber);
    void SaveProgram(string path);
    void LoadProgram(string path);
    void NewProgram();
    void RunStatementList(IStatement statement, IInterpreter interpreter, bool breakOnLineChange);
    void SetNextStatement(IStatement statement);
    void HaltRun();
    IStatement GetStatementByLineNumber(int lineNumber);
    void LoadData(IInterpreter interpreter);
    IStatement GetNextStatement();
    IStatement GetNextStatement(IStatement statement);
    void Initialize();
}