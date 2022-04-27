using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Trs80.Level1Basic.Domain;
using Trs80.Level1Basic.Exceptions;
using Trs80.Level1Basic.Services.Interpreter;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Parser;

public interface IParser
{
    Line Parse(string source);
}

public class Parser : IParser
{
    private List<Token> _tokens;
    private int _current;
    private Line _currentLine;
    private readonly IScanner _tokenizer;
    private readonly IBuiltinFunctions _builtins;

    public Parser(IScanner tokenizer, IBuiltinFunctions builtins)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _builtins = builtins ?? throw new ArgumentNullException(nameof(builtins));
    }

    public Line Parse(string source)
    {
        Initialize();
        _tokens = _tokenizer.ScanTokens(source);

        return IsAtEnd() ? new Line() : Line();
    }

    private Line Line()
    {
        _currentLine = new Line();
        var lineNumber = Peek();

        _currentLine.LineNumber = GetLineNumberValue(lineNumber);

        if (_currentLine.LineNumber == 0 && char.IsLetter(lineNumber.SourceLine[0]))
            _currentLine.SourceLine = lineNumber.SourceLine;
        else
            _currentLine.SourceLine = lineNumber.SourceLine.Replace(_currentLine.LineNumber.ToString(), "").TrimStart(' ');


        if (lineNumber.Type == TokenType.Number)
        {
            if (PeekNext().Type != TokenType.EndOfLine)
                Advance();
            else
            {
                _currentLine.Statements = new List<Statement> { DeleteStatement(lineNumber) };
                return _currentLine;
            }
        }

        _currentLine.Statements = Statements(lineNumber);
        return _currentLine;
    }

    private void Initialize()
    {
        _tokens = null;
        _current = 0;
    }

    private List<Statement> Statements(Token lineNumber)
    {
        var statements = new List<Statement> { Statement(lineNumber) };

        while (Match(TokenType.Colon))
            statements.Add(Statement(lineNumber));

        return statements;
    }
    private Statement Statement(Token lineNumber)
    {
        if (Match(TokenType.Cls))
            return ClsStatement(lineNumber);
        if (Match(TokenType.Cont))
            return ContStatement(lineNumber);
        if (Match(TokenType.Data))
            return DataStatement(lineNumber);
        if (Match(TokenType.End))
            return EndStatement(lineNumber);
        if (Match(TokenType.For))
            return ForStatement(lineNumber);
        if (Match(TokenType.Gosub))
            return GosubStatement(lineNumber);
        if (Match(TokenType.Goto))
            return GotoStatement(lineNumber);
        if (Match(TokenType.If))
            return IfStatement(lineNumber);
        if (Match(TokenType.Input))
            return InputStatement(lineNumber);
        if (Match(TokenType.Let))
            return LetStatement(lineNumber);
        if (Match(TokenType.List))
            return ListStatement();
        if (Match(TokenType.Load))
            return LoadStatement();
        if (Match(TokenType.N))
            return lineNumber.Type == TokenType.Number ? NextStatement(lineNumber) : NewStatement(lineNumber);
        if (Match(TokenType.New))
            return NewStatement(lineNumber);
        if (Match(TokenType.Next))
            return NextStatement(lineNumber);
        if (Match(TokenType.On))
            return OnStatement(lineNumber);
        if (Match(TokenType.Print))
            return PrintStatement(lineNumber);
        if (Match(TokenType.Read))
            return ReadStatement(lineNumber);
        if (Match(TokenType.Rem))
            return RemarkStatement(lineNumber);
        if (Match(TokenType.Restore))
            return RestoreStatement(lineNumber);
        if (Match(TokenType.Return))
            return ReturnStatement(lineNumber);
        if (Match(TokenType.Run))
            return RunStatement();
        if (Match(TokenType.Save))
            return SaveStatement();
        if (Match(TokenType.Stop))
            return StopStatement(lineNumber);

        return Peek().Type == TokenType.Identifier ? LetStatement(lineNumber) : ExpressionStatement();
    }

    private Statement StopStatement(Token lineNumber)
    {
        return StatementWrapper(new Stop(), lineNumber);
    }

    private Statement RestoreStatement(Token lineNumber)
    {
        return StatementWrapper(new Restore(), lineNumber);
    }

    private Statement ReadStatement(Token lineNumber)
    {
        var variables = new List<Expression>();
        do
        {
            if (Peek().Type != TokenType.Identifier)
                throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, "Expected variable after 'READ'.");

            variables.Add(Expression());
        } while (Match(TokenType.Comma));

        return StatementWrapper(new Read(variables), lineNumber);
    }

    private Statement DataStatement(Token lineNumber)
    {
        var elements = new List<Expression>();
        do
        {
            elements.Add(Expression());
        } while (Match(TokenType.Comma));

        return StatementWrapper(new Data(elements), lineNumber);
    }

    //private Expression StringData()
    //{
    //    var sb = new StringBuilder();
    //    sb.Append(Peek().Lexeme);
    //    Advance();

    //    while (!IsAtEnd() && Peek().Type != TokenType.Comma)
    //    {
    //        sb.Append(" ");
    //        sb.Append(Peek().Lexeme);
    //        Advance();
    //    }
    //    return new Literal(sb.ToString());
    //}

    private Statement ReturnStatement(Token lineNumber)
    {
        return StatementWrapper(new Return(), lineNumber);
    }

    private Statement GosubStatement(Token lineNumber)
    {
        var location = Expression();
        return StatementWrapper(new Gosub(location), lineNumber);
    }

    private Statement OnStatement(Token lineNumber)
    {
        var selector = Expression();
        var isGosub = false;
        var locations = new List<Expression>();
        if (Match(TokenType.Goto))
        {
            do
            {
                locations.Add(Expression());
            } while (Match(TokenType.Comma));

            while (!IsAtEnd())
                Advance();
        }
        else if (Match(TokenType.Gosub))
        {
            isGosub = true;
            do
            {
                locations.Add(Expression());
            } while (Match(TokenType.Comma));

            while (!IsAtEnd())
                Advance();
        }
        else
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
                "Expected 'GOTO' or 'GOSUB' after 'ON'");

        return StatementWrapper(new On(selector, locations, isGosub), lineNumber);
    }

    private Statement StatementWrapper(Statement statement, Token lineNumber)
    {
        var lineNumberValue = GetLineNumberValue(lineNumber);
        statement.LineNumber = lineNumberValue;
            
        if (lineNumberValue == 0 && char.IsLetter(lineNumber.SourceLine[0]))
            statement.SourceLine = lineNumber.SourceLine;
        else
            statement.SourceLine = lineNumber.SourceLine.Replace(lineNumberValue.ToString(), "").TrimStart(' ');
            
        return statement;
    }
    private Statement ContStatement(Token lineNumber)
    {
        return StatementWrapper(new Cont(), lineNumber);
    }

    private Statement GotoStatement(Token lineNumber)
    {
        var location = Expression();
        return StatementWrapper(new Goto(location), lineNumber);
    }

    private Statement ClsStatement(Token lineNumber)
    {
        return StatementWrapper(new Cls(), lineNumber);
    }

    private Statement NextStatement(Token lineNumber)
    {
        if (Peek().Type != TokenType.Identifier)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected variable name after 'NEXT'.");

        var identifier = Identifier();
        Advance();
            
        return StatementWrapper(new Next(identifier), lineNumber);
    }

    private Statement ForStatement(Token lineNumber)
    {
        var identifier = Identifier();

        Consume(TokenType.Equal, "Expected assignment.");

        var startValue = Expression();
        Consume(TokenType.To, "Expected TO");
        var endValue = Expression();

        Expression stepValue;
        if (Peek().Type == TokenType.Step)
        {
            Advance();
            stepValue = Expression();
        }
        else
            stepValue = new Literal(1);

        return StatementWrapper(new For(identifier, startValue, endValue, stepValue), lineNumber);
    }

    private Statement InputStatement(Token lineNumber)
    {
        var newline = true;
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
                        new List<Expression>()));

            if (IsAtStatementEnd() && values[^1] is Identifier)
                newline = false;
        }


        return StatementWrapper(new Input(values, newline), lineNumber);
    }

    private Statement NewStatement(Token lineNumber)
    {
        return StatementWrapper(new New(), lineNumber);
    }

    private Statement DeleteStatement(Token lineNumber)
    {
        var lineNumberValue = GetLineNumberValue(lineNumber);
        Advance();
        return new Delete(lineNumberValue);
    }

    private Statement EndStatement(Token lineNumber)
    {
        return StatementWrapper(new End(), lineNumber);
    }

    private Statement IfStatement(Token lineNumber)
    {
        var condition = Expression();

        if (!Match(TokenType.Then, TokenType.Goto) && Peek().Type == TokenType.Number)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
                "Expected 'THEN' or 'GOTO' before line number in 'IF' statement.");

        var thenStatements = new List<Statement>
        {
            Peek().Type == TokenType.Number ?
                new Goto(Expression()) :
                Statement(lineNumber)
        };

        while (Match(TokenType.Colon))
            thenStatements.Add(Statement(lineNumber));

        return StatementWrapper(new If(condition, thenStatements), lineNumber);
    }

    private Statement SaveStatement()
    {
        var path = Expression();
        return new Save(path);
    }

    private Statement LoadStatement()
    {
        var path = Expression();
        return new Load(path);
    }

    private Statement RemarkStatement(Token lineNumber)
    {
        var value = new Literal(Previous().Literal);
        return StatementWrapper(new Rem(value), lineNumber);
    }

    private Statement ListStatement()
    {
        var value = !IsAtEnd() ? Expression() : new Literal(0);
        return new List(value);
    }

    private Statement RunStatement()
    {
        var value = !IsAtEnd() ? Expression() : new Literal(0);
        return new Run(value);
    }

    private Statement ExpressionStatement()
    {
        var expression = Expression();
        return new StatementExpression(expression);
    }

    private Statement LetStatement(Token lineNumber)
    {
        var peek = Peek();
        var peekNext = PeekNext();

        if (peek.Type != TokenType.Identifier)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
                "Expected variable name or function call.");

        if (peekNext.Type != TokenType.Equal && _builtins.Get(peek.Lexeme) != null) return StatementWrapper(new StatementExpression(Call()), lineNumber);

        var identifier = Identifier();

        Consume(TokenType.Equal, "Expected assignment.");

        var value = Expression();
        return StatementWrapper(new Let(identifier, value), lineNumber);
    }

    private Expression Identifier()
    {
        var peek = Peek();

        if (peek.Type != TokenType.Identifier)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
                "Expected variable name or function call.");

        Advance();

        if (!Match(TokenType.LeftParen)) return new Identifier(peek);

        var index = Expression();
        Consume(TokenType.RightParen, "Expected ')' after array index");
        return new BasicArray(peek, index);
    }

    private bool IsAtStatementEnd()
    {
        return IsAtEnd() || Peek().Type == TokenType.Colon;
    }
    private Statement PrintStatement(Token lineNumber)
    {
        var newline = true;
        var values = new List<Expression>();
        Expression atPosition = null;

        if (Match(TokenType.At))
        {
            atPosition = Expression();
            Consume(TokenType.Comma, "Expected ',' after AT clause.");
        }

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
                        new List<Expression>()));

        }

        return StatementWrapper(new Print(atPosition, values, newline), lineNumber);
    }

    private int GetLineNumberValue(Token lineNumber)
    {
        var line = lineNumber.Literal;

        if (line == null) return 0;

        if (line > short.MaxValue)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, $"Line number cannot exceed {short.MaxValue}.");

        return line;
    }

    private Expression Expression()
    {
        return Comparison();
    }

    private Expression Comparison()
    {
        var left = Term();
        while (Match(TokenType.LessThanOrEqual, TokenType.LessThan, 
                   TokenType.GreaterThanOrEqual, TokenType.GreaterThan, 
                   TokenType.NotEqual, TokenType.Equal))
        {
            var operatorType = Previous();
            var right = Term();
            left = new Binary(left, operatorType, right);
        }

        return left;
    }

    private Expression Term()
    {
        var left = Factor();
        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var operatorType = Previous();
            var right = Factor();
            left = new Binary(left, operatorType, right);
        }

        return left;
    }

    private Expression Factor()
    {
        var left = Unary();
        while (Match(TokenType.Slash, TokenType.Star))
        {
            var operatorType = Previous();
            var right = Unary();
            left = new Binary(left, operatorType, right);
        }

        return left;
    }

    private Expression Unary()
    {
        if (!Match(TokenType.Minus)) return Call();

        var operatorType = Previous();
        var right = Unary();
        return new Unary(operatorType, right);
    }

    private Expression Call()
    {
        var name = Peek();

        var expression = Primary();

        if (Match(TokenType.LeftParen) && name.Type == TokenType.Identifier)
            return _builtins.Get(name.Lexeme) != null ? FinishCall(name) : FinishArray(name);

        var previous = Previous();
        if (previous.Type != TokenType.Identifier) return expression;

        var function = _builtins.Get(previous.Lexeme);
        if (function == null) return expression;

        if (function.Arity == 0)
            return new Call(name, new List<Expression>());

        throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
            $"Invalid number of arguments passed to function '{previous.Lexeme}'");
    }

    private Expression FinishCall(Token name)
    {
        var arguments = new List<Expression>();

        if (!Check(TokenType.RightParen))
            do
            {
                arguments.Add(Expression());
            } while (Match(TokenType.Comma));

        Consume(TokenType.RightParen, "Expected ')' after arguments");

        CheckArgs(name, arguments);

        return new Call(name, arguments);
    }

    private Expression FinishArray(Token name)
    {
        var index = Expression();

        var rightParen = Consume(TokenType.RightParen, 
            "Expected ')' after arguments");

        CheckIndex(name, index, rightParen);

        return new BasicArray(name, index);
    }

    private void CheckArgs(Token name, List<Expression> arguments)
    {
        var function = _builtins.Get(name.Lexeme);
        if (function == null)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, $"Unknown function '{name.Lexeme}'");

        if (arguments.Count != function.Arity)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
                $"Invalid number of arguments passed to function '{name.Lexeme}'");
    }

#pragma warning disable IDE0079
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private void CheckIndex(Token name, Expression index, Token rightParen)
    {
        //var function = _builtins.Get(name.Lexeme);
        //if (function == null)
        //    throw new ParseException(rightParen, $"Unknown function '{name.Lexeme}'");

        //if (arguments.Count != function.Arity)
        //    throw new ParseException(rightParen, $"Invalid number of arguments passed to function '{name.Lexeme}'");
    }
#pragma warning restore IDE0079

    private Expression Primary()
    {
        if (Match(TokenType.Number, TokenType.String))
            return new Literal(Previous().Literal);

        if (Match(TokenType.LeftParen))
        {
            var expression = Expression();
            Consume(TokenType.RightParen, "Expected ')' after expression.");
            return new Grouping(expression);
        }

        if (Match(TokenType.Identifier))
            return new Identifier(Previous());

        //if (Match(TokenType.Stop))
        //    return new Stop();

        //if (Peek().Type == TokenType.Colon)
        //    return new Literal(string.Empty);

        throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, 
            "Expected expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (!Check(type)) throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, message);

        return Advance();
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

    private Token Advance()
    {
        if (!IsAtEnd())
            _current++;

        return Previous();
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