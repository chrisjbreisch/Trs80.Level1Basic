using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

using Array = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Array;
using Expression = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Expression;
using Void = Trs80.Level1Basic.Common.Void;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Interpreter : IInterpreter
{
    private readonly IMachine _machine;
    private readonly ITrs80 _trs80;
    private readonly IProgram _program;
    private readonly IHost _host;

    public Interpreter(IHost host, ITrs80 trs80, IMachine machine, IProgram program)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _machine = machine ?? throw new ArgumentNullException(nameof(machine));
        _program = program ?? throw new ArgumentNullException(nameof(program));
    }

    public void Interpret(IStatement statement)
    {
        Execute(statement.LineNumber >= 0 ? new Replace(statement) : statement);
    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public dynamic VisitArrayExpression(Array expression)
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
                _machine.Assign(identifier.Name.Lexeme, value);
                break;
            case Array array:
                {
                    dynamic index = Evaluate(array.Index);
                    _machine.Assign(array.Name.Lexeme, index, value);
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
            TokenType.Slash => right == 0 ? throw new ValueOutOfRangeException(0, "", "Divide by zero") : (float)left / right,
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

        ICallable function = _machine.Function(expression.Callee.Lexeme).First(f => f.Arity == arguments.Count);

        return function.Call(_trs80, arguments);
    }

    public dynamic VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    public dynamic VisitIdentifierExpression(Identifier expression)
    {
        return _machine.Get(expression.Name.Lexeme);
    }

    public dynamic VisitLiteralExpression(Literal expression)
    {
        if (expression.Value is not int) return expression.Value;

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
            default:
                sb.Append(value.ToString());
                break;
        }

        if (value is (int or float))
            sb.Append(' ');

        _machine.CursorX += sb.Length;
        return sb.ToString();
    }

    private string StringifyFloat(dynamic value)
    {
        if (value == 0)
            return "0";
        if (value < .1 && value > -.1)
            return value.ToString("0.#####E+00");
        if (value < 1 && value > -1)
            return value.ToString("0.######");
        if (value > 999999 || value < -999999)
            return value.ToString("0.#####E+00");
        if (value > -10 && value < 10)
            return value.ToString("#.#####");
        if (value > -100 && value < 100)
            return value.ToString("##.####");
        if (value > -1000 && value < 1000)
            return value.ToString("###.###");
        if (value > -10000 && value < 10000)
            return value.ToString("####.##");
        if (value > -100000 && value < 100000)
            return value.ToString("#####.#");

        return value.ToString("######");
    }

    public void Execute(IStatement statement)
    {
        _program.CurrentStatement = statement;
        try
        {
            statement.Accept(this);
        }
        catch (ReturnFromGosub)
        {
            throw;
        }
        catch (ScanException)
        {
            throw;
        }
        catch (ParseException)
        {
            throw;
        }
        catch (RuntimeStatementException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine, ex.Message, ex);
        }
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
        _trs80.WriteLine();
        _trs80.WriteLine("READY");
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
        dynamic startValue = Evaluate(statement.StartValue);
        Assign(statement.Identifier, startValue);

        dynamic endValue = Evaluate(statement.EndValue);
        dynamic stepValue = Evaluate(statement.StepValue);

        _machine.ForConditions.Push(new ForCondition
        {
            Identifier = statement.Identifier,
            Start = (int)startValue,
            End = (int)endValue,
            Step = (int)stepValue,
            Goto = _machine.GetNextStatement(statement)
        });

        return null!;
    }

    public Void VisitGosubStatement(Gosub statement)
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

    public Void VisitGotoStatement(Goto statement)
    {
        IStatement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOTO");
        _machine.SetNextStatement(jumpToStatement);
        return null!;
    }

    public Void VisitIfStatement(If statement)
    {
        if (!IsTruthy(Evaluate(statement.Condition))) return null!;

        _machine.RunCompoundStatement(statement.ThenBranch, this);

        return null!;
    }

    public Void VisitInputStatement(Input statement)
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
            case Array array:
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
        {
            try
            {
                Assign(variable, value);
            }
            catch (ValueOutOfRangeException)
            {
                _trs80.WriteLine("WHAT?");
                GetInputValue(variable, writeNewline);
            }
        }
    }

    public Void VisitLetStatement(Let statement)
    {
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
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForLoad();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.LoadProgram(path);
        _trs80.WriteLine($"Loaded \"{path}\".");

        return null!;
    }

    public Void VisitMergeStatement(Merge statement)
    {
        string path = Evaluate(statement.Path);
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
        ForCondition condition = PopCondition(statement);
        dynamic newValue = IncrementIndexer(condition.Identifier, condition.Step);
        if (ConditionMet(newValue, condition.Step, condition.End)) return null!;

        _machine.ForConditions.Push(condition);
        _machine.SetNextStatement(condition.Goto);

        return null!;
    }

    private ForCondition PopCondition(Next next)
    {
        ForCondition checkCondition = null;

        if (next.Variable != null)
        {
            Identifier checkIdentifier;
            var nextIdentifier = next.Variable as Identifier;
            do
            {
                if (_machine.ForConditions.Count == 0)
                    throw new ParseException(next.LineNumber, next.SourceLine,
                        "'NEXT' variable mismatch with 'FOR'");

                checkCondition = _machine.ForConditions.Pop();
                if (checkCondition.Identifier is Identifier variable)
                    checkIdentifier = variable;
                else
                    throw new ParseException(next.LineNumber, next.SourceLine,
                        "Expected variable name after 'FOR'.");
            } while (checkIdentifier.Name.Lexeme != nextIdentifier?.Name.Lexeme);
        }
        else
        {
            if (next.Variable is not Identifier)
                throw new ParseException(next.LineNumber, next.SourceLine,
                    "Expected variable name after 'NEXT'.");
        }
        return checkCondition;
    }

    private dynamic IncrementIndexer(Expression identifier, int step)
    {
        dynamic currentValue = Evaluate(identifier);
        dynamic newValue = currentValue + step;
        Assign(identifier, newValue);
        return newValue;
    }

    private static bool ConditionMet(dynamic nextValue, int step, int endValue)
    {
        if (step > 0)
        {
            if (nextValue > endValue) return true;
        }
        else if (nextValue < endValue) return true;

        return false;
    }

    public Void VisitOnStatement(On statement)
    {
        int selector = (int)Math.Floor((float)Evaluate(statement.Selector)) - 1;
        var locations = statement.Locations.Select(location => Evaluate(location)).ToList();

        if (selector >= locations.Count || selector < 0) return null!;

        if (statement.IsGosub)
        {
            IStatement resumeStatement = _machine.GetNextStatement(statement);
            Expression location = new Literal(locations[selector]);
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

    public Void VisitPrintStatement(Print statement)
    {
        if (statement.AtPosition != null) PrintAt(statement.AtPosition);

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
            Assign(variable, _machine.Data.GetNext());

        return null!;
    }

    public Void VisitRemStatement(Rem statement)
    {
        // do nothing
        return null!;
    }

    public Void VisitReplaceStatement(Replace statement)
    {
        if (string.IsNullOrEmpty(statement.Statement.SourceLine))
            DeleteStatement(statement.Statement.LineNumber);
        else
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

    public Void VisitSaveStatement(Save statement)
    {
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForSave();

        if (string.IsNullOrEmpty(path)) return null!;

        _machine.SaveProgram(path);
        _trs80.WriteLine($"Saved \"{path}\".");

        return null!;
    }

    public Void VisitStatementExpressionStatement(StatementExpression statement)
    {
        Evaluate(statement.Expression);

        return null!;
    }

    public Void VisitStopStatement(Stop statement)
    {
        _trs80.WriteLine($"BREAK AT {statement.LineNumber}");
        _machine.HaltRun();

        return null!;
    }
}