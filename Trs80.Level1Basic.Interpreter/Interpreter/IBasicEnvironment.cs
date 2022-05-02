﻿using System.Collections.Generic;
using Trs80.Level1Basic.Interpreter.Parser.Statements;

namespace Trs80.Level1Basic.Interpreter.Interpreter;

public interface IBasicEnvironment
{
    Stack<ForCheckCondition> ForChecks { get; }
    Stack<Statement> ProgramStack { get; }
    DataElements Data { get; }
    IProgram Program { get; }
    dynamic AssignVariable(string name, dynamic value);
    dynamic AssignArray(string name, int index, dynamic value);
    dynamic GetVariable(string name);
    bool VariableExists(string name);
    FunctionDefinition GetFunctionDefinition(string name);
    void InitializeProgram();
    void ListProgram(int lineNumber);
    void SaveProgram(string path);
    void LoadProgram(string path);
    void NewProgram();
    void RunProgram(Statement statement, IBasicInterpreter interpreter);
    void SetNextStatement(Statement statement);
    void HaltRun();
    int MemoryInUse();
    Statement GetStatementByLineNumber(int lineNumber);
    void LoadData(IBasicInterpreter interpreter);
    Statement GetNextStatement();
    void Initialize();
    dynamic GetArrayValue(string name, int index);
}