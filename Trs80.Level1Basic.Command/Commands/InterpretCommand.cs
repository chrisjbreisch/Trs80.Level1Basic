using System.Diagnostics;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.Command.Commands;

public class InterpretCommand : ICommand<InterpretModel>
{
    private readonly IInterpreter _interpreter;
    private readonly ITrs80 _trs80;

    public InterpretCommand(IInterpreter interpreter, ITrs80 trs80)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
    }

    public void Execute(InterpretModel parameterObject)
    {
        InterpretParsedStatement(parameterObject.Statement);
    }

    private void InterpretParsedStatement(Statement? statement)
    {
        if (statement == null) return;

        _interpreter.Interpret(statement);
    }
}