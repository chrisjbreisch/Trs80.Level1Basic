using System;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public interface IListStatementDecorator : IStatement
{
    public Type BaseType();
    public IStatement UnDecorate();

    IStatement Enclosing { get; set; }
}