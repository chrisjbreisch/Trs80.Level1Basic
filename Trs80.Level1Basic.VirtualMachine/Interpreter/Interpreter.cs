using System;
using System.Linq;
using System.Text;
using System.Threading;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.Common.Extensions;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

using Array = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Array;
using Void = Trs80.Level1Basic.Common.Void;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Interpreter : IInterpreter
{
    private readonly IMachine _machine;
    private readonly ITrs80 _trs80;
    private readonly ITrs80Api _trs80Api;
    private readonly IProgram _program;
    private readonly IHost _host;
    private readonly IAppSettings _appSettings;

    public Interpreter(IHost host, ITrs80 trs80, ITrs80Api trs80Api,
        IMachine machine, IProgram program, IAppSettings appSettings)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _trs80Api = trs80Api ?? throw new ArgumentNullException(nameof(trs80Api));
        _machine = machine ?? throw new ArgumentNullException(nameof(machine));
        _program = program ?? throw new ArgumentNullException(nameof(program));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    public void Interpret(IStatement statement)
    {
        if (statement == null) return;

        try
        {
            Execute(statement.LineNumber >= 0 ? new Replace(statement) : statement);
        }
        catch (Exception ex)
        {
            ExceptionHandler.HandleError(_trs80, _appSettings, ex);
        }

    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public dynamic VisitArrayExpression(Array expression)
    {
        dynamic index = Evaluate(expression.Index);
        if (index > _trs80Api.Mem() / 4 - 1)
            throw new ProgramTooLargeException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, expression.LinePosition, "Insufficient memory.");
        return _machine.Get(expression.Name.Lexeme, index);
    }

    private void Assign(Expression expression, dynamic value)
    {
        switch (expression)
        {
            case Identifier identifier:
                if (!identifier.Name.Lexeme.EndsWith('$') && value is string)
                    throw new ValueOutOfRangeException(-1, string.Empty, string.Empty);

                _machine.Set(identifier.Name.Lexeme, value);
                break;
            case Array array:
                {
                    dynamic index = Evaluate(array.Index);
                    _machine.Set(array.Name.Lexeme, index, value);
                    break;
                }
        }
    }

    public dynamic VisitBinaryExpression(Binary expression)
    {
        dynamic left = Evaluate(expression.Left);
        dynamic right = Evaluate(expression.Right);

        CheckOperands(expression.BinaryOperator, left, right);

        return expression.BinaryOperator.Type switch
        {
            TokenType.Plus => (left is bool && right is bool) ? left || right : left + right,
            TokenType.Minus => left - right,
            TokenType.Slash => right == 0 ? throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber, _program.CurrentStatement.SourceLine, "Divide by zero") : (float)left / right,
            TokenType.Star => (left is bool && right is bool) ? left && right : left * right,
            TokenType.GreaterThan => left > right,
            TokenType.GreaterThanOrEqual => left >= right,
            TokenType.LessThan => left < right,
            TokenType.LessThanOrEqual => left <= right,
            TokenType.NotEqual => !IsEqual(left, right),
            TokenType.Equal => IsEqual(left, right),
            _ => null
        };
    }

    public dynamic VisitCallExpression(Call expression)
    {
        var arguments = expression.Arguments.Select(argument => Evaluate(argument)).ToList();

        return expression.Callee.Call(_trs80Api, arguments);
    }

    public dynamic VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    public dynamic VisitIdentifierExpression(Identifier expression)
    {
        string name = expression.Name.Lexeme;
        if (name.Length > 1 &&
            (!name.EndsWith('$') || name.Length > 2))
            throw new ParseException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, expression.LinePosition, "Invalid Identifier.");

        return _machine.Get(name);
    }

    public dynamic VisitLiteralExpression(Literal expression)
    {
        switch (expression.Value)
        {
            case float:
                return expression.Value;
            case string:
                return expression.UpperValue;
        }

        if (expression.Value > short.MaxValue || expression.Value < short.MinValue)
            // ReSharper disable once PossibleInvalidCastException
            return (float)expression.Value;

        return expression.Value;
    }

    public dynamic VisitSelectorExpression(Selector expression)
    {
        return Evaluate(expression.Expression);
    }

    public dynamic VisitUnaryExpression(Unary expression)
    {
        dynamic right = Evaluate(expression.Right);

        CheckNumericOperand(expression.UnaryOperator, right);
        return -1 * right;
    }

    private void CheckNumericOperand(Token operatorType, dynamic operand)
    {
        switch (operand)
        {
            case float:
            case int:
                return;
            default:
                throw new RuntimeExpressionException(_program.CurrentStatement.LineNumber,
                    _program.CurrentStatement.SourceLine, operatorType.LinePosition + 1,
                    "Operand must be a number.");
        }
    }

    private void CheckOperands(Token operatorType, dynamic left, dynamic right)
    {
        switch (left)
        {
            case bool when right is bool:
            case float when right is float:
            case float when right is int:
            case int when right is float:
            case int when right is int:
                return;
            default:
                throw new RuntimeExpressionException(_program.CurrentStatement.LineNumber,
                    _program.CurrentStatement.SourceLine, operatorType.LinePosition,
                    "Operands are of incompatible types.");
        }
    }

    private static bool IsTruthy(dynamic value)
    {
        if (value == null) return false;

        if (value is int intVal)
            return intVal == 1;

        return value;
    }

    private static bool IsEqual(dynamic left, dynamic right)
    {
        if (left == null && right == null) return true;
        return left != null && (bool)left.Equals(right);
    }

    private string Stringify(dynamic value)
    {
        StringBuilder sb = new();
        if (value is >= 0 or float and >= 0)
            sb.Append(' ');

        switch (value)
        {
            case float:
                sb.Append(StringifyFloat(value));
                break;
            case int:
                sb.Append(StringifyInt(value));
                break;
            case bool:
                sb.Append(value ? " 1 " : " 0 ");
                break;
            default:
                sb.Append(value.ToString());
                break;
        }

        if (value is (int or float))
            sb.Append(' ');

        _machine.CursorX += sb.Length;
        return sb.ToString();
    }

    private string StringifyInt(int value)
    {
        return value is <= 1000000 and >= -1000000 ? value.ToString() : value.ToString("0.#####E+00");
    }

    private string StringifyFloat(float value)
    {
        switch (value)
        {
            case 0:
                return "0";
            case < .1f and > -.1f:
                return value.ToString("0.#####E+00");
            case < 1 and > -1:
                return value.ToString(".######");
            case > 999999:
            case < -999999:
                return value.ToString("0.#####E+00");
            case <= -100000:
            case >= 100000:
                return value.ToString("######");
            default:
                {
                    string result = value.ToString("######.#####");
                    return result.Left(value < 0 ? 8 : 7);
                }
        }
    }

    public void Execute(IStatement statement)
    {
        _program.CurrentStatement = statement;
        statement.Accept(this);
    }

    public Void VisitClsStatement(Cls statement)
    {
        _trs80.Clear();
        Thread.Sleep(500);

        return null!;
    }

    public Void VisitCompoundStatement(Compound statement)
    {
        _machine.RunCompoundStatement(statement.Statements, this);

        return null!;
    }

    public Void VisitContStatement(Cont statement)
    {
        RunProgram(_machine.GetNextStatement(), false);

        return null!;
    }

    public void RunProgram(IStatement statement, bool initialize)
    {
        if (initialize)
            _machine.Initialize();

        _machine.RunStatementList(statement, this);

        WritePrompt();
    }

    public Void VisitDataStatement(Data statement)
    {
        foreach (Expression element in statement.DataElements)
            _machine.Data.Add(Evaluate(element));

        return null!;
    }

    public Void VisitDeleteStatement(Delete statement)
    {
        DeleteStatement(statement.LineToDelete);

        return null!;
    }

    private void DeleteStatement(int lineNumber)
    {
        IStatement statement = _machine.Program.List().FirstOrDefault(l => l.LineNumber == lineNumber);
        if (statement != null)
            _machine.Program.RemoveStatement(statement);
    }

    public Void VisitEndStatement(End statement)
    {
        _machine.HaltRun();

        return null!;
    }

    public Void VisitForStatement(For statement)
    {
        if (statement.Identifier is not Identifier && statement.Identifier is not Array)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Identifier.LinePosition, "Expected variable after 'FOR'.");

        if (!_machine.Exists(statement.IdentifierName.Lexeme))
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Identifier.LinePosition, "Invalid identifier after 'FOR'.");

        dynamic current = Evaluate(statement.StartValue);
        Assign(statement.Identifier, current);

        dynamic end = Evaluate(statement.EndValue);
        dynamic step = Evaluate(statement.StepValue);
        IStatement next = _machine.GetNextStatement(statement);

        do
            try
            {
                _machine.RunStatementList(next, this);
            }
            catch (LoopAfterNext lan)
            {
                if (lan.Next.IdentifierName.Lexeme != statement.IdentifierName.Lexeme)
                    throw;

                current = IncrementIndexer(statement.Identifier, step);
            }
        while ((!_machine.ExecutionHalted) &&
            step > 0 && current <= end || step < 0 && current >= end);

        return null!;
    }

    public Void VisitGosubStatement(Gosub statement)
    {
        IStatement resumeStatement = _machine.GetNextStatement(_program.CurrentStatement);

        IStatement jumpToStatement = GetJumpToStatement(statement, statement.Location, statement.LinePosition, "GOSUB");
        ExecuteGosub(jumpToStatement, resumeStatement);

        return null!;
    }

    private IStatement GetJumpToStatement(Statement statement, Expression location, int linePosition, string jumpType)
    {
        dynamic jumpToLineNumber = Evaluate(location);

        IStatement jumpToStatement = GetStatementByLineNumber(jumpToLineNumber);

        if (jumpToStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                linePosition, $"Can't {jumpType} line {jumpToLineNumber}");

        return jumpToStatement;
    }

    private IStatement GetStatementByLineNumber(int lineNumber)
    {
        return _machine.GetStatementByLineNumber(lineNumber);
    }

    public Void VisitGotoStatement(Goto statement)
    {
        IStatement jumpToStatement = GetJumpToStatement(statement, statement.Location, statement.LinePosition, "GOTO");
        _machine.SetNextStatement(jumpToStatement);
        return null!;
    }

    public Void VisitIfStatement(If statement)
    {
        dynamic logicalExpression = Evaluate(statement.Condition);
        if (logicalExpression is string)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.ThenPosition, "Cannot convert string to logical expression.");

        if (!IsTruthy(logicalExpression)) return null!;

        if (statement.ThenException != null)
            throw statement.ThenException;

        _machine.RunCompoundStatement(statement.ThenBranch, this);

        return null!;
    }

    public Void VisitInputStatement(Input statement)
    {
        foreach (Expression expression in statement.Expressions)
            ProcessInputExpression(expression);

        return null!;
    }

    private void ProcessInputExpression(Expression expression)
    {
        switch (expression)
        {
            case Literal:
                _trs80.Write(Stringify(Evaluate(expression)));
                break;
            case Identifier variable:
                GetInputValue(variable);
                break;
            case Array array:
                GetInputValue(array);
                break;
        }
    }

    private void GetInputValue(Expression identifier)
    {
        _trs80.Write("?");

        string value = _trs80.ReadLine();
        if (value is null)
            Assign(identifier, null);
        else if (int.TryParse(value, out int intValue))
            Assign(identifier, intValue);
        else if (float.TryParse(value, out float floatValue))
            Assign(identifier, floatValue);
        else if (_machine.Exists(value))
        {
            dynamic lookup = _machine.Get(value);
            Assign(identifier, lookup);
        }
        else
            try
            {
                Assign(identifier, value);
            }
            catch (ValueOutOfRangeException)
            {
                _trs80.WriteLine("WHAT?");
                GetInputValue(identifier);
            }
    }

    public Void VisitLetStatement(Let statement)
    {
        if (statement.Variable is not Identifier && statement.Variable is not Array)
            throw new ParseException(statement.LineNumber,
                statement.SourceLine, statement.Variable.LinePosition,
                "Expected variable name or function call.");

        dynamic value = null;
        if (statement.Initializer != null)
            value = Evaluate(statement.Initializer);

        Assign(statement.Variable, value);

        return null!;
    }

    public Void VisitListStatement(List statement)
    {
        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        _machine.ListProgram(lineNumber);

        return null!;
    }

    private int GetStartingLineNumber(Expression startAtLineNumber)
    {
        int lineNumber = -1;
        dynamic value = Evaluate(startAtLineNumber);
        if (value != null)
            lineNumber = (int)value;
        return lineNumber;
    }

    public Void VisitLoadStatement(Load statement)
    {
        _machine.NewProgram();
        if (statement.Path is not Literal literalPath)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Path.LinePosition, "Path must be a quoted string.");
        string path = literalPath.Value;

        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForLoad();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.LoadProgram(path);
        _trs80.WriteLine($"Loaded \"{path}\".");

        return null!;
    }
    
    public Void VisitMergeStatement(Merge statement)
    {
        if (statement.Path is not Literal literalPath)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Path.LinePosition, "Path must be a quoted string.");
        string path = literalPath.Value;

        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForLoad();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.LoadProgram(path);
        _trs80.WriteLine($"Merged \"{path}\".");

        return null!;
    }

    public Void VisitNewStatement(New statement)
    {
        _machine.Program.Clear();

        return null!;
    }

    public Void VisitNextStatement(Next statement)
    {
        throw new LoopAfterNext(statement);
    }

    private dynamic IncrementIndexer(Expression identifier, int step)
    {
        dynamic currentValue = Evaluate(identifier);
        dynamic newValue = currentValue + step;
        Assign(identifier, newValue);
        return newValue;
    }

    public Void VisitOnStatement(On statement)
    {
        int selector = (int)Math.Floor((float)Evaluate(statement.Selector)) - 1;
        var locations = statement.Locations.Select(location => Evaluate(location)).ToList();
        System.Collections.Generic.List<int> linePositions = statement.LinePositions;

        if (selector >= locations.Count || selector < 0) return null!;

        if (statement.IsGosub)
        {
            IStatement resumeStatement = _machine.GetNextStatement(statement);
            Expression location = new Literal(locations[selector], null, linePositions[selector]);
            IStatement jumpToStatement = GetJumpToStatement(statement, location, linePositions[selector], "GOSUB");
            ExecuteGosub(jumpToStatement, resumeStatement);

            return null!;
        }

        IStatement nextStatement = GetStatementByLineNumber(locations[selector]);

        if (nextStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                linePositions[selector], $"Can't 'GOTO' line {locations[selector]}.");

        _machine.SetNextStatement(nextStatement);

        return null!;
    }

    private void ExecuteGosub(IStatement jumpToStatement, IStatement resumeStatement)
    {
        try
        {
            _machine.RunStatementList(jumpToStatement, this);
        }
        catch (ReturnFromGosub)
        {
            _machine.SetNextStatement(resumeStatement);
        }
    }

    public Void VisitPrintStatement(Print statement)
    {
        if (statement.AtPosition != null)
            PrintAt(statement.AtPosition);

        if (statement.Expressions is { Count: > 0 })
            foreach (Expression expression in statement.Expressions)
                _trs80.Write(Stringify(Evaluate(expression)));

        if (!statement.WriteNewline) return null!;

        _trs80.WriteLine();
        _machine.CursorX = 0;
        _machine.CursorY++;

        return null!;
    }

    private void PrintAt(Expression position)
    {
        dynamic value = Evaluate(position);
        dynamic row = value / 64;
        dynamic column = value % 64;

        _trs80.SetCursorPosition(column, row);

        _machine.CursorX = column;
        _machine.CursorY = row;
    }

    public Void VisitReadStatement(Read statement)
    {
        foreach (Expression variable in statement.Variables)
        {
            dynamic value = _machine.Data.GetNext();
            if (variable is Identifier identifier && !identifier.Name.Lexeme.EndsWith('$') && value is string)
                Assign(variable, 0);
            else
                Assign(variable, value);
        }

        return null!;
    }

    public Void VisitRemStatement(Rem statement)
    {
        // do nothing
        return null!;
    }

    public Void VisitReplaceStatement(Replace statement)
    {
        _machine.Program.ReplaceStatement(statement.Statement);

        return null!;
    }

#pragma warning disable S927 // Parameter names should match base declaration and other partial definitions
    public Void VisitRestoreStatement(Restore _)
#pragma warning restore S927 // Parameter names should match base declaration and other partial definitions
    {
        _machine.Data.MoveFirst();

        return null!;
    }

    public Void VisitReturnStatement(Return statement)
    {
        throw new ReturnFromGosub();
    }

    public Void VisitRunStatement(Run statement)
    {
        _machine.InitializeProgram();

        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        if (lineNumber < 0)
            lineNumber = GetFirstLineNumber();
        if (lineNumber >= 0)
        {
            _machine.LoadData(this);
            IStatement firstStatement = GetStatementByLineNumber(lineNumber);
            if (firstStatement is null)
                WritePrompt();
            else
                RunProgram(firstStatement, true);
        }
        else
            WritePrompt();

        return null!;
    }

    private void WritePrompt()
    {
        _trs80.WriteLine();
        _trs80.WriteLine("READY");
    }

    private int GetFirstLineNumber()
    {
        IStatement statement = _machine.Program.GetFirstStatement();

        if (statement is null)
            return -1;

        return statement.LineNumber;
    }

    public Void VisitSaveStatement(Save statement)
    {
        if (statement.Path is not Literal literalPath)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Path.LinePosition, "Path must be a quoted string.");
        string path = literalPath.Value;
        
        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForSave();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.SaveProgram(path);
        _trs80.WriteLine($"Saved \"{path}\".");

        return null!;
    }

    public Void VisitStatementExpressionStatement(StatementExpression statement)
    {
        if (statement.Expression is Call)
            Evaluate(statement.Expression);
        else
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Expression.LinePosition, "Expected statement.");

        return null!;
    }

    public Void VisitStopStatement(Stop statement)
    {
        _trs80.WriteLine($"BREAK AT {statement.LineNumber}");
        _machine.HaltRun();

        return null!;
    }
}