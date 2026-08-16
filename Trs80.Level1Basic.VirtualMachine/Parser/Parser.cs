using System;
using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;
using Array = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Array;

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
    private ParseException _parseException;

    public Parser(ITrs80 trs80, INativeFunctions natives, IAppSettings appSettings)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _natives = natives ?? throw new ArgumentNullException(nameof(natives));
        _padQuadrant = _natives.Get("_pad_quadrant").First();
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public IStatement Parse(List<Token> tokens)
    {
        _parseException = null;
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
        if (IsDefFunction())
            return DefFunctionStatement();
        if (Match(TokenType.Beep))
            return BeepStatement();
        if (Match(TokenType.Clear))
            return ClearStatement();
        if (IsMidAssignment())
            return MidAssignmentStatement();
        if (Match(TokenType.Cls))
            return ClsStatement();
        if (Match(TokenType.Cont))
            return ContStatement();
        if (Match(TokenType.Data))
            return DataStatement();
        if (Match(TokenType.Delete))
            return ExplicitDeleteStatement();
        if (Match(TokenType.DefDbl, TokenType.DefInt, TokenType.DefSng, TokenType.DefStr))
            return DefTypeStatement();
        if (Match(TokenType.Dim))
            return DimStatement();
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
        if (Match(TokenType.Llist))
            return LlistStatement();
        if (Match(TokenType.Lprint))
            return LprintStatement();
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
        if (Match(TokenType.Out))
            return OutStatement();
        if (Match(TokenType.Poke))
            return PokeStatement();
        if (Match(TokenType.Wait))
            return WaitStatement();
        if (Match(TokenType.Print))
            return PrintStatement();
        if (Match(TokenType.Read))
            return ReadStatement();
        if (Match(TokenType.Rem))
            return RemarkStatement();
        if (Match(TokenType.Reset))
            return ResetStatement();
        if (Match(TokenType.Restore))
            return RestoreStatement();
        if (Match(TokenType.Return))
            return ReturnStatement();
        if (Match(TokenType.Run))
            return RunStatement();
        if (Match(TokenType.Save))
            return SaveStatement();
        if (Match(TokenType.Set))
            return SetStatement();
        if (Match(TokenType.Stop))
            return StopStatement();
        if (Match(TokenType.System))
            return SystemStatement();
        if (Peek().Type == TokenType.Identifier && _natives.Get(Peek().Lexeme) != null && !Check(TokenType.LeftParen) && !Check(TokenType.Equal))
            return StatementWrapper(new StatementExpression(NativeStatementCall()));
        if (Peek().Type != TokenType.R || PeekNext().Type == TokenType.LeftParen)
            return Peek().Type == TokenType.Identifier ? LetStatement() : ExpressionStatement();

        Advance();
        return RunStatement();
    }

    private IStatement DefTypeStatement()
    {
        TokenType type = Previous().Type;
        var names = new List<string>();

        do
        {
            if (Peek().Type != TokenType.Identifier)
                _parseException = new ParseException(_lineNumber, _source,
                    Peek().LinePosition, "Expected variable name after type declaration.");

            string startName = Peek().Lexeme.ToUpperInvariant();
            Advance();

            if (Match(TokenType.Minus))
            {
                if (Peek().Type != TokenType.Identifier)
                    _parseException = new ParseException(_lineNumber, _source,
                        Peek().LinePosition, "Expected variable name after range operator.");

                string endName = Peek().Lexeme.ToUpperInvariant();
                Advance();
                names.AddRange(GetRangeNames(startName, endName));
            }
            else
            {
                names.Add(startName);
            }
        } while (Match(TokenType.Comma));

        return StatementWrapper(new DefType(type, names));
    }

    private bool IsDefFunction()
    {
        return Peek().Type == TokenType.Identifier
            && string.Equals(Peek().Lexeme, "DEF", StringComparison.OrdinalIgnoreCase)
            && PeekNext().Type == TokenType.Identifier
            && PeekNext().Lexeme.StartsWith("FN", StringComparison.OrdinalIgnoreCase);
    }

    private IStatement DefFunctionStatement()
    {
        Advance();
        Token function = Peek();
        Advance();
        Consume(TokenType.LeftParen, "Expected '(' after function name.");
        Token parameter = Peek();
        Consume(TokenType.Identifier, "Expected function parameter.");
        Consume(TokenType.RightParen, "Expected ')' after function parameter.");
        Consume(TokenType.Equal, "Expected '=' after function parameter.");
        return StatementWrapper(new DefFunction(function.Lexeme, parameter.Lexeme, Expression()));
    }

    private IStatement ClearStatement()
    {
        return StatementWrapper(new Clear());
    }

    private IStatement BeepStatement()
    {
        return StatementWrapper(new Beep());
    }

    private bool IsMidAssignment()
    {
        return Peek().Type == TokenType.Identifier
            && string.Equals(Peek().Lexeme, "MID$", StringComparison.OrdinalIgnoreCase)
            && PeekNext().Type == TokenType.LeftParen;
    }

    private IStatement MidAssignmentStatement()
    {
        Advance();
        Consume(TokenType.LeftParen, "Expected '(' after 'MID$'.");
        Expression target = Expression();
        Consume(TokenType.Comma, "Expected ',' after MID$ target.");
        Expression start = Expression();
        Expression length = null;
        if (Match(TokenType.Comma))
            length = Expression();
        Consume(TokenType.RightParen, "Expected ')' after MID$ arguments.");
        Consume(TokenType.Equal, "Expected '=' after MID$ arguments.");

        if (target is not Identifier identifier || !identifier.Name.Lexeme.EndsWith('$'))
        {
            _parseException = new ParseException(_lineNumber, _source, target.LinePosition,
                "MID$ target must be a string variable.");
            return StatementWrapper(new MidAssignment(new Identifier(Peek(), Peek().LinePosition), start, length, Expression()));
        }

        return StatementWrapper(new MidAssignment(identifier, start, length, Expression()));
    }

    private IStatement DimStatement()
    {
        var dimensions = new List<Expression>();

        do
        {
            if (Peek().Type != TokenType.Identifier)
                _parseException = new ParseException(_lineNumber, _source,
                    Peek().LinePosition, "Expected array name after 'DIM'.");

            Expression dimension = Expression();
            if (dimension is not Array)
                _parseException = new ParseException(_lineNumber, _source,
                    Peek().LinePosition, "Expected array declaration after 'DIM'.");

            dimensions.Add(dimension);
        } while (Match(TokenType.Comma));

        return StatementWrapper(new Dim(dimensions));
    }

    private static List<string> GetRangeNames(string startName, string endName)
    {
        if (string.IsNullOrEmpty(startName) || string.IsNullOrEmpty(endName) || startName.Length != 1 || endName.Length != 1)
            return new List<string> { startName };

        char start = char.ToUpperInvariant(startName[0]);
        char end = char.ToUpperInvariant(endName[0]);

        if (start > end)
            return new List<string> { startName };

        var names = new List<string>();
        for (char current = start; current <= end; current++)
            names.Add(current.ToString());

        return names;
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

        if (Match(TokenType.Goto, TokenType.Gosub))
        {
            isGosub = Previous().Type == TokenType.Gosub;
            do
            {
                Token current = Peek();
                locations.Add(Expression());
                linePositions.Add(current.LinePosition + current.Lexeme.Length);
            } while (Match(TokenType.Comma));
        }
        else
            _parseException = new ParseException(_lineNumber, _source,
            Peek().LinePosition, "Expected 'GOTO' or 'GOSUB' after variable in 'ON'");

        return StatementWrapper(new On(selector, locations, linePositions, isGosub));
    }

    private IStatement OutStatement()
    {
        Expression port = Expression();
        Consume(TokenType.Comma, "Expected ',' after OUT port.");
        Expression value = Expression();
        return StatementWrapper(new Out(port, value));
    }

    private IStatement WaitStatement()
    {
        Expression port = Expression();
        Consume(TokenType.Comma, "Expected ',' after WAIT port.");
        Expression mask = Expression();
        Expression invert = null;
        if (Match(TokenType.Comma))
            invert = Expression();

        return StatementWrapper(new Wait(port, mask, invert));
    }

    private IStatement StatementWrapper(IStatement statement)
    {
        statement.LineNumber = _lineNumber;
        statement.SourceLine = _source;
        statement.ParseException ??= _parseException;

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
        Token identifierName = Peek();
        if (Peek().Type != TokenType.Identifier)
            _parseException = new ParseException(_lineNumber, _source, Peek().LinePosition,
                "Expected variable name after 'NEXT'.");

        Expression identifier = Identifier();

        return StatementWrapper(new Next(identifierName, identifier));
    }

    private IStatement ForStatement()
    {
        Token identifierName = Peek();
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

        return StatementWrapper(new For(identifierName, identifier, startValue, endValue, stepValue));
    }

    private IStatement InputStatement()
    {
        var values = new List<Expression>();

        while (!IsAtStatementEnd())
        {
            values.Add(Expression());

            Match(TokenType.Semicolon);

            if (Match(TokenType.Comma))
                values.Add(
                    new Call(_padQuadrant,
                        new List<Expression>(),
                        0));
        }

        return StatementWrapper(new Input(values));
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

    private IStatement ExplicitDeleteStatement()
    {
        if (Match(TokenType.Minus))
        {
            Consume(TokenType.Number, "Expected ending line number after DELETE range.");
            return new Delete(-1, GetLineNumberValue(Previous()));
        }

        Consume(TokenType.Number, "Expected line number after DELETE.");
        int startLine = GetLineNumberValue(Previous());
        if (!Match(TokenType.Minus))
            return new Delete(startLine);

        if (IsAtEnd())
            return new Delete(startLine, int.MaxValue);

        Consume(TokenType.Number, "Expected ending line number after DELETE range.");
        return new Delete(startLine, GetLineNumberValue(Previous()));
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

        return StatementWrapper(new If(condition, current.LinePosition, thenBranch, thenException));
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
        if (Match(TokenType.Minus))
        {
            Consume(TokenType.Number, "Expected ending line number after LIST range.");
            return new List(new Literal(-1, null, 0), new Literal(Previous().Literal, null, Previous().LinePosition));
        }

        if (Check(TokenType.Number))
        {
            Token start = Peek();
            Advance();
            Expression startExpression = new Literal(start.Literal, null, start.LinePosition);
            if (Match(TokenType.Minus))
            {
                if (IsAtEnd())
                    return new List(startExpression);

                Consume(TokenType.Number, "Expected ending line number after LIST range.");
                Token end = Previous();
                return new List(startExpression, new Literal(end.Literal, null, end.LinePosition));
            }

            return new List(startExpression);
        }

        Expression value = !IsAtEnd() ? Expression() : new Literal(0, null, 0);
        return new List(value);
    }

    private IStatement LlistStatement()
    {
        var list = (List)ListStatement();
        return StatementWrapper(new Llist(list.StartAtLineNumber, list.EndAtLineNumber));
    }

    private IStatement SystemStatement()
    {
        return StatementWrapper(new SystemStatement());
    }

    private IStatement ResetStatement()
    {
        bool parenthesized = Match(TokenType.LeftParen);
        Expression x = Expression();
        Consume(TokenType.Comma, "Expected ',' after RESET x coordinate.");
        Expression y = Expression();
        if (parenthesized)
            Consume(TokenType.RightParen, "Expected ')' after RESET arguments.");
        return StatementWrapper(new ResetStatement(x, y));
    }

    private IStatement SetStatement()
    {
        bool parenthesized = Match(TokenType.LeftParen);
        Expression x = Expression();
        Consume(TokenType.Comma, "Expected ',' after SET x coordinate.");
        Expression y = Expression();
        if (parenthesized)
            Consume(TokenType.RightParen, "Expected ')' after SET arguments.");
        return StatementWrapper(new SetStatement(x, y));
    }

    private IStatement PokeStatement()
    {
        bool parenthesized = Match(TokenType.LeftParen);
        Expression address = Expression();
        Consume(TokenType.Comma, "Expected ',' after POKE address.");
        Expression value = Expression();
        if (parenthesized)
            Consume(TokenType.RightParen, "Expected ')' after POKE arguments.");
        return StatementWrapper(new PokeStatement(address, value));
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

    private Expression NativeStatementCall()
    {
        Token name = Peek();
        Advance();
        var arguments = new List<Expression>();
        var argumentPositions = new List<int> { name.LinePosition + name.Lexeme.Length };

        bool hasParentheses = Match(TokenType.LeftParen);
        if (hasParentheses)
        {
            if (!Check(TokenType.RightParen))
                do
                {
                    arguments.Add(Expression());
                    argumentPositions.Add(Previous().LinePosition + Previous().Lexeme.Length);
                }
                while (Match(TokenType.Comma));

            Consume(TokenType.RightParen, "Expected ')' after arguments");
        }
        else if (!IsAtStatementEnd())
        {
            do
            {
                arguments.Add(Expression());
                argumentPositions.Add(Peek().LinePosition);
            }
            while (Match(TokenType.Comma) && !IsAtStatementEnd());
        }

        List<Callable> callees = _natives.Get(name.Lexeme);
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
                linePosition = argumentPositions[Math.Min(arguments.Count, argumentPositions.Count - 1)];
            }

            pe = new ParseException(_lineNumber, _source, linePosition, $"Unknown function '{name.Lexeme}' with argument count {arguments.Count}");
        }

        Expression call = new Call(callee, arguments, name.LinePosition + name.Lexeme.Length);
        call.ParseException = pe;
        return call;
    }

    private IStatement LetStatement()
    {
        Token identifierToken = Peek();
        Token next = PeekNext();
        Expression identifier;

        if (identifierToken.Type == TokenType.Identifier)
        {
            if (next.Type != TokenType.Equal && _natives.Get(identifierToken.Lexeme) != null)
                return StatementWrapper(new StatementExpression(Call()));

            identifier = Identifier();
        }
        else
            identifier = Unary();

        if (identifierToken.Type == TokenType.Identifier
            && identifierToken.Lexeme.Contains("ON", StringComparison.OrdinalIgnoreCase))
            _parseException = new ParseException(_lineNumber, _source,
                identifierToken.LinePosition, "Invalid variable name.");

        Consume(TokenType.Equal, "Expected assignment.");

        if (!identifierToken.Lexeme.EndsWith('$')
            || Peek().Type == TokenType.String
            || Peek().Type == TokenType.Identifier)
            return StatementWrapper(new Let(identifier, Expression()));

        Expression unquoted = GetUnquotedExpression();

        return StatementWrapper(new Let(identifier, unquoted));
    }

    private Expression GetUnquotedExpression()
    {
        int startPosition = Peek().LinePosition;
        string unquotedString = GetUnquotedString();

        Expression literal = null;

        if (unquotedString.Length > 1 &&
            (!unquotedString.EndsWith('$') || unquotedString.Length > 2))
            literal = new Literal(unquotedString, unquotedString.ToUpperInvariant(), startPosition);

        return literal;
    }

    private string GetUnquotedString()
    {
        Token start = Peek();
        while (Peek().Type != TokenType.Colon && !IsAtEnd())
            Advance();

        Token end = Previous();
        string unquotedString = _source.Substring(start.LinePosition,
            end.LinePosition + end.Lexeme.Length - start.LinePosition);
        return unquotedString;
    }

    private Expression Identifier()
    {
        Token current = Peek();

        Advance();

        if (!Match(TokenType.LeftParen))
            return new Identifier(current, current.LinePosition + 1);

        Expression index = Expression();
        Expression index2 = null;
        if (Match(TokenType.Comma))
            index2 = Expression();

        Token previous = Previous();
        int linePosition = previous.LinePosition + previous.Lexeme.Length;

        Consume(TokenType.RightParen, "Expected ')' after array index");

        return index2 == null ? new Array(current, index, linePosition) : new Array(current, index, index2, linePosition);
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
                    _parseException = new ParseException(_lineNumber, _source,
                        Peek().LinePosition, "Expected ',' or ';' after AT clause.");
            }
#pragma warning restore S1066 // Collapsible "if" statements should be merged

        while (!IsAtStatementEnd())
        {
            values.Add(Expression());

            if (Match(TokenType.Semicolon) && IsAtStatementEnd())
                newline = false;
            if (Match(TokenType.Comma))
                values.Add(
                    new Call(_padQuadrant,
                        new List<Expression>(),
                        0));

        }

        return StatementWrapper(new Print(atPosition, values, newline));
    }

    private IStatement LprintStatement()
    {
        var print = (Print)PrintStatement();
        return StatementWrapper(new Lprint(print.AtPosition, print.Expressions, print.WriteNewline));
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
        return Imp();
    }

    private Expression Imp()
    {
        Expression left = Eqv();
        while (Match(TokenType.Imp))
        {
            Token operatorType = Previous();
            Expression right = Eqv();
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
    }

    private Expression Eqv()
    {
        Expression left = Xor();
        while (Match(TokenType.Eqv))
        {
            Token operatorType = Previous();
            Expression right = Xor();
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
    }

    private Expression Xor()
    {
        Expression left = Or();
        while (Match(TokenType.Xor))
        {
            Token operatorType = Previous();
            Expression right = Or();
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
    }

    private Expression Or()
    {
        Expression left = And();
        while (Match(TokenType.Or))
        {
            Token operatorType = Previous();
            Expression right = And();
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
    }

    private Expression And()
    {
        Expression left = Comparison();
        while (Match(TokenType.And))
        {
            Token operatorType = Previous();
            Expression right = Comparison();
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
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
        while (Match(TokenType.Slash, TokenType.Star, TokenType.Mod))
        {
            Token operatorType = Previous();
            Expression right = Unary();
            left = new Binary(left, operatorType, right, operatorType.LinePosition);
        }

        return left;
    }

    private Expression Unary()
    {
        if (Match(TokenType.Not))
        {
            Token operatorType = Previous();
            Expression right = Unary();
            return new Unary(operatorType, right, operatorType.LinePosition);
        }

        if (!Match(TokenType.Minus, TokenType.Plus)) return Call();

        Token operatorType2 = Previous();
        Expression right2 = Unary();
        return new Unary(operatorType2, right2, operatorType2.LinePosition);
    }

    private Expression Call()
    {
        Token name = Peek();

        Expression expression = Primary();
        List<Callable> callees = _natives.Get(name.Lexeme);

        if (Match(TokenType.LeftParen) && expression is Identifier)
        {
            if (callees != null)
                return FinishCall(name, callees);
            if (name.Lexeme.StartsWith("FN", StringComparison.OrdinalIgnoreCase))
                return FinishUserCall(name);
            return FinishArray(name);
        }

        Token previous = Previous();
        if (previous.Type != TokenType.Identifier) return expression;

        callees = _natives.Get(previous.Lexeme);
        if (callees == null) return expression;

        Callable callee = callees.FirstOrDefault(f => f.Arity == 0);

        if (callee != null)
            return new Call(callee, new List<Expression>(), Peek().LinePosition);

        var pe = new ParseException(_lineNumber, _source, Peek().LinePosition, $"Invalid number of arguments passed to function '{previous.Lexeme}'");
        callee = callees.First();
        Expression call = new Call(callee, new List<Expression>(), Peek().LinePosition);
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

    private Expression FinishUserCall(Token name)
    {
        var arguments = new List<Expression>();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                arguments.Add(Expression());
            }
            while (Match(TokenType.Comma));
        }

        Consume(TokenType.RightParen, "Expected ')' after function arguments.");
        return new Call(name.Lexeme, arguments, name.LinePosition);
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

        Expression call = new Call(callee, arguments, Peek().LinePosition);
        call.ParseException = pe;

        return call;
    }

    private Expression FinishArray(Token name)
    {
        Expression index = Expression();
        Expression index2 = null;
        if (Match(TokenType.Comma))
            index2 = Expression();

        Token previous = Previous();
        int linePosition = previous.LinePosition + previous.Lexeme.Length;

        Consume(TokenType.RightParen,
            "Expected ')' after arguments");

        return index2 == null ? new Array(name, index, linePosition) : new Array(name, index, index2, linePosition);
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
            return new Identifier(previous, previous.LinePosition + 1);
        }

        if (!IsIdentifierShortHand())
        {
            Token unexpected = Peek();
            Advance();
            Expression literal = new Literal(unexpected.Lexeme, unexpected.Lexeme, unexpected.LinePosition);
            literal.ParseException = new ParseException(_lineNumber, _source, unexpected.LinePosition, "Expected expression.");
            return literal;
        }

        Token current = Peek();
        Advance();
        var identifier = new Token(TokenType.Identifier, current.Lexeme, current.Lexeme, _source, current.LinePosition);
        return new Identifier(identifier, current.LinePosition + 1);
    }

    private bool IsIdentifierShortHand()
    {
        Token token = Peek();

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