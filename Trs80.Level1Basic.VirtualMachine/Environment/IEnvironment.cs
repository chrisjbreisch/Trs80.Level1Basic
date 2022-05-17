using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Environment;

public interface IEnvironment
{
    Stack<ForCondition> ForConditions { get; }
    Stack<Statement> ProgramStack { get; }
    DataElements Data { get; }
    IProgram Program { get; }
    int CursorX { get; set;  }
    int CursorY { get; set;  }

    dynamic Assign(string name, dynamic value);
    dynamic Assign(string name, int index, dynamic value);
    dynamic Get(string name);
    dynamic Get(string name, int index);
    List<FunctionDefinition> Function(string name);
    bool Exists(string name);
    void InitializeProgram();
    void ListProgram(int lineNumber);
    void SaveProgram(string path);
    void LoadProgram(string path);
    void NewProgram();
    void RunProgram(Statement statement, IInterpreter interpreter);
    void SetNextStatement(Statement statement);
    void HaltRun();
    Statement GetStatementByLineNumber(int lineNumber);
    void LoadData(IInterpreter interpreter);
    Statement GetNextStatement();
    void Initialize();
}