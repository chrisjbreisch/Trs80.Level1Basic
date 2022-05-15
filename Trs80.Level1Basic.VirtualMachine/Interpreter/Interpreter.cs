using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.VirtualMachine.Environment;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

using Array = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Array;
using Expression = Trs80.Level1Basic.VirtualMachine.Parser.Expressions.Expression;
using Void = Trs80.Level1Basic.Common.Void;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Interpreter : IInterpreter
{
    private readonly IConsole _console;
    private readonly IEnvironment _environment;
    public int CursorX { get; private set; }
    public int CursorY { get; private set; }
    public Statement CurrentStatement { get; private set; }

    public FunctionImplementations Functions { get; } = new();

    public Interpreter(IConsole console, IEnvironment environment)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public void Interpret(ParsedLine line)
    {
        if (line.LineNumber >= 0)
            Execute(new Replace(line));
        else
            foreach (Statement statement in line.Statements)
                Execute(statement);
    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public dynamic VisitArrayExpression(Array expression)
    {
        dynamic index = Evaluate(expression.Index);
        return _environment.Get(expression.Name.Lexeme, index);
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
                _environment.Assign(identifier.Name.Lexeme, value);
                break;
            case Array array:
            {
                dynamic index = Evaluate(array.Index);
                _environment.Assign(array.Name.Lexeme, index, value);
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

        CheckForProperOperands(expression.BinaryOperator, left, right);

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

        FunctionDefinition function = _environment
            .GetFunctionDefinition(expression.Name.Lexeme).First(f => f.Arity == arguments.Count);

        return function.Call(this, arguments);
    }

    public dynamic VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    public dynamic VisitIdentifierExpression(Identifier expression)
    {
        return _environment.Get(expression.Name.Lexeme);
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

        CheckForNumericOperand(expression.UnaryOperator, right);
        return -1 * right;
    }

    private static void CheckForProperOperands(Token operatorType, dynamic left, dynamic right)
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

    private static void CheckForNumericOperand(Token operatorType, dynamic operand)
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

    private static bool IsTruthy(dynamic value)
    {
        if (value == null) return false;

        if (value is int intVal)
            return intVal == 1;
        else
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
                sb.Append(StringifyFloat(sb, value));
                break;
            default:
                sb.Append(value.ToString());
                break;
        }

        if (value is (int or float))
            sb.Append(' ');

        CursorX += sb.Length;
        return sb.ToString();
    }

    private string StringifyFloat(StringBuilder sb, dynamic value)
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
    
    public void Execute(Statement statement)
    {
        CurrentStatement = statement;
        try
        {
            statement.Accept(this);
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
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine, ex.Message);
        }
    }

    public Void VisitClsStatement(Cls statement)
    {
        _console.Clear();
        Thread.Sleep(500);

        return null!;
    }

    public Void VisitContStatement(Cont statement)
    {
        RunProgram(_environment.GetNextStatement(), false);

        return null!;
    }

    public Void VisitDataStatement(Data statement)
    {
        foreach (Expression element in statement.DataElements)
            _environment.Data.Add(Evaluate(element));

        return null!;
    }

    public Void VisitDeleteStatement(Delete statement)
    {
        DeleteStatement(statement.LineToDelete);

        return null!;
    }

    public Void VisitEndStatement(End statement)
    {
        _environment.HaltRun();

        return null!;
    }

    public Void VisitForStatement(For statement)
    {
        dynamic startValue = Evaluate(statement.StartValue);
        Assign(statement.Variable, startValue);

        dynamic endValue = Evaluate(statement.EndValue);
        dynamic stepValue = Evaluate(statement.StepValue);

        _environment.ForChecks.Push(new ForCheckCondition
        {
            Variable = statement.Variable,
            Start = (int)startValue,
            End = (int)endValue,
            Step = (int)stepValue,
            Next = statement.Next
        });

        return null!;
    }

    public Void VisitGosubStatement(Gosub statement)
    {
        _environment.ProgramStack.Push(statement.Next ?? CurrentStatement.Next);

        Statement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOSUB");
        _environment.RunProgram(jumpToStatement, this);

        _environment.SetNextStatement(_environment.ProgramStack.Pop());

        return null!;
    }

    public Void VisitGotoStatement(Goto statement)
    {
        Statement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOTO");
        _environment.SetNextStatement(jumpToStatement);

        return null!;
    }

    public Void VisitIfStatement(If statement)
    {
        if (!IsTruthy(Evaluate(statement.Condition))) return null!;

        switch (statement.ThenBranch)
        {
            case Goto gotoStatement:
                VisitGotoStatement(gotoStatement);
                break;
            case Gosub gosubStatement:
                VisitGosubStatement(gosubStatement);
                break;
            default:
                ExecuteThenBranch(statement.ThenBranch);
                break;
        }
        return null!;
    }

    public Void VisitInputStatement(Input statement)
    {
        var sb = new StringBuilder();
        foreach (Expression expression in statement.Expressions)
            ProcessInputExpression(expression, statement.WriteNewline);

        return null!;
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
        _environment.ListProgram(lineNumber);

        return null!;
    }

    public Void VisitLoadStatement(Load statement)
    {
        _environment.NewProgram();
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = OpenFileDialog();

        if (string.IsNullOrEmpty(path)) return null!;

        _environment.LoadProgram(path);
        _console.WriteLine($"Loaded \"{path}\".");

        return null!;
    }

    public Void VisitMergeStatement(Merge statement)
    {
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = OpenFileDialog();

        if (string.IsNullOrEmpty(path)) return null!;

        _environment.LoadProgram(path);
        _console.WriteLine($"Merged \"{path}\".");

        return null!;
    }

    public Void VisitNewStatement(New statement)
    {
        NewProgram();

        return null!;
    }

    public Void VisitNextStatement(Next statement)
    {
        ForCheckCondition checkCondition = GetCheckCondition(statement);
        dynamic nextIndexerValue = IncrementIndexer(checkCondition);
        if (ExitFor(checkCondition, nextIndexerValue)) return null!;

        _environment.ForChecks.Push(checkCondition);
        _environment.SetNextStatement(checkCondition.Next);

        return null!;
    }
    
    private ForCheckCondition GetCheckCondition(Next next)
    {
        ForCheckCondition checkCondition = null;

        if (next.Variable != null)
        {
            Identifier checkIdentifier;
            var nextIdentifier = next.Variable as Identifier;
            do
            {
                if (_environment.ForChecks.Count == 0)
                    throw new ParseException(next.LineNumber, next.SourceLine,
                        "'NEXT' variable mismatch with 'FOR'");

                checkCondition = _environment.ForChecks.Pop();
                if (checkCondition.Variable is Identifier variable)
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
    
    private dynamic IncrementIndexer(ForCheckCondition checkCondition)
    {
        dynamic indexerValue = Evaluate(checkCondition.Variable);
        dynamic nextIndexerValue = indexerValue + checkCondition.Step;
        Assign(checkCondition.Variable, nextIndexerValue);
        return nextIndexerValue;
    }
    
    private static bool ExitFor(ForCheckCondition checkCondition, dynamic nextIndexerValue)
    {
        if (checkCondition.Step > 0)
        {
            if (nextIndexerValue > checkCondition.End) return true;
        }
        else if (nextIndexerValue < checkCondition.End) return true;

        return false;
    }
    
    public Void VisitOnStatement(On statement)
    {
        int selector = (int)Math.Floor((float)Evaluate(statement.Selector)) - 1;
        var locations = statement.Locations.Select(location => Evaluate(location)).ToList();

        if (selector >= locations.Count || selector < 0) return null!;

        if (statement.IsGosub)
        {
            _environment.ProgramStack.Push(statement.Next);
            Expression location = new Literal(locations[selector]);
            Statement jumpToStatement = GetJumpToStatement(statement, location, "GOSUB");
            _environment.RunProgram(jumpToStatement, this);

            _environment.SetNextStatement(_environment.ProgramStack.Pop());
            return null!;
        }

        Statement nextStatement = GetStatementByLineNumber(locations[selector]);

        if (nextStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                $"Can't 'GOTO' line {locations[selector]}.");

        _environment.SetNextStatement(nextStatement);

        return null!;
    }

    public Void VisitPrintStatement(Print statement)
    {
        var sb = new StringBuilder();
        if (statement.AtPosition != null) PrintAt(statement.AtPosition);

        if (statement.Expressions is { Count: > 0 })
            foreach (Expression expression in statement.Expressions)
                _console.Write(Stringify(Evaluate(expression)));

        //string text = sb.ToString();
        //_console.Write(text);

        if (!statement.WriteNewline) return null!;

        _console.WriteLine();
        CursorX = 0;
        CursorY++;
        return null!;
    }

    private void PrintAt(Expression position)
    {
        dynamic value = Evaluate(position);
        dynamic row = value / 64;
        dynamic column = value % 64;

        _console.SetCursorPosition(column, row);

        CursorX = column;
        CursorY = row;
    }

    public string PadToPosition(int position)
    {
        if (CursorX > position) return "";

        string padding = "".PadRight(position - CursorX, ' ');

        return padding;
    }

    public string PadToQuadrant()
    {
        int nextPosition = (CursorX / 16 + 1) * 16;
        string padding = "".PadRight(nextPosition - CursorX, ' ');
        return padding;
    }

    public Void VisitReadStatement(Read statement)
    {
        foreach (Expression variable in statement.Variables)
            Assign(variable, _environment.Data.GetNext());

        return null!;
    }

    public Void VisitRemStatement(Rem statement)
    {
        // do nothing
        return null!;
    }

    public Void VisitReplaceStatement(Replace statement)
    {
        if (string.IsNullOrEmpty(statement.Line.SourceLine))
            DeleteStatement(statement.Line.LineNumber);
        else
            _environment.Program.ReplaceLine(statement.Line);

        return null!;
    }

    public Void VisitRestoreStatement(Restore _)
    {
        _environment.Data.MoveFirst();

        return null!;
    }

    public Void VisitReturnStatement(Return statement)
    {
        _environment.SetNextStatement(null);

        return null!;
    }

    public Void VisitRunStatement(Run statement)
    {
        _environment.InitializeProgram();
        GetCursorPosition();

        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        if (lineNumber < 0)
            lineNumber = GetFirstLineNumber();
        if (lineNumber < 0) return null!;

        _environment.LoadData(this);
        Statement firstStatement = GetStatementByLineNumber(lineNumber);
        if (firstStatement is null)
            throw new RuntimeStatementException(-1, statement.SourceLine, $"Can't start execution at {lineNumber}");

        RunProgram(firstStatement, true);

        return null!;
    }

    public Void VisitSaveStatement(Save statement)
    {
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = SaveFileDialog();

        if (string.IsNullOrEmpty(path)) return null!;

        _environment.SaveProgram(path);
        _console.WriteLine($"Saved \"{path}\".");

        return null!;
    }

    public Void VisitStatementExpressionStatement(StatementExpression statement)
    {
        Evaluate(statement.Value);

        return null!;
    }
    
    public Void VisitStopStatement(Stop statement)
    {
        _console.WriteLine($"BREAK AT {statement.LineNumber}");
        _environment.HaltRun();

        return null!;
    }

    public void Set(int x, int y)
    {
        _console.Set(x, y);
    }

    public void Reset(int x, int y)
    {
        _console.Reset(x, y);
    }

    public bool Point(int x, int y)
    {
        return _console.Point(x, y);
    }

    public int MemoryInUse()
    {
        return _environment.MemoryInUse();
    }

    private void GetCursorPosition()
    {
        (int left, int top) = _console.GetCursorPosition();
        CursorX = left;
        CursorY = top;
    }

    private int GetFirstLineNumber()
    {
        Statement statement = _environment.Program.GetFirstStatement();

        if (statement is null)
            return -1;

        return statement.LineNumber;
    }

    private int GetStartingLineNumber(Expression startAtLineNumber)
    {
        int lineNumber = -1;
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

    private const string Filter = "BASIC files (*.bas)|*.bas|All files (*.*)|*.*";
    private const string Title = "TRS-80 Level I BASIC File";

    private static string SaveFileDialog()
    {
        var dialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = "bas",
            Filter = Filter,
            Title = $"Save {Title}",
            OverwritePrompt = true
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }

    private static string OpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            DefaultExt = "bas",
            Filter = Filter,
            Title = $"Open {Title}",
            CheckFileExists = true,
            Multiselect = false,
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }

    private Statement GetJumpToStatement(Statement statement, Expression location, string jumpType)
    {
        dynamic jumpToLineNumber = Evaluate(location);

        Statement jumpToStatement = GetStatementByLineNumber(jumpToLineNumber);

        if (jumpToStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                $"Can't {jumpType} line {jumpToLineNumber}");

        return jumpToStatement;
    }

    private void ExecuteThenBranch(Statement thenBranch)
    {
        Statement nextStatement = thenBranch;
        while (nextStatement != null)
        {
            Execute(nextStatement);
            if (nextStatement is Goto) break;
            nextStatement = nextStatement.Next;
        }
    }

    private void ProcessInputExpression(Expression expression, bool writeNewline)
    {
        switch (expression)
        {
            case Literal:
                _console.Write(Stringify(Evaluate(expression)));
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
        //_console.Write(sb.ToString());
        _console.Write("?");

        if (writeNewline)
            _console.WriteLine();

        string value = _console.ReadLine();
        if (value is null)
            Assign(variable, null);
        else if (int.TryParse(value, out int intValue))
            Assign(variable, intValue);
        else if (float.TryParse(value, out float floatValue))
            Assign(variable, floatValue);
        else if (_environment.Exists(value))
        {
            dynamic lookup = _environment.Get(value);
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
                _console.WriteLine("WHAT?");
                GetInputValue(variable, writeNewline);
            }
        }

        GetCursorPosition();
    }

    private Statement GetStatementByLineNumber(int lineNumber)
    {
        return _environment.GetStatementByLineNumber(lineNumber);
    }
    private void NewProgram()
    {
        _environment.Program.Clear();
    }

    private void DeleteStatement(int lineNumber)
    {
        ParsedLine programLine = _environment.Program.List().FirstOrDefault(l => l.LineNumber == lineNumber);
        if (programLine != null)
            _environment.Program.RemoveLine(programLine);
    }

}