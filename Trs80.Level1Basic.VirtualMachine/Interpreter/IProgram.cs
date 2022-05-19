﻿using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public interface IProgram
{
    void Initialize();
    Statement GetExecutableStatement(int lineNumber);
    LinkedList<Statement> List();
    void Clear();
    void Load(string path);
    void RemoveStatement(Statement statement);
    int Size();
    void ReplaceStatement(Statement line);
    Statement GetFirstStatement();
    IStatement CurrentStatement { get; set; }

}