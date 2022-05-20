//
//
// This file was automatically generated by generateAst
// at 2022-05-19 6:42:09 PM UTC. Do not modify.
//
//

using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public interface IStatement
{
    int LineNumber { get; set; }
    string SourceLine { get; set; }
    IStatement Next { get; set; }
    IStatement Previous { get; set; }
    IStatement Parent { get; set; }

    T Accept<T>(IVisitor<T> visitor);
}