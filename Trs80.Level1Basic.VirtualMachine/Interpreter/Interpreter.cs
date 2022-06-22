using System;
using System.Linq;
using System.Text;
using System.Threading;

using Trs80.Level1Basic.Common.Extensions;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Interpreter : IInterpreter
{
    private readonly IMachine _machine;
    private readonly ITrs80 _trs80;
    private readonly ITrs80Api _trs80Api;
    private readonly IProgram _program;
    private readonly IHost _host;

    public Interpreter(IHost host, ITrs80 trs80, ITrs80Api trs80Api, IMachine machine, IProgram program)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _trs80Api = trs80Api ?? throw new ArgumentNullException(nameof(trs80Api));
        _machine = machine ?? throw new ArgumentNullException(nameof(machine));
        _program = program ?? throw new ArgumentNullException(nameof(program));
    }

    public void Interpret(IStatement statement)
    {
        try
        {
            Execute(statement.LineNumber >= 0 ? new Replace(statement) : statement);
        }
        catch (Exception ex)
        {
            ExceptionHandler.HandleError(_trs80, ex);
        }

    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public dynamic VisitArrayExpression(Parser.Expressions.Array expression)
    {
        dynamic index = Evaluate(expression.Index);
        return _machine.Get(expression.Name.Lexeme, index);
    }

    public dynamic VisitAssignExpression(Assign expression)
    {
        dynamic value = Evaluate(expression.Value);
        return Assign(expression, value);
    }

    private dynamic Assign(Expression expression, dynamic value)
    {
        switch (expression)
        {
            case Identifier identifier:
                _machine.Set(identifier.Name.Lexeme, value);
                break;
            case Parser.Expressions.Array array:
                {
                    dynamic index = Evaluate(array.Index);
                    _machine.Set(array.Name.Lexeme, index, value);
                    break;
                }
            default:
                throw new RuntimeExpressionException(null, "Expected variable.");
        }

        return value;
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
            throw new ParseException(_program.CurrentStatement.LineNumber, _program.CurrentStatement.SourceLine, "Invalid Identifier.", expression.LinePosition);

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

    public dynamic VisitUnaryExpression(Unary expression)
    {
        dynamic right = Evaluate(expression.Right);

        CheckNumericOperand(expression.UnaryOperator, right);
        return -1 * right;
    }

    private static void CheckNumericOperand(Token operatorType, dynamic operand)
    {
        switch (operand)
        {
            case float:
            case int:
                return;
            default:
                throw new RuntimeExpressionException(operatorType, "Operand must be a number.");
        }
    }

    private static void CheckOperands(Token operatorType, dynamic left, dynamic right)
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
                throw new RuntimeExpressionException(operatorType, "Operands are of incompatible types.");
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
            case null:
                return "";
            case float:
                sb.Append(StringifyFloat(value));
                break;
            case int:
                sb.Append(StringifyInt(value));
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
        if (value == 0)
            return "0";
        if (value < .1 && value > -.1)
            return value.ToString("0.#####E+00");
        if (value is < 1 and > -1)
            return value.ToString(".######");
        if (value > 999999 || value < -999999)
            return value.ToString("0.#####E+00");
        if (value <= -100000 || value >= 100000)
            return value.ToString("######");

        string result = value.ToString("######.#####");
        return result.Left(value < 0 ? 8 : 7);
    }

    public void Execute(IStatement statement)
    {
        _program.CurrentStatement = statement;
        statement.Accept(this);
    }

    public Common.Void VisitClsStatement(Cls statement)
    {
        _trs80.Clear();
        Thread.Sleep(500);

        return null!;
    }

    public Common.Void VisitCompoundStatement(Compound statement)
    {
        _machine.RunCompoundStatement(statement.Statements, this);

        return null!;
    }

    public Common.Void VisitContStatement(Cont statement)
    {
        RunProgram(_machine.GetNextStatement(), false);

        return null!;
    }

    public void RunProgram(IStatement statement, bool initialize)
    {
        if (initialize)
            _machine.Initialize();

        _machine.RunStatementList(statement, this);
        _trs80.WriteLine();
        _trs80.WriteLine("READY");
    }

    public Common.Void VisitDataStatement(Data statement)
    {
        foreach (Expression element in statement.DataElements)
            _machine.Data.Add(Evaluate(element));

        return null!;
    }

    public Common.Void VisitDeleteStatement(Delete statement)
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

    public Common.Void VisitEndStatement(End statement)
    {
        _machine.HaltRun();

        return null!;
    }

    public Common.Void VisitForStatement(For statement)
    {
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
            catch (LoopAfterNext)
            {
                current = IncrementIndexer(statement.Identifier, step);
            }
        while (step > 0 && current <= end || step < 0 && current >= end);

        return null!;
    }

    public Common.Void VisitGosubStatement(Gosub statement)
    {
        IStatement resumeStatement = _machine.GetNextStatement(_program.CurrentStatement);

        IStatement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOSUB");
        ExecuteGosub(jumpToStatement, resumeStatement);

        return null!;
    }

    private IStatement GetJumpToStatement(Statement statement, Expression location, string jumpType)
    {
        dynamic jumpToLineNumber = Evaluate(location);

        IStatement jumpToStatement = GetStatementByLineNumber(jumpToLineNumber);

        if (jumpToStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                $"Can't {jumpType} line {jumpToLineNumber}");

        return jumpToStatement;
    }

    private IStatement GetStatementByLineNumber(int lineNumber)
    {
        return _machine.GetStatementByLineNumber(lineNumber);
    }

    public Common.Void VisitGotoStatement(Goto statement)
    {
        IStatement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOTO");
        _machine.SetNextStatement(jumpToStatement);
        return null!;
    }

    public Common.Void VisitIfStatement(If statement)
    {
        if (!IsTruthy(Evaluate(statement.Condition))) return null!;

        _machine.RunCompoundStatement(statement.ThenBranch, this);

        return null!;
    }

    public Common.Void VisitInputStatement(Input statement)
    {
        foreach (Expression expression in statement.Expressions)
            ProcessInputExpression(expression, statement.WriteNewline);

        return null!;
    }

    private void ProcessInputExpression(Expression expression, bool writeNewline)
    {
        switch (expression)
        {
            case Literal:
                _trs80.Write(Stringify(Evaluate(expression)));
                break;
            case Identifier variable:
                GetInputValue(variable, writeNewline);
                break;
            case Parser.Expressions.Array array:
                GetInputValue(array, writeNewline);
                break;
        }
    }

    private void GetInputValue(Expression variable, bool writeNewline)
    {
        _trs80.Write("?");

        if (writeNewline)
            _trs80.WriteLine();

        string value = _trs80.ReadLine();
        if (value is null)
            Assign(variable, null);
        else if (int.TryParse(value, out int intValue))
            Assign(variable, intValue);
        else if (float.TryParse(value, out float floatValue))
            Assign(variable, floatValue);
        else if (_machine.Exists(value))
        {
            dynamic lookup = _machine.Get(value);
            Assign(variable, lookup);
        }
        else
            try
            {
                Assign(variable, value);
            }
            catch (ValueOutOfRangeException)
            {
                _trs80.WriteLine(" 0 WHAT?");
                GetInputValue(variable, writeNewline);
            }
    }

    public Common.Void VisitLetStatement(Let statement)
    {
        dynamic value = null;
        if (statement.Initializer != null)
            value = Evaluate(statement.Initializer);

        Assign(statement.Variable, value);

        return null!;
    }

    public Common.Void VisitListStatement(List statement)
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

    public Common.Void VisitLoadStatement(Load statement)
    {
        _machine.NewProgram();
        string path = EvaluatePath(statement.Path);

        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForLoad();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.LoadProgram(path);
        _trs80.WriteLine($"Loaded \"{path}\".");

        return null!;
    }

    private string EvaluatePath(Expression pathExpression)
    {
        string path;
        if (pathExpression is Literal literalPath)
            path = literalPath.Value;
        else
            path = Evaluate(pathExpression);
        return path;
    }

    public Common.Void VisitMergeStatement(Merge statement)
    {
        string path = EvaluatePath(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForLoad();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.LoadProgram(path);
        _trs80.WriteLine($"Merged \"{path}\".");

        return null!;
    }

    public Common.Void VisitNewStatement(New statement)
    {
        _machine.Program.Clear();

        return null!;
    }

    public Common.Void VisitNextStatement(Next statement)
    {
        throw new LoopAfterNext();
    }

    private dynamic IncrementIndexer(Expression identifier, int step)
    {
        dynamic currentValue = Evaluate(identifier);
        dynamic newValue = currentValue + step;
        Assign(identifier, newValue);
        return newValue;
    }

    public Common.Void VisitOnStatement(On statement)
    {
        int selector = (int)Math.Floor((float)Evaluate(statement.Selector)) - 1;
        var locations = statement.Locations.Select(location => Evaluate(location)).ToList();

        if (selector >= locations.Count || selector < 0) return null!;

        if (statement.IsGosub)
        {
            IStatement resumeStatement = _machine.GetNextStatement(statement);
            Expression location = new Literal(locations[selector], null);
            IStatement jumpToStatement = GetJumpToStatement(statement, location, "GOSUB");
            ExecuteGosub(jumpToStatement, resumeStatement);

            return null!;
        }

        IStatement nextStatement = GetStatementByLineNumber(locations[selector]);

        if (nextStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                $"Can't 'GOTO' line {locations[selector]}.");

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

    public Common.Void VisitPrintStatement(Print statement)
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

    public Common.Void VisitReadStatement(Read statement)
    {
        foreach (Expression variable in statement.Variables)
            Assign(variable, _machine.Data.GetNext());

        return null!;
    }

    public Common.Void VisitRemStatement(Rem statement)
    {
        // do nothing
        return null!;
    }

    public Common.Void VisitReplaceStatement(Replace statement)
    {
        if (string.IsNullOrEmpty(statement.Statement.SourceLine))
            DeleteStatement(statement.Statement.LineNumber);
        else
            _machine.Program.ReplaceStatement(statement.Statement);

        return null!;
    }

#pragma warning disable S927 // Parameter names should match base declaration and other partial definitions
    public Common.Void VisitRestoreStatement(Restore _)
#pragma warning restore S927 // Parameter names should match base declaration and other partial definitions
    {
        _machine.Data.MoveFirst();

        return null!;
    }

    public Common.Void VisitReturnStatement(Return statement)
    {
        throw new ReturnFromGosub();
    }

    public Common.Void VisitRunStatement(Run statement)
    {
        _machine.InitializeProgram();

        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        if (lineNumber < 0)
            lineNumber = GetFirstLineNumber();
        if (lineNumber < 0) return null!;

        _machine.LoadData(this);
        IStatement firstStatement = GetStatementByLineNumber(lineNumber);
        if (firstStatement is null)
            throw new RuntimeStatementException(-1, statement.SourceLine, $"Can't start execution at {lineNumber}");

        RunProgram(firstStatement, true);

        return null!;
    }

    private int GetFirstLineNumber()
    {
        IStatement statement = _machine.Program.GetFirstStatement();

        if (statement is null)
            return -1;

        return statement.LineNumber;
    }

    public Common.Void VisitSaveStatement(Save statement)
    {
        string path = EvaluatePath(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForSave();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.SaveProgram(path);
        _trs80.WriteLine($"Saved \"{path}\".");

        return null!;
    }

    public Common.Void VisitStatementExpressionStatement(StatementExpression statement)
    {
        Evaluate(statement.Expression);

        return null!;
    }

    public Common.Void VisitStopStatement(Stop statement)
    {
        _trs80.WriteLine($"BREAK AT {statement.LineNumber}");
        _machine.HaltRun();

        return null!;
    }
}