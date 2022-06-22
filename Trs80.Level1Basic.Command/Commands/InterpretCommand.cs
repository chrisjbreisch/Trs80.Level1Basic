using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.Command.Commands;

// ReSharper disable once UnusedMember.Global
public class InterpretCommand : ICommand<InterpretModel>
{
    private readonly IInterpreter _interpreter;

    public InterpretCommand(IInterpreter interpreter)
    {
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
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