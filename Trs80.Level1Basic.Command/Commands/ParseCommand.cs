using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ParseCommand : ICommand<ParseModel>
{
    private readonly IParser _parser;

    public ParseCommand(IParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public void Execute(ParseModel parameterObject)
    {
        parameterObject.Statement = ParseTokens(parameterObject.Tokens)!;
    }

    private IStatement? ParseTokens(List<Token>? tokens)
    {
        return tokens == null ? null : _parser.Parse(tokens);
    }
}