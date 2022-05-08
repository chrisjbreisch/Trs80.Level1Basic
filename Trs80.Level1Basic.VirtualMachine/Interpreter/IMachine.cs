using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public interface IMachine
{
    Stack<ForCheckCondition> ForChecks { get; }
    Stack<Statement> ProgramStack { get; }
    DataElements Data { get; }
    IProgram Program { get; }
    dynamic AssignVariable(string name, dynamic value);
    dynamic AssignArray(string name, int index, dynamic value);
    dynamic GetVariable(string name);
    bool VariableExists(string name);
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