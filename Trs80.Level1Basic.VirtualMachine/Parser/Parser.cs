using System;
using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

using Array = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Array;

namespace Trs80.Level1Basic.VirtualMachine.Parser;

public interface IParser
{
    IStatement Parse(List<Token> tokens);
}

public class Parser : IParser
{
    private List<Token> _tokens;
    private int _current;
    private int _lineNumber;
    private string _source;
    private readonly INativeFunctions _natives;
    private readonly Callable _padQuadrant;

    public Parser(INativeFunctions natives)
    {
        _natives = natives ?? throw new ArgumentNullException(nameof(natives));
        _padQuadrant = _natives.Get("_padquadrant").First();
    }

    public IStatement Parse(List<Token> tokens)
    {
        if (tokens == null) return null;

        Initialize();
        _tokens = tokens;

        return IsAtEnd() ? null : Line();
    }

    private IStatement Line()
    {
        Token lineNumber = Peek();

        _lineNumber = GetLineNumberValue(lineNumber);

        if (_lineNumber == -1 && char.IsLetter(lineNumber.SourceLine[0]))
            _source = lineNumber.SourceLine;
        else
        {
            int lineNumberLength = _lineNumber.ToString().Length;
            _source = lineNumber.SourceLine[lineNumberLength..].TrimStart(' ');
        }

        if (lineNumber.Type != TokenType.Number) return Compound();

        if (PeekNext().Type == TokenType.EndOfLine)
            return DeleteStatement();

        Advance();
        return Compound();
    }

    private void Initialize()
    {
        _tokens = null;
        _current = 0;
    }

    private IStatement Compound()
    {
        var compound = new Compound(new CompoundStatementList());
        compound.Statements.Enclosing = compound;
        do
        {
            IStatement current = Statement();
            compound.Statements.Add(current);

        } while (Match(TokenType.Colon));

        if (compound.Statements.Count != 1) return StatementWrapper(compound);

        var statement = (IListStatementDecorator)compound.Statements[0];
        return StatementWrapper(statement.UnDecorate());
    }

    private IStatement Statement()
    {
        if (Match(TokenType.Cls))
            return ClsStatement();
        if (Match(TokenType.Cont))
            return ContStatement();
        if (Match(TokenType.Data))
            return DataStatement();
        if (Match(TokenType.End))
            return EndStatement();
        if (Match(TokenType.For))
            return ForStatement();
        if (Match(TokenType.Gosub))
            return GosubStatement();
        if (Match(TokenType.Goto))
            return GotoStatement();
        if (Match(TokenType.If))
            return IfStatement();
        if (Match(TokenType.Input))
            return InputStatement();
        if (Match(TokenType.Let))
            return LetStatement();
        if (Match(TokenType.List))
            return ListStatement();
        if (Match(TokenType.Load))
            return LoadStatement();
        if (Match(TokenType.Merge))
            return MergeStatement();
        if (Match(TokenType.N))
            return _lineNumber >= 0 ? NextStatement() : NewStatement();
        if (Match(TokenType.New))
            return NewStatement();
        if (Match(TokenType.Next))
            return NextStatement();
        if (Match(TokenType.On))
            return OnStatement();
        if (Match(TokenType.Print))
            return PrintStatement();
        if (Match(TokenType.Read))
            return ReadStatement();
        if (Match(TokenType.Rem))
            return RemarkStatement();
        if (Match(TokenType.Restore))
            return RestoreStatement();
        if (Match(TokenType.Return))
            return ReturnStatement();
        if (Match(TokenType.Run))
            return RunStatement();
        if (Match(TokenType.Save))
            return SaveStatement();
        if (Match(TokenType.Stop))
            return StopStatement();
        if (Peek().Type != TokenType.R || PeekNext().Type == TokenType.LeftParen)
            return Peek().Type == TokenType.Identifier ? LetStatement() : ExpressionStatement();

        Advance();
        return RunStatement();
    }

    private IStatement StopStatement()
    {
        return StatementWrapper(new Stop());
    }

    private IStatement RestoreStatement()
    {
        return StatementWrapper(new Restore());
    }

    private IStatement ReadStatement()
    {
        var variables = new List<Expression>();
        do
        {
            if (Peek().Type != TokenType.Identifier)
                throw new ParseException(_lineNumber, _source, "Expected variable after 'READ'.");

            variables.Add(Expression());
        } while (Match(TokenType.Comma));

        return StatementWrapper(new Read(variables));
    }

    private IStatement DataStatement()
    {
        var elements = new List<Expression>();
        do
            elements.Add(Expression());
        while (Match(TokenType.Comma));

        return StatementWrapper(new Data(elements));
    }

    private IStatement ReturnStatement()
    {
        return StatementWrapper(new Return());
    }

    private IStatement GosubStatement()
    {
        Expression location = Expression();
        return StatementWrapper(new Gosub(location));
    }

    private IStatement OnStatement()
    {
        Expression selector = Expression();
        bool isGosub = false;
        var locations = new List<Expression>();

        if (Match(TokenType.Goto))
        {
            do
                locations.Add(Expression());
            while (Match(TokenType.Comma));

            while (!IsAtEnd())
                Advance();
        }
        else if (Match(TokenType.Gosub))
        {
            isGosub = true;
            do
                locations.Add(Expression());
            while (Match(TokenType.Comma));

            while (!IsAtEnd())
                Advance();
        }
        else
            throw new ParseException(_lineNumber, _source,
                "Expected 'GOTO' or 'GOSUB' after 'ON'");

        return StatementWrapper(new On(selector, locations, isGosub));
    }

    private IStatement StatementWrapper(IStatement statement)
    {
        statement.LineNumber = _lineNumber;
        statement.SourceLine = _source;

        return statement;
    }

    private IStatement ContStatement()
    {
        return StatementWrapper(new Cont());
    }

    private IStatement GotoStatement()
    {
        Expression location = Expression();
        return StatementWrapper(new Goto(location));
    }

    private IStatement ClsStatement()
    {
        return StatementWrapper(new Cls());
    }

    private IStatement NextStatement()
    {
        if (Peek().Type != TokenType.Identifier)
            throw new ParseException(_lineNumber, _source,
                "Expected variable name after 'NEXT'.");

        Expression identifier = Identifier();

        return StatementWrapper(new Next(identifier));
    }

    private IStatement ForStatement()
    {
        Expression identifier = Identifier();

        Consume(TokenType.Equal, "Expected assignment.");

        Expression startValue = Expression();
        Consume(TokenType.To, "Expected TO");
        Expression endValue = Expression();

        Expression stepValue;
        if (Peek().Type == TokenType.Step)
        {
            Advance();
            stepValue = Expression();
        }
        else
            stepValue = new Literal(1);

        return StatementWrapper(new For(identifier, startValue, endValue, stepValue));
    }

    private IStatement InputStatement()
    {
        bool newline = true;
        var values = new List<Expression>();

        while (!IsAtStatementEnd())
        {
            values.Add(Expression());

            Match(TokenType.Semicolon);

            if (Match(TokenType.Comma))
                values.Add(
                    new Call(
                        new Token(
                            TokenType.Identifier,
                            "_padquadrant",
                            "_padquadrant",
                            null
                        ),
                        _padQuadrant,
                        new List<Expression>()));

            if (IsAtStatementEnd() && values[^1] is Identifier)
                newline = false;
        }

        return StatementWrapper(new Input(values, newline));
    }

    private IStatement NewStatement()
    {
        return StatementWrapper(new New());
    }

    private IStatement DeleteStatement()
    {
        Advance();
        return new Delete(_lineNumber);
    }

    private IStatement EndStatement()
    {
        return StatementWrapper(new End());
    }

    private IStatement IfStatement()
    {
        Expression condition = Expression();
        Token current = Peek();
        if (!Match(TokenType.Then, TokenType.Goto, TokenType.T, TokenType.Gosub) && Peek().Type == TokenType.Number)
            throw new ParseException(_lineNumber, _source,
                "Expected 'THEN' or 'GOTO' before line number in 'IF' statement.");

        var thenBranch = new CompoundStatementList {
            StatementWrapper(
            current.Type switch
            {
                TokenType.Gosub => new Gosub(Expression()),
                TokenType.Goto => new Goto(Expression()),
                _ => Peek().Type == TokenType.Number ? new Goto(Expression()) : Statement()
            })
        };

        while (Match(TokenType.Colon))
            thenBranch.Add(Statement());

        return StatementWrapper(new If(condition, thenBranch));
    }

    private IStatement SaveStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty);

        return new Save(path);
    }

    private IStatement LoadStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty);
        return new Load(path);
    }

    private IStatement MergeStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty);
        return new Merge(path);
    }
    private IStatement RemarkStatement()
    {
        var value = new Literal(Previous().Literal);
        return StatementWrapper(new Rem(value));
    }

    private IStatement ListStatement()
    {
        Expression value = !IsAtEnd() ? Expression() : new Literal(0);
        return new List(value);
    }

    private IStatement RunStatement()
    {
        Expression value = !IsAtEnd() ? Expression() : new Literal(-1);
        return new Run(value);
    }

    private IStatement ExpressionStatement()
    {
        Expression expression = Expression();
        return new StatementExpression(expression);
    }

    private IStatement LetStatement()
    {
        Token peek = Peek();
        Token peekNext = PeekNext();

        if (peek.Type != TokenType.Identifier)
            throw new ParseException(_lineNumber, _source,
                "Expected variable name or function call.");

        if (peekNext.Type != TokenType.Equal && _natives.Get(peek.Lexeme) != null) return StatementWrapper(new StatementExpression(Call()));

        Expression identifier = Identifier();

        Consume(TokenType.Equal, "Expected assignment.");

        Expression value = Expression();
        if (value is not Identifier unquoted) return StatementWrapper(new Let(identifier, value));

        string lexeme = unquoted.Name.Lexeme;
        if (lexeme.Length > 1 &&
            (!lexeme.EndsWith('$') || lexeme.Length > 2))
            value = new Literal(lexeme);

        return StatementWrapper(new Let(identifier, value));
    }

    private Expression Identifier()
    {
        Token peek = Peek();

        if (peek.Type != TokenType.Identifier)
            throw new ParseException(_lineNumber, _source,
                "Expected variable name or function call.");

        Advance();

        if (!Match(TokenType.LeftParen))
            return new Identifier(peek, peek.Lexeme.EndsWith('$'), peek.Lexeme.ToLowerInvariant());

        Expression index = Expression();
        Consume(TokenType.RightParen, "Expected ')' after array index");
        return new Array(peek, index, peek.Lexeme.ToLowerInvariant());
    }

    private bool IsAtStatementEnd()
    {
        return IsAtEnd() || Peek().Type == TokenType.Colon;
    }

    private IStatement PrintStatement()
    {
        bool newline = true;
        var values = new List<Expression>();
        Expression atPosition = null;

        Token current = Peek();
        Token next = PeekNext();
        if (current.Type != TokenType.EndOfLine && next.Type != TokenType.EndOfLine)
#pragma warning disable S1066 // Collapsible "if" statements should be merged
            if (current.Type == TokenType.At || (current.Type == TokenType.A && next.Type != TokenType.LeftParen))
            {
                Advance();
                atPosition = Expression();
                if (!Match(TokenType.Comma, TokenType.Semicolon))
                    throw new ParseException(_lineNumber, _source, "Expected ',' or ';' after AT clause.");
            }
#pragma warning restore S1066 // Collapsible "if" statements should be merged

        while (!IsAtStatementEnd())
        {
            values.Add(Expression());

            if (Match(TokenType.Semicolon) && IsAtStatementEnd())
                newline = false;
            if (Match(TokenType.Comma))
                values.Add(
                    new Call(
                        new Token(
                            TokenType.Identifier,
                            "_padquadrant",
                            "_padquadrant",
                            null
                        ),
                        _padQuadrant,
                        new List<Expression>()));

        }

        return StatementWrapper(new Print(atPosition, values, newline));
    }

    private int GetLineNumberValue(Token lineNumber)
    {
        dynamic line = lineNumber.Literal;

        if (line == null) return -1;

        if (line is not int)
            throw new ParseException(-1, lineNumber.SourceLine, $"Invalid text at {line}");
        if (line > short.MaxValue)
            throw new ParseException(_lineNumber, _source, $"Line number cannot exceed {short.MaxValue}.");

        return line;
    }

    private Expression Expression()
    {
        return Comparison();
    }

    private Expression Comparison()
    {
        Expression left = Term();
        while (Match(TokenType.LessThanOrEqual, TokenType.LessThan,
                   TokenType.GreaterThanOrEqual, TokenType.GreaterThan,
                   TokenType.NotEqual, TokenType.Equal))
        {
            Token operatorType = Previous();
            Expression right = Term();
            left = new Binary(left, operatorType, right);
        }

        return left;
    }

    private Expression Term()
    {
        Expression left = Factor();
        while (Match(TokenType.Minus, TokenType.Plus))
        {
            Token operatorType = Previous();
            Expression right = Factor();
            left = new Binary(left, operatorType, right);
        }

        return left;
    }

    private Expression Factor()
    {
        Expression left = Unary();
        while (Match(TokenType.Slash, TokenType.Star))
        {
            Token operatorType = Previous();
            Expression right = Unary();
            left = new Binary(left, operatorType, right);
        }

        return left;
    }

    private Expression Unary()
    {
        if (!Match(TokenType.Minus)) return Call();

        Token operatorType = Previous();
        Expression right = Unary();
        return new Unary(operatorType, right);
    }

    private Expression Call()
    {
        Token name = Peek();

        Expression expression = Primary();
        List<Callable> callees = _natives.Get(name.Lexeme);

        if (Match(TokenType.LeftParen) && expression is Identifier)
            return callees != null ? FinishCall(name, callees) : FinishArray(name);

        Token previous = Previous();
        if (previous.Type != TokenType.Identifier) return expression;

        callees = _natives.Get(previous.Lexeme);
        if (callees == null) return expression;

        Callable callee = callees.FirstOrDefault(f => f.Arity == 0);

        if (callee != null)
            return new Call(name, callee, new List<Expression>());

        throw new ParseException(_lineNumber, _source,
            $"Invalid number of arguments passed to function '{previous.Lexeme}'");
    }

    private Expression FinishCall(Token name, List<Callable> callees)
    {
        var arguments = new List<Expression>();

        if (!Check(TokenType.RightParen))
            do
                arguments.Add(Expression());
            while (Match(TokenType.Comma));

        Consume(TokenType.RightParen, "Expected ')' after arguments");

        Callable callee = callees.FirstOrDefault(f => f.Arity == arguments.Count);

        if (callee == null)
            throw new ParseException(_lineNumber, _source,
                $"Unknown function '{name.Lexeme}' with argument count {arguments.Count}");


        return new Call(name, callee, arguments);
    }

    private Expression FinishArray(Token name)
    {
        Expression index = Expression();

        Consume(TokenType.RightParen,
            "Expected ')' after arguments");

        return new Array(name, index, name.Lexeme.ToLowerInvariant());
    }

    private Expression Primary()
    {
        if (Match(TokenType.Number, TokenType.String))
            return new Literal(Previous().Literal);

        if (Match(TokenType.LeftParen))
        {
            Expression expression = Expression();
            Consume(TokenType.RightParen, "Expected ')' after expression.");
            return new Grouping(expression);
        }

        if (Match(TokenType.Identifier))
        {
            Token previous = Previous();
            return new Identifier(previous, previous.Lexeme.EndsWith('$'), previous.Lexeme.ToLowerInvariant());
        }

        if (!IsIdentifierShortHand())
            throw new ParseException(_lineNumber, _source,
                "Expected expression.");

        Token current = Peek();
        Advance();
        var identifier = new Token(TokenType.Identifier, current.Lexeme, current.Lexeme, _source);
        return new Identifier(identifier, current.Lexeme.EndsWith('$'), current.Lexeme.ToLowerInvariant());
    }

    private bool IsIdentifierShortHand()
    {
        Token token = Peek();

        if (token == null) return false;
        if (token.Type == TokenType.EndOfLine) return false;

        Callable function = _natives.Get(token.Lexeme).FirstOrDefault();

        return function != null;
    }

    private void Consume(TokenType type, string message)
    {
        if (!Check(type)) throw new ParseException(_lineNumber, _source, message);

        Advance();
    }

    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Check)) return false;
        Advance();
        return true;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private void Advance()
    {
        if (!IsAtEnd())
            _current++;

        Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EndOfLine;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token PeekNext()
    {
        return IsAtEnd() ? _tokens[_current] : _tokens[_current + 1];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }
}