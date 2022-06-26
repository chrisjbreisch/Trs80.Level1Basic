using System;
using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser;

public class Parser : IParser
{
    private List<Token> _tokens;
    private int _current;
    private int _lineNumber;
    private string _source;
    private readonly ITrs80 _trs80;
    private readonly INativeFunctions _natives;
    private readonly Callable _padQuadrant;
    private readonly IAppSettings _appSettings;
    private ParseException _parseException = null;

    public Parser(ITrs80 trs80, INativeFunctions natives, IAppSettings appSettings)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _natives = natives ?? throw new ArgumentNullException(nameof(natives));
        _padQuadrant = _natives.Get("_pad_quadrant").First();
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public IStatement Parse(List<Token> tokens)
    {
        if (tokens == null) return null;
        Initialize();
        _tokens = tokens;
        try
        {
            return IsAtEnd() ? null : Line();
        }
        catch (Exception ex)
        {
            ExceptionHandler.HandleError(_trs80, _appSettings, ex);
            return null;
        }
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
        var compound = new Compound(new CompoundStatementList(Peek().LinePosition));
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
                _parseException = new ParseException(_lineNumber, _source, 
                    Peek().LinePosition, "Expected variable after 'READ'.");

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
        return StatementWrapper(new Gosub(location, Peek().LinePosition));
    }

    private IStatement OnStatement()
    {
        var selector = new Selector(Expression(), Peek().LinePosition);
        bool isGosub = false;
        var locations = new List<Expression>();
        var linePositions = new List<int>();
        bool hasRedirector = true;

        if (Match(TokenType.Goto, TokenType.Gosub))
        {
            isGosub = Previous().Type == TokenType.Gosub;
            do
            {
                Token current = Peek();
                locations.Add(Expression());
                linePositions.Add(current.LinePosition + current.Lexeme.Length);
            } while (Match(TokenType.Comma));

            while (!IsAtEnd())
                Advance();
        }
        else
            hasRedirector = false;

        return StatementWrapper(new On(selector, hasRedirector, locations, linePositions, isGosub));
    }

    private IStatement StatementWrapper(IStatement statement)
    {
        statement.LineNumber = _lineNumber;
        statement.SourceLine = _source;
        if (statement.ParseException == null)
            statement.ParseException = _parseException;

        return statement;
    }

    private IStatement ContStatement()
    {
        return StatementWrapper(new Cont());
    }

    private IStatement GotoStatement()
    {
        Expression location = Expression();
        return StatementWrapper(new Goto(location, Peek().LinePosition));
    }

    private IStatement ClsStatement()
    {
        return StatementWrapper(new Cls());
    }

    private IStatement NextStatement()
    {
        return IsAtEnd() ?
            StatementWrapper(new Next(null)) :
            StatementWrapper(Peek().Type != TokenType.Identifier ?
                new Next(Expression()) :
                new Next(Identifier()));
    }

    private IStatement ForStatement()
    {
        Expression identifier = Peek().Type == TokenType.Identifier ? Identifier() : Unary();

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
            stepValue = new Literal(1, null, Peek().LinePosition);

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
                            "_pad_quadrant",
                            "_pad_quadrant",
                            null,
                            0
                        ),
                        _padQuadrant,
                        new List<Expression>(),
                        0));

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
        ParseException thenException = null;
        Token current = Peek();
        if (!Match(TokenType.Then, TokenType.Goto, TokenType.T, TokenType.Gosub) && Peek().Type == TokenType.Number)
            thenException = new ParseException(_lineNumber, _source, current.LinePosition,
                "Expected 'THEN' or 'GOTO' before line number in 'IF' statement.");

        var thenBranch = new CompoundStatementList(Peek().LinePosition) {
            StatementWrapper(
                current.Type switch
                {
                    TokenType.Gosub => new Gosub(Expression(), Peek().LinePosition),
                    TokenType.Goto => new Goto(Expression(), Peek().LinePosition),
                    _ => Peek().Type == TokenType.Number ? new Goto(Expression(), Peek().LinePosition) : Statement()
                })
        };

        while (Match(TokenType.Colon))
            thenBranch.Add(Statement());

        return StatementWrapper(new If(condition, thenBranch, thenException));
    }

    private IStatement SaveStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty, string.Empty, 0);

        return new Save(path);
    }

    private IStatement LoadStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty, string.Empty, 0);
        return new Load(path);
    }

    private IStatement MergeStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty, string.Empty, 0);
        return new Merge(path);
    }
    private IStatement RemarkStatement()
    {
        string literal = Previous().Literal;
        var value = new Literal(literal, literal.ToUpperInvariant(), 0);
        return StatementWrapper(new Rem(value));
    }

    private IStatement ListStatement()
    {
        Expression value = !IsAtEnd() ? Expression() : new Literal(0, null, 0);
        return new List(value);
    }

    private IStatement RunStatement()
    {
        Expression value = !IsAtEnd() ? Expression() : new Literal(-1, null, 0);
        return new Run(value);
    }

    private IStatement ExpressionStatement()
    {
        Expression expression = Expression();
        return new StatementExpression(expression);
    }

    private IStatement LetStatement()
    {
        Token current = Peek();
        Token next = PeekNext();
        Expression identifier;

        if (current.Type == TokenType.Identifier)
        {
            if (next.Type != TokenType.Equal && _natives.Get(current.Lexeme) != null)
                return StatementWrapper(new StatementExpression(Call()));

            identifier = Identifier();
        }
        else
            identifier = Unary();

        Consume(TokenType.Equal, "Expected assignment.");

        Expression value = Expression();
        if (value is not Identifier unquoted) return StatementWrapper(new Let(identifier, value));

        string lexeme = unquoted.Name.Lexeme;
        if (lexeme.Length > 1 &&
            (!lexeme.EndsWith('$') || lexeme.Length > 2))
            value = new Literal(lexeme, lexeme.ToUpperInvariant(), unquoted.LinePosition);

        return StatementWrapper(new Let(identifier, value));
    }

    private Expression Identifier()
    {
        Token current = Peek();

        if (current.Type != TokenType.Identifier)
            throw new ParseException(_lineNumber, _source, current.LinePosition, "Expected variable name or function call.");

        Advance();

        if (!Match(TokenType.LeftParen))
            return new Identifier(current, current.Lexeme.EndsWith('$'), current.LinePosition + 1);

        Expression index = Expression();

        Token previous = Previous();
        int linePosition = previous.LinePosition + previous.Lexeme.Length;

        Consume(TokenType.RightParen, "Expected ')' after array index");

        return new Expressions.Array(current, index, linePosition);
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
        bool atSeparator = true;

        Token current = Peek();
        Token next = PeekNext();
        if (current.Type != TokenType.EndOfLine && next.Type != TokenType.EndOfLine)
#pragma warning disable S1066 // Collapsible "if" statements should be merged
            if (current.Type == TokenType.At || (current.Type == TokenType.A && next.Type != TokenType.LeftParen))
            {
                Advance();
                atPosition = Expression();
                if (!Match(TokenType.Comma, TokenType.Semicolon))
                    atSeparator = false;
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
                            "_pad_quadrant",
                            "_pad_quadrant",
                            null,
                            0
                        ),
                        _padQuadrant,
                        new List<Expression>(),
                        0));

        }

        return StatementWrapper(new Print(atPosition, atSeparator, values, newline));
    }

    private int GetLineNumberValue(Token lineNumber)
    {
        dynamic line = lineNumber.Literal;

        if (line == null || line is not int) return -1;

        if (line > short.MaxValue)
            throw new ValueOutOfRangeException(-1, null, $"Line number cannot exceed {short.MaxValue}.");

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
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
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
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
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
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
    }

    private Expression Unary()
    {
        if (!Match(TokenType.Minus)) return Call();

        Token operatorType = Previous();
        Expression right = Unary();
        return new Unary(operatorType, right, operatorType.LinePosition);
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
            return new Call(name, callee, new List<Expression>(), Peek().LinePosition);

        ParseException pe = new ParseException(_lineNumber, _source, Peek().LinePosition, $"Invalid number of arguments passed to function '{previous.Lexeme}'");
        callee = callees.First();
        Expression call = new Call(name, callee, new List<Expression>(), Peek().LinePosition);
        call.ParseException = pe;

        return call;
        /*
                ParseException pe = null;
                if (callee == null)
                {
                    int linePosition;
                    callee = callees.FirstOrDefault(f => f.Arity < arguments.Count);
                    if (callee != null)
                        linePosition = argumentPositions[callee.Arity];
                    else
                    {
                        callee = callees.FirstOrDefault(f => f.Arity > arguments.Count);
                        linePosition = argumentPositions[arguments.Count - 1];
                    }

                    pe = new ParseException(false, _lineNumber, _source, linePosition, $"Unknown function '{name.Lexeme}' with argument count {arguments.Count}");
                }

                Expression call = new Call(name, callee, arguments, Peek().LinePosition);
                call.ParseException = pe;

                return call;
        */
    }

    private Expression FinishCall(Token name, List<Callable> callees)
    {
        var arguments = new List<Expression>();
        var argumentPositions = new List<int> { Previous().LinePosition };

        if (!Check(TokenType.RightParen))
            do
            {
                arguments.Add(Expression());
                argumentPositions.Add(Peek().LinePosition);
            }
            while (Match(TokenType.Comma));

        Consume(TokenType.RightParen, "Expected ')' after arguments");
        int parenPosition = Previous().LinePosition;

        Callable callee = callees.FirstOrDefault(f => f.Arity == arguments.Count);

        ParseException pe = null;
        if (callee == null)
        {
            int linePosition;
            callee = callees.FirstOrDefault(f => f.Arity < arguments.Count);
            if (callee != null)
                linePosition = argumentPositions[callee.Arity];
            else
            {
                callee = callees.FirstOrDefault(f => f.Arity > arguments.Count);
                linePosition = argumentPositions[arguments.Count];
            }

            pe = new ParseException(_lineNumber, _source, linePosition, $"Unknown function '{name.Lexeme}' with argument count {arguments.Count}");
        }

        Expression call = new Call(name, callee, arguments, Peek().LinePosition);
        call.ParseException = pe;

        return call;
    }

    private Expression FinishArray(Token name)
    {
        Expression index = Expression();

        Token previous = Previous();
        int linePosition = previous.LinePosition + previous.Lexeme.Length;

        Consume(TokenType.RightParen,
            "Expected ')' after arguments");

        return new Expressions.Array(name, index, linePosition);
    }

    private Expression Primary()
    {
        if (Match(TokenType.Number))
            return new Literal(Previous().Literal, null, Previous().LinePosition);

        if (Match(TokenType.String))
        {
            string literal = Previous().Literal;
            return new Literal(literal, literal.ToUpperInvariant(), Previous().LinePosition);
        }

        if (Match(TokenType.LeftParen))
        {
            Expression expression = Expression();
            Consume(TokenType.RightParen, "Expected ')' after expression.");
            return new Grouping(expression, expression.LinePosition);
        }

        if (Match(TokenType.Identifier))
        {
            Token previous = Previous();
            return new Identifier(previous, previous.Lexeme.EndsWith('$'), previous.LinePosition + 1);
        }

        if (!IsIdentifierShortHand())
        {
            Expression literal = new Literal(Peek().Lexeme, Peek().Lexeme, Peek().LinePosition);
            literal.ParseException = new ParseException(_lineNumber, _source, Peek().LinePosition, "Expected expression.");
            return literal;
        }

        Token current = Peek();
        Advance();
        var identifier = new Token(TokenType.Identifier, current.Lexeme, current.Lexeme, _source, current.LinePosition);
        return new Identifier(identifier, current.Lexeme.EndsWith('$'), current.LinePosition + 1);
    }

    private bool IsIdentifierShortHand()
    {
        Token token = Peek();

        if (token == null) return false;
        if (token.Type == TokenType.EndOfLine) return false;
        try
        {
            Callable function = _natives.Get(token.Lexeme).FirstOrDefault();

            return function != null;
        }
        catch
        {
            return false;
        }
    }

    private void Consume(TokenType type, string message)
    {
        if (!Check(type) && _parseException == null)
            _parseException = new ParseException(_lineNumber, _source, Peek().LinePosition, message);
        else
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