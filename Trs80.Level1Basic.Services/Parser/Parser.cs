﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Trs80.Level1Basic.Domain;
using Trs80.Level1Basic.Exceptions;
using Trs80.Level1Basic.Services.Interpreter;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Parser;

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
        var lineNumber = Peek();

        _currentLine.LineNumber = GetLineNumberValue(lineNumber);

        if (_currentLine.LineNumber == 0 && char.IsLetter(lineNumber.SourceLine[0]))
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
        bool isGosub = false;
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
        int lineNumberValue = GetLineNumberValue(lineNumber);
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
        var condition = Expression();

        if (!Match(TokenType.Then, TokenType.Goto, TokenType.T, TokenType.Gosub) && Peek().Type == TokenType.Number)
            throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
                "Expected 'THEN' or 'GOTO' before line number in 'IF' statement.");

        var thenStatement =
            Peek().Type == TokenType.Number ?
                new Goto(Expression()) :
                Statement(lineNumber);

        var savedThenStatement = thenStatement;

        while (Match(TokenType.Colon))
        {
            thenStatement.Next = Statement(lineNumber);
            thenStatement = thenStatement.Next;
        }

        return StatementWrapper(new If(condition, savedThenStatement), lineNumber);
    }

    private Statement SaveStatement()
    {
        var path = !IsAtEnd() ? Expression() : SaveFileDialog();

        return new Save(path);
    }

    private const string Filter = "BASIC files (*.bas)|*.bas|All files (*.*)|*.*";
    private const string Title = "TRS-80 Level I BASIC File";
    private Expression SaveFileDialog()
    {
        var dialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = "bas",
            Filter = Filter,
            Title = $"Save {Title}",
            OverwritePrompt = true
        };

        return dialog.ShowDialog() == DialogResult.OK ? new Literal(dialog.FileName) : null;
    }

    private Statement LoadStatement()
    {
        var path = !IsAtEnd() ? Expression() : OpenFileDialog();
        return new Load(path);
    }

    private Statement MergeStatement()
    {
        var path = !IsAtEnd() ? Expression() : OpenFileDialog();
        return new Merge(path);
    }

    private Expression OpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            DefaultExt = "bas",
            Filter = Filter,
            Title = $"Open {Title}",
            CheckFileExists = true,
            Multiselect = false,
        };

        return dialog.ShowDialog() == DialogResult.OK ? new Literal(dialog.FileName) : null;
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
        bool newline = true;
        var values = new List<Expression>();
        Expression atPosition = null;

        if (Match(TokenType.At))
        {
            atPosition = Expression();
            if (Match(TokenType.Comma) || Match(TokenType.Semicolon))
                Advance();
            else
                throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine, "Expected ',' or ';' after AT clause.");
        }

        if (Match(TokenType.T) && Match(TokenType.LeftParen))
        {
            var t = new Token(TokenType.Identifier, "tab", "tab", _currentLine.SourceLine);
            values.Add(FinishCall(t));
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

        if (line == null) return 0;

        if (line is not int)
            throw new ParseException(0, lineNumber.SourceLine, $"Invalid text at {line}");
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

        Consume(TokenType.RightParen,
            "Expected ')' after arguments");

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

        throw new ParseException(_currentLine.LineNumber, _currentLine.SourceLine,
            "Expected expression.");
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