using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public interface IEnvironment
{
    Stack<ForCheckCondition> ForChecks { get; }
    Stack<Statement> ProgramStack { get; }
    DataElements Data { get; }
    IProgram Program { get; }
    dynamic Assign(string name, dynamic value);
    dynamic Assign(string name, int index, dynamic value);
    dynamic Get(string name);
    bool Exists(string name);
    List<FunctionDefinition> GetFunctionDefinition(string name);
    void InitializeProgram();
    void ListProgram(int lineNumber);
    void SaveProgram(string path);
    void LoadProgram(string path);
    void NewProgram();
    void RunProgram(Statement statement, IInterpreter interpreter);
    void SetNextStatement(Statement statement);
    void HaltRun();
    int MemoryInUse();
    Statement GetStatementByLineNumber(int lineNumber);
    void LoadData(IInterpreter interpreter);
    Statement GetNextStatement();
    void Initialize();
    dynamic GetArrayValue(string name, int index);
}