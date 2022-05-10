﻿using System;
using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Parser;

public interface IParser
{
    ParsedLine Parse(List<Token> tokens);
}

public class Parser : IParser
{
    private List<Token> _tokens;
    private int _current;
    private ParsedLine _currentLine;
    private readonly IBuiltinFunctions _builtins;

    public Parser(IBuiltinFunctions builtins)
    {
        _builtins = builtins ?? throw new ArgumentNullException(nameof(builtins));
    }

    public ParsedLine Parse(List<Token> tokens)
    {
        if (tokens == null) return null;

        Initialize();
        _tokens = tokens;

        return IsAtEnd() ? null : Line();
    }

    private ParsedLine Line()
    {
        _currentLine = new ParsedLine();
        Token lineNumber = Peek();

        _currentLine.LineNumber = GetLineNumberValue(lineNumber);

        if (_currentLine.LineNumber == -1 && char.IsLetter(lineNumber.SourceLine[0]))
            _currentLine.SourceLine = lineNumber.SourceLine;
        else
        {
            int lineNumberLength = _currentLine.LineNumber.ToString().Length;
            _currentLine.SourceLine = lineNumber.SourceLine[lineNumberLength..].TrimStart(' ');
        }

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
        if (Match(TokenType.Merge))
            return MergeStatement();
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
        if (Peek().Type != TokenType.R || PeekNext().Type == TokenType.LeftParen)
            return Peek().Type == TokenType.Identifier ? LetStatement(lineNumber) : ExpressionStatement();

        Advance();
        return RunStatement();
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
            elements.Add(Expression());
        while (Match(TokenType.Comma));

        return StatementWrapper(new Data(elements), lineNumber);
    }

    private Statement ReturnStatement(Token lineNumber)
    {
        return StatementWrapper(new Return(), lineNumber);
    }

    private Statement GosubStatement(Token lineNumber)
    {
        Expression location = Expression();
        return StatementWrapper(new Gosub(location), lineNumber);
    }

    private Statement OnStatement(Token lineNumber)
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
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected 'GOTO' or 'GOSUB' after 'ON'");

        return StatementWrapper(new On(selector, locations, isGosub), lineNumber);
    }

    private Statement StatementWrapper(Statement statement, Token lineNumber)
    {
        int lineNumberValue = GetLineNumberValue(lineNumber);
        statement.LineNumber = lineNumberValue;

        if (lineNumberValue == 0 && char.IsLetter(lineNumber.SourceLine[0]))
            statement.SourceLine = lineNumber.SourceLine;
        else if (lineNumber.SourceLine.StartsWith(lineNumberValue.ToString()))
            statement.SourceLine = lineNumber.SourceLine.Replace(lineNumberValue.ToString(), "").TrimStart(' ');

        return statement;
    }

    private Statement ContStatement(Token lineNumber)
    {
        return StatementWrapper(new Cont(), lineNumber);
    }

    private Statement GotoStatement(Token lineNumber)
    {
        Expression location = Expression();
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

        Expression identifier = Identifier();

        return StatementWrapper(new Next(identifier), lineNumber);
    }

    private Statement ForStatement(Token lineNumber)
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

        return StatementWrapper(new For(identifier, startValue, endValue, stepValue), lineNumber);
    }

    private Statement InputStatement(Token lineNumber)
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
        int lineNumberValue = GetLineNumberValue(lineNumber);
        Advance();
        return new Delete(lineNumberValue);
    }

    private Statement EndStatement(Token lineNumber)
    {
        return StatementWrapper(new End(), lineNumber);
    }

    private Statement IfStatement(Token lineNumber)
    {
        Expression condition = Expression();
        Token current = Peek();
        if (!Match(TokenType.Then, TokenType.Goto, TokenType.T, TokenType.Gosub) && Peek().Type == TokenType.Number)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected 'THEN' or 'GOTO' before line number in 'IF' statement.");

        Statement thenStatement = current.Type switch {
            TokenType.Gosub => new Gosub(Expression()),
            TokenType.Goto => new Goto(Expression()),
            _ => Peek().Type == TokenType.Number ? new Goto(Expression()) : Statement(lineNumber)
        };

        Statement savedThenStatement = thenStatement;

        while (Match(TokenType.Colon))
        {
            thenStatement.Next = Statement(lineNumber);
            thenStatement = thenStatement.Next;
        }

        return StatementWrapper(new If(condition, savedThenStatement), lineNumber);
    }

    private Statement SaveStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty);

        return new Save(path);
    }

    private Statement LoadStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty);
        return new Load(path);
    }

    private Statement MergeStatement()
    {
        Expression path = !IsAtEnd() ? Expression() : new Literal(string.Empty);
        return new Merge(path);
    }
    private Statement RemarkStatement(Token lineNumber)
    {
        var value = new Literal(Previous().Literal);
        return StatementWrapper(new Rem(value), lineNumber);
    }

    private Statement ListStatement()
    {
        Expression value = !IsAtEnd() ? Expression() : new Literal(0);
        return new List(value);
    }

    private Statement RunStatement()
    {
        Expression value = !IsAtEnd() ? Expression() : new Literal(-1);
        return new Run(value);
    }

    private Statement ExpressionStatement()
    {
        Expression expression = Expression();
        return new StatementExpression(expression);
    }

    private Statement LetStatement(Token lineNumber)
    {
        Token peek = Peek();
        Token peekNext = PeekNext();

        if (peek.Type != TokenType.Identifier)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected variable name or function call.");

        if (peekNext.Type != TokenType.Equal && _builtins.Get(peek.Lexeme) != null) return StatementWrapper(new StatementExpression(Call()), lineNumber);

        Expression identifier = Identifier();

        Consume(TokenType.Equal, "Expected assignment.");

        Expression value = Expression();
        return StatementWrapper(new Let(identifier, value), lineNumber);
    }

    private Expression Identifier()
    {
        Token peek = Peek();

        if (peek.Type != TokenType.Identifier)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected variable name or function call.");

        Advance();

        if (!Match(TokenType.LeftParen)) return new Identifier(peek);

        Expression index = Expression();
        Consume(TokenType.RightParen, "Expected ')' after array index");
        return new BasicArray(peek, index);
    }

    private bool IsAtStatementEnd()
    {
        return IsAtEnd() || Peek().Type == TokenType.Colon;
    }

    private Statement PrintStatement(Token lineNumber)
    {
        bool newline = true;
        var values = new List<Expression>();
        Expression atPosition = null;

        Token current = Peek();
        Token next = PeekNext();
        if (current.Type != TokenType.EndOfLine && next.Type != TokenType.EndOfLine)
            if (current.Type == TokenType.At || (current.Type == TokenType.A && next.Type != TokenType.LeftParen))
            {
                Advance();
                atPosition = Expression();
                if (!Match(TokenType.Comma, TokenType.Semicolon))
                    throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, "Expected ',' or ';' after AT clause.");
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
        dynamic line = lineNumber.Literal;

        if (line == null) return -1;

        if (line is not int)
            throw new ParseException(-1, lineNumber.SourceLine, $"Invalid text at {line}");
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

        if (Match(TokenType.LeftParen) && expression is Identifier)
            return _builtins.Get(name.Lexeme) != null ? FinishCall(name) : FinishArray(name);

        Token previous = Previous();
        if (previous.Type != TokenType.Identifier) return expression;

        List<FunctionDefinition> functions = _builtins.Get(previous.Lexeme);
        if (functions == null) return expression;

        if (functions.Any(f => f.Arity == 0))
            return new Call(name, new List<Expression>());

        throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
            $"Invalid number of arguments passed to function '{previous.Lexeme}'");
    }

    private Expression FinishCall(Token name)
    {
        var arguments = new List<Expression>();

        if (!Check(TokenType.RightParen))
            do
                arguments.Add(Expression());
            while (Match(TokenType.Comma));

        Consume(TokenType.RightParen, "Expected ')' after arguments");

        CheckArgs(name, arguments);

        return new Call(name, arguments);
    }

    private Expression FinishArray(Token name)
    {
        Expression index = Expression();

        Consume(TokenType.RightParen,
            "Expected ')' after arguments");

        return new BasicArray(name, index);
    }

    private void CheckArgs(Token name, List<Expression> arguments)
    {
        FunctionDefinition function =
            _builtins.Get(name.Lexeme).FirstOrDefault(f => f.Arity == arguments.Count);

        if (function == null)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                $"Unknown function '{name.Lexeme}' with argument count {arguments.Count}");
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
            return new Identifier(Previous());

        if (!IsIdentifierShortHand())
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected expression.");

        Token current = Peek();
        Advance();
        return new Identifier(new Token(TokenType.Identifier, current.Lexeme, current.Lexeme, _currentLine.SourceLine));
    }

    private bool IsIdentifierShortHand()
    {
        Token token = Peek();

        if (token == null) return false;
        if (token.Type == TokenType.EndOfLine) return false;

        FunctionDefinition function = _builtins.Get(token.Lexeme).FirstOrDefault();

        return function != null;
    }

    private void Consume(TokenType type, string message)
    {
        if (!Check(type)) throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, message);

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