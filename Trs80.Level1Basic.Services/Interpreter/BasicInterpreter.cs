using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Trs80.Level1Basic.Domain;
using Trs80.Level1Basic.Exceptions;
using Trs80.Level1Basic.Graphics;
using Trs80.Level1Basic.Services.Parser;
using Trs80.Level1Basic.Services.Parser.Expressions;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter;

public class BasicInterpreter : IBasicInterpreter
{
    private readonly ITrs80Console _console;
    private readonly IBasicEnvironment _environment;
    private readonly IScreen _screen;

    public BasicFunctionImplementations Functions { get; } = new();

    public BasicInterpreter(ITrs80Console console, IBasicEnvironment environment, IScreen screen)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _screen = screen ?? throw new ArgumentNullException(nameof(screen));
    }

    public void Interpret(Line line)
    {
        if (line.LineNumber > 0)
            Execute(new Replace(line));
        else
            foreach (var statement in line.Statements)
                Execute(statement);
    }
    
    public void Execute(Statement statement)
    {
        try
        {
            statement.Accept(this);
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
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine, ex.Message);
        }
    }

    public dynamic VisitBasicArrayExpression(BasicArray root)
    {
        dynamic index = Evaluate(root.Index);
        return _environment.GetArrayValue(root.Name.Lexeme, index);
    }

    public dynamic VisitAssignExpression(Assign assign)
    {
        dynamic value = Evaluate(assign.Value);
        return AssignVariable(assign, value);
    }

    public dynamic VisitBinaryExpression(Binary binary)
    {
        dynamic left = Evaluate(binary.Left);
        dynamic right = Evaluate(binary.Right);

        CheckForProperOperands(binary.OperatorType, left, right);

        return binary.OperatorType.Type switch
        {
            TokenType.Plus => (left is bool && right is bool) ? left || right : left + right,
            TokenType.Minus => left - right,
            TokenType.Slash => right == 0 ? throw new ValueOutOfRangeException(0, "Divide by zero") : (float)left / right,
            TokenType.Star => (left is bool && right is bool) ? left && right : left * right,
            TokenType.GreaterThan => left > right,
            TokenType.GreaterThanOrEqual => left >= right,
            TokenType.LessThan => left < right,
            TokenType.LessThanOrEqual => left <= right,
            TokenType.NotEqual => !AreEqual(left, right),
            TokenType.Equal => AreEqual(left, right),
            _ => null
        };
    }

    public dynamic VisitCallExpression(Call call)
    {
        List<dynamic> arguments = call.Arguments.Select(argument => Evaluate(argument)).ToList();

        var function = _environment.GetFunctionDefinition(call.Name.Lexeme);

        return function.Call(this, arguments);
    }

    private void CheckForProperOperands(Token operatorType, dynamic left, dynamic right)
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
    private void CheckForNumericOperand(Token operatorType, dynamic operand)
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


    private bool AreEqual(dynamic left, dynamic right)
    {
        if (left == null && right == null) return true;
        return left != null && (bool)left.Equals(right);
    }

    public dynamic VisitGroupingExpression(Grouping root)
    {
        return Evaluate(root.Expression);
    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public dynamic VisitLiteralExpression(Literal literal)
    {
        if (literal.Value is not int) return literal.Value;

        if (literal.Value > short.MaxValue || literal.Value < short.MinValue)
            // ReSharper disable once PossibleInvalidCastException
            return (float)literal.Value;

        return literal.Value;
    }

    //public dynamic VisitStopExpression(Stop stop)
    //{
    //    return null;
    //}

    public dynamic VisitUnaryExpression(Unary unary)
    {
        dynamic right = Evaluate(unary.Right);

        CheckForNumericOperand(unary.OperatorType, right);
        return -1 * right;
    }

    public dynamic VisitIdentifierExpression(Identifier identifier)
    {
        return _environment.GetVariable(identifier.Name.Lexeme);
    }

    private void WriteValue(dynamic value)
    {
        switch (value)
        {
            case null:
                return;
            case float:
                {
                    if (value == 0)
                        _sb.Append("0");
                    else if (value < .1 && value > -.1)
                        _sb.Append(value.ToString("0.#####E+00"));
                    else if (value < 1 && value > -1)
                        _sb.Append(value.ToString("0.######"));
                    else if (value > 999999 || value < -999999)
                        _sb.Append(value.ToString("0.#####E+00"));
                    else if (value > -10 && value < 10)
                        _sb.Append(value.ToString("#.#####"));
                    else if (value > -100 && value < 100)
                        _sb.Append(value.ToString("##.####"));
                    else if (value > -1000 && value < 1000)
                        _sb.Append(value.ToString("###.###"));
                    else if (value > -10000 && value < 10000)
                        _sb.Append(value.ToString("####.##"));
                    else if (value > -100000 && value < 100000)
                        _sb.Append(value.ToString("#####.#"));
                    else
                        _sb.Append(value.ToString("######"));
                    break;
                }
            default:
                _sb.Append(value.ToString());
                break;
        }
    }

    public void VisitNextStatement(Next root)
    {
        var checkCondition = GetCheckCondition(root);
        dynamic nextIndexerValue = IncrementIndexer(checkCondition);
        if (EndOfLoop(checkCondition, nextIndexerValue)) return;
        Loop(checkCondition);
    }

    private void Loop(ForCheckCondition checkCondition)
    {
        _environment.ForChecks.Push(checkCondition);
        _environment.SetNextStatement(checkCondition.Next);
    }

    private bool EndOfLoop(ForCheckCondition checkCondition, dynamic nextIndexerValue)
    {
        if (checkCondition.Step > 0)
        {
            if (nextIndexerValue > checkCondition.End) return true;
        }
        else if (nextIndexerValue < checkCondition.End) return true;

        return false;
    }

    private dynamic IncrementIndexer(ForCheckCondition checkCondition)
    {
        dynamic indexerValue = GetVariable(checkCondition.Variable);
        dynamic nextIndexerValue = indexerValue + checkCondition.Step;
        AssignVariable(checkCondition.Variable, nextIndexerValue);
        return nextIndexerValue;
    }

    private dynamic AssignVariable(Expression expression, dynamic value)
    {
        switch (expression)
        {
            case Identifier identifier:
                _environment.AssignVariable(identifier.Name.Lexeme, value);
                break;
            case BasicArray array:
                {
                    dynamic index = Evaluate(array.Index);
                    _environment.AssignArray(array.Name.Lexeme, index, value);
                    break;
                }
            default:
                throw new RuntimeExpressionException(null, "Expected variable.");
        }

        return value;
    }

    private dynamic GetVariable(Expression expression)
    {
        return Evaluate(expression);
    }

    private ForCheckCondition GetCheckCondition(Next next)
    {
        ForCheckCondition checkCondition;

        if (next.Variable != null)
            checkCondition = _environment.ForChecks.Pop();
        else
        {
            if (next.Variable is not Identifier nextIdentifier)
                throw new ParseException(next.LineNumber, next.SourceLine,
                    "Expected variable name after 'NEXT'.");
            Identifier checkIdentifier;

            do
            {
                checkCondition = _environment.ForChecks.Pop();
                if (checkCondition.Variable is Identifier variable)
                    checkIdentifier = variable;
                else
                    throw new ParseException(next.LineNumber, next.SourceLine,
                        "Expected variable name after 'FOR'.");
            } while (checkIdentifier.Name.Lexeme != nextIdentifier.Name.Lexeme);
        }
        return checkCondition;
    }

    public void VisitOnStatement(On on)
    {
        int selector = (int)Math.Floor((float)Evaluate(on.Selector)) - 1;
        List<dynamic> locations = on.Locations.Select(location => Evaluate(location)).ToList();

        if (selector >= locations.Count || selector < 0) return;

        if (on.IsGosub)
            _environment.ProgramStack.Push(on.Next);

        _environment.SetNextStatement(GetStatementByLineNumber(locations[selector]));
    }

    private int _printPosition;
    public void VisitPrintStatement(Print printStatement)
    {
        _sb = new StringBuilder();
        if (printStatement.AtPosition != null) PrintAt(printStatement.AtPosition);

        if (printStatement.Expressions is { Count: > 0 })
            for (int i = 0; i < printStatement.Expressions.Count; i++)
                WriteExpression(printStatement.Expressions[i], i == 0);

        //WriteExpression(
        //    printStatement.Expressions[printStatement.Expressions.Count - 1],
        //    printStatement.Expressions.Count == 1);

        string text = _sb.ToString();
        _printPosition += text.Length;

        _console.Write(text);
        if (!printStatement.WriteNewline) return;

        _console.WriteLine();
        _printPosition = 0;
    }

    private void PrintAt(Expression position)
    {
        dynamic value = Evaluate(position);
        dynamic column = value / 64;
        dynamic row = value - column * 64;

        _console.SetCursorPosition(row, column);
    }

    public void VisitReplaceStatement(Replace root)
    {
        _environment.ReplaceProgramLine(root.Line);
    }

    public void VisitReadStatement(Read root)
    {
        foreach (var variable in root.Variables)
            AssignVariable(variable, _environment.Data.GetNext());
    }

    private StringBuilder _sb = new();
    public void WriteToPosition(int position)
    {
        string currentText = _sb.ToString().TrimEnd();
        if (currentText.Length + _printPosition > position) return;

        string padding = "".PadRight(position - (currentText.Length + _printPosition), ' ');

        _sb.Clear();
        _sb.Append(currentText);
        _sb.Append(padding);
    }

    public string PadQuadrant()
    {
        int currentPosition = _printPosition + _sb.Length;

        int nextPosition = (currentPosition / 15 + 1) * 15 + 1;
        string padding = "".PadRight(nextPosition - currentPosition, ' ');

        return padding;
    }

    public void Set(int x, int y)
    {
        _screen.Set(x, y);
    }

    public void Reset(int x, int y)
    {
        _screen.Reset(x, y);
    }

    public bool Point(int x, int y)
    {
        return _screen.Point(x, y);
    }

    public int MemoryInUse()
    {
        return _environment.MemoryInUse();
    }

    private void PrependSpaceIfNecessary(dynamic value, bool first)
    {
        bool isNonNegativeNumber = (value is int || value is float) && value >= 0;

        if (first && !isNonNegativeNumber) return;

        if (value is string str && str.StartsWith(" ")) return;

        string text = _sb.ToString();
        if (text.EndsWith(" ") && !isNonNegativeNumber) return;

        _sb.Append(" ");
    }
    private void WriteExpression(Expression expression, bool first)
    {
        dynamic value = Evaluate(expression);
        PrependSpaceIfNecessary(value, first);
        WriteValue(value);
    }

    public void VisitStatementExpressionStatement(StatementExpression statement)
    {
        Evaluate(statement.Expression);
    }

    public void VisitStopStatement(Stop root)
    {
        _console.WriteLine($"BREAK AT {root.LineNumber}");
        _environment.HaltRun();
    }

    public void VisitRestoreStatement(Restore _)
    {
        _environment.Data.MoveFirst();
    }

    public void VisitReturnStatement(Return returnStatement)
    {
        _environment.SetNextStatement(_environment.ProgramStack.Pop());
    }

    public void VisitRunStatement(Run runStatement)
    {
        _environment.GetProgramStatements();

        int lineNumber = GetStartingLineNumber(runStatement.StartAtLineNumber);

        _environment.LoadData(this);
        RunProgram(GetStatementByLineNumber(lineNumber), true);
    }

    private int GetStartingLineNumber(Expression startAtLineNumber)
    {
        int lineNumber = 0;
        dynamic value = Evaluate(startAtLineNumber);
        if (value != null)
            lineNumber = (int)value;
        return lineNumber;
    }

    public void RunProgram(Statement statement, bool initialize)
    {
        if (initialize)
            _environment.Initialize();

        _environment.RunProgram(statement, this);
        _console.WriteLine();
        _console.WriteLine("READY");
    }

    public void VisitListStatement(List listStatement)
    {
        int lineNumber = GetStartingLineNumber(listStatement.StartAtLineNumber);
        _environment.ListProgram(lineNumber);
    }

    public void VisitRemStatement(Rem remStatement)
    {
        // do nothing
    }

    public void VisitLoadStatement(Load loadStatement)
    {
        _environment.NewProgram();
        _environment.LoadProgram(Evaluate(loadStatement.Path));
    }

    public void VisitSaveStatement(Save saveStatement)
    {
        _environment.SaveProgram(Evaluate(saveStatement.Path));
    }
    public void VisitEndStatement(End endStatement)
    {
        _environment.HaltRun();
    }

    public void VisitForStatement(For forStatement)
    {
        dynamic startValue = Evaluate(forStatement.StartValue);
        AssignVariable(forStatement.Variable, startValue);

        dynamic endValue = Evaluate(forStatement.EndValue);
        dynamic stepValue = Evaluate(forStatement.StepValue);

        _environment.ForChecks.Push(new ForCheckCondition
        {
            Variable = forStatement.Variable,
            Start = (int)startValue,
            End = (int)endValue,
            Step = (int)stepValue,
            Next = forStatement.Next
        });
    }

    public void VisitGotoStatement(Goto gotoStatement)
    {
        dynamic nextLineNumber = Evaluate(gotoStatement.Location);
        _environment.SetNextStatement(GetStatementByLineNumber(nextLineNumber));
    }

    public void VisitGosubStatement(Gosub gosub)
    {
        _environment.ProgramStack.Push(gosub.Next);
        dynamic nextLineNumber = Evaluate(gosub.Location);
        _environment.SetNextStatement(GetStatementByLineNumber(nextLineNumber));
    }

    public void VisitIfStatement(If ifStatement)
    {
        dynamic value = Evaluate(ifStatement.Condition);

        if (!value) return;

        foreach (var statement in ifStatement.ThenStatements.Where(_ => !_environment.Halted))
            Execute(statement);

        //switch (ifStatement.ThenExpression)
        //{
        //    case Stop _:
        //        _console.WriteLine($"BREAK AT {ifStatement.LineNumber}");
        //        _environment.HaltRun();
        //        break;
        //    case Literal _:
        //        {
        //            var lineNumber = Evaluate(ifStatement.ThenExpression);
        //            _environment.SetNextStatement(GetStatementByLineNumber(lineNumber));
        //            break;
        //        }
        //    case Binary assignment:
        //        {
        //            if (assignment.OperatorType.Type != TokenType.Equal)
        //                throw new RuntimeStatementException(ifStatement.LineNumber, "Expected assignment after 'THEN'");

        //            var newValue = Evaluate(assignment.Right);
        //            AssignVariable(assignment.Left, newValue);
        //            break;
        //        }
        //    //case Return _:
        //    //{
        //    //    var statement = _environment.ProgramStack.Pop();
        //    //    _environment.SetNextStatement(statement);
        //    //    break;
        //    //}
        //    default:
        //        throw new RuntimeStatementException(ifStatement.LineNumber, "Unknown expression type after 'THEN'");
        //}
    }

    public void VisitInputStatement(Input inputStatement)
    {
        _sb = new StringBuilder();
        foreach (var expression in inputStatement.Expressions)
            ProcessInputExpression(expression, inputStatement.WriteNewline);

        //ProcessInputExpression(
        //    inputStatement.Expressions[inputStatement.Expressions.Count - 1],
        //    inputStatement.WriteNewline);
    }

    private void ProcessInputExpression(Expression expression, bool writeNewline)
    {
        switch (expression)
        {
            case Literal:
                WriteValue(Evaluate(expression));
                break;
            case Identifier variable:
                GetInputValue(variable, writeNewline);
                break;
            case BasicArray array:
                GetInputValue(array, writeNewline);
                break;
        }
    }

    private void GetInputValue(Expression variable, bool writeNewline)
    {
        _console.Write(_sb.ToString());
        _console.Write("?");
        if (writeNewline)
            _console.WriteLine();

        string value = _console.ReadLine();
        if (int.TryParse(value, out int intValue))
            AssignVariable(variable, intValue);
        else if (float.TryParse(value, out float floatValue))
            AssignVariable(variable, floatValue);
        else if (_environment.VariableExists(value))
        {
            dynamic lookup = _environment.GetVariable(value);
            AssignVariable(variable, lookup);

        }
        else
        {
            try
            {
                AssignVariable(variable, value);
            }
            catch (ValueOutOfRangeException)
            {
                _console.WriteLine("WHAT?");
                GetInputValue(variable, writeNewline);
            }
        }

    }

    private Statement GetStatementByLineNumber(int lineNumber)
    {
        return _environment.GetStatementByLineNumber(lineNumber);
    }

    public void VisitLetStatement(Let root)
    {
        dynamic value = null;
        if (root.Initializer != null)
            value = Evaluate(root.Initializer);

        AssignVariable(root.Variable, value);
    }

    public void VisitNewStatement(New root)
    {
        NewProgram();
    }

    public void VisitClsStatement(Cls root)
    {
        _console.Clear();
        _screen.Clear();
        Thread.Sleep(500);
    }

    public void VisitContStatement(Cont root)
    {
        RunProgram(_environment.GetNextStatement(), false);
    }

    public void VisitDataStatement(Data data)
    {
        foreach (var element in data.DataElements)
            _environment.Data.Add(Evaluate(element));
    }

    public void VisitDeleteStatement(Delete root)
    {
        var programLine = _environment.ProgramLines.FirstOrDefault(l => l.LineNumber == root.LineToDelete);
        if (programLine != null)
            _environment.ProgramLines.Remove(programLine);
    }

    private void NewProgram()
    {
        _environment.ProgramLines = new List<Line>();
    }
}