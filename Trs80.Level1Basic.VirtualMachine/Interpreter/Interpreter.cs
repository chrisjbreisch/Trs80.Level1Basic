using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Interpreter : IInterpreter
{
    private readonly IConsole _console;
    private readonly IMachine _machine;
    public int CursorX { get; private set; }
    public int CursorY { get; private set; }

    public BasicFunctionImplementations Functions { get; } = new();

    public Interpreter(IConsole console, IMachine machine)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _machine = machine ?? throw new ArgumentNullException(nameof(machine));
    }

    public Statement CurrentStatement { get; private set; }

    public void Interpret(ParsedLine line)
    {
        if (line.LineNumber >= 0)
            Execute(new Replace(line));
        else
            foreach (Statement statement in line.Statements)
                Execute(statement);
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

    public dynamic VisitBasicArrayExpression(BasicArray expression)
    {
        dynamic index = Evaluate(expression.Index);
        return _machine.GetArrayValue(expression.Name.Lexeme, index);
    }

    public dynamic VisitAssignExpression(Assign expression)
    {
        dynamic value = Evaluate(expression.Value);
        return AssignVariable(expression, value);
    }

    public dynamic VisitBinaryExpression(Binary expression)
    {
        dynamic left = Evaluate(expression.Left);
        dynamic right = Evaluate(expression.Right);

        CheckForProperOperands(expression.OperatorType, left, right);

        return expression.OperatorType.Type switch
        {
            TokenType.Plus => (left is bool && right is bool) ? left || right : left + right,
            TokenType.Minus => left - right,
            TokenType.Slash => right == 0 ? throw new ValueOutOfRangeException(0, "", "Divide by zero") : (float)left / right,
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

    public dynamic VisitCallExpression(Call expression)
    {
        var arguments = expression.Arguments.Select(argument => Evaluate(argument)).ToList();

        FunctionDefinition function = _machine
            .GetFunctionDefinition(expression.Name.Lexeme).First(f => f.Arity == arguments.Count);

        return function.Call(this, arguments);
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


    private static bool AreEqual(dynamic left, dynamic right)
    {
        if (left == null && right == null) return true;
        return left != null && (bool)left.Equals(right);
    }

    public dynamic VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
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

        CheckForNumericOperand(expression.OperatorType, right);
        return -1 * right;
    }

    public dynamic VisitIdentifierExpression(Identifier expression)
    {
        return _machine.GetVariable(expression.Name.Lexeme);
    }

    private void WriteExpression(Expression expression)
    {
        dynamic value = Evaluate(expression);
        int startingLength = _sb.Length;
        
        if (value is >= 0 or float and >= 0)
            _sb.Append(' ');
        
        switch (value)
        {
            case null:
                return;
            case float:
                WriteFloatValue(value);
                break;
            default:
                _sb.Append(value.ToString());
                break;
        }

        if (value is (int or float))
            _sb.Append(' ');

        CursorX += _sb.Length - startingLength;
    }

    private void WriteFloatValue(dynamic value)
    {
        if (value == 0)
            _sb.Append('0');
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
    }

    public sd VisitNextStatement(Next statement)
    {
        ForCheckCondition checkCondition = GetCheckCondition(statement);
        dynamic nextIndexerValue = IncrementIndexer(checkCondition);
        if (EndOfLoop(checkCondition, nextIndexerValue)) return;
        Loop(checkCondition);
    }

    private void Loop(ForCheckCondition checkCondition)
    {
        _machine.ForChecks.Push(checkCondition);
        _machine.SetNextStatement(checkCondition.Next);
    }

    private static bool EndOfLoop(ForCheckCondition checkCondition, dynamic nextIndexerValue)
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
                _machine.AssignVariable(identifier.Name.Lexeme, value);
                break;
            case BasicArray array:
                {
                    dynamic index = Evaluate(array.Index);
                    _machine.AssignArray(array.Name.Lexeme, index, value);
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
        ForCheckCondition checkCondition = null;

        if (next.Variable != null)
        {
            Identifier checkIdentifier;
            var nextIdentifier = next.Variable as Identifier;
            do
            {
                if (_machine.ForChecks.Count == 0)
                    throw new ParseException(next.LineNumber, next.SourceLine,
                        "'NEXT' variable mismatch with 'FOR'");

                checkCondition = _machine.ForChecks.Pop();
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

    public void VisitOnStatement(On statement)
    {
        int selector = (int)Math.Floor((float)Evaluate(statement.Selector)) - 1;
        var locations = statement.Locations.Select(location => Evaluate(location)).ToList();

        if (selector >= locations.Count || selector < 0) return;

        if (statement.IsGosub)
        {
            _machine.ProgramStack.Push(statement.Next);
            Expression location = new Literal(locations[selector]);
            Statement jumpToStatement = GetJumpToStatement(statement, location, "GOSUB");
            _machine.RunProgram(jumpToStatement, this);

            _machine.SetNextStatement(_machine.ProgramStack.Pop());
            return;
        }

        Statement nextStatement = GetStatementByLineNumber(locations[selector]);
        
        if (nextStatement is null)
            throw new RuntimeStatementException(statement.LineNumber, statement.SourceLine,
                $"Can't 'GOTO' line {locations[selector]}.");

        _machine.SetNextStatement(nextStatement);
    }

    public void VisitPrintStatement(Print statement)
    {
        _sb = new StringBuilder();
        if (statement.AtPosition != null) PrintAt(statement.AtPosition);

        if (statement.Expressions is { Count: > 0 })
            foreach (Expression expression in statement.Expressions)
                WriteExpression(expression);

        string text = _sb.ToString();
        _console.Write(text);

        if (!statement.WriteNewline) return;

        _console.WriteLine();
        CursorX = 0;
        CursorY++;
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

    public void VisitReplaceStatement(Replace statement)
    {
        if (string.IsNullOrEmpty(statement.Line.SourceLine))
            DeleteStatement(statement.Line.LineNumber);
        else
            _machine.Program.ReplaceLine(statement.Line);
    }

    public void VisitReadStatement(Read statement)
    {
        foreach (Expression variable in statement.Variables)
            AssignVariable(variable, _machine.Data.GetNext());
    }

    private StringBuilder _sb = new();
    public void WriteToPosition(int position)
    {
        if (CursorX > position) return;

        string padding = "".PadRight(position - CursorX, ' ');

        _sb.Append(padding);
        CursorX = position;
    }

    public string PadQuadrant()
    {
        int nextPosition = (CursorX / 16 + 1) * 16;
        string padding = "".PadRight(nextPosition - CursorX, ' ');
        return padding;
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
        return _machine.MemoryInUse();
    }
    
    public void VisitStatementExpressionStatement(StatementExpression statement)
    {
        Evaluate(statement.Expression);
    }

    public void VisitStopStatement(Stop statement)
    {
        _console.WriteLine($"BREAK AT {statement.LineNumber}");
        _machine.HaltRun();
    }

    public void VisitRestoreStatement(Restore _)
    {
        _machine.Data.MoveFirst();
    }

    public void VisitReturnStatement(Return statement)
    {
        _machine.SetNextStatement(null);
    }

    public void VisitRunStatement(Run statement)
    {
        _machine.InitializeProgram();
        GetCursorPosition();

        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        if (lineNumber < 0)
            lineNumber = GetFirstLineNumber();
        if (lineNumber < 0) return;

        _machine.LoadData(this);
        Statement firstStatement = GetStatementByLineNumber(lineNumber);
        if (firstStatement is null)
            throw new RuntimeStatementException(-1, statement.SourceLine, $"Can't start execution at {lineNumber}");

        RunProgram(firstStatement, true);
    }

    private void GetCursorPosition()
    {
        (int left, int top) = _console.GetCursorPosition();
        CursorX = left;
        CursorY = top;
    }

    private int GetFirstLineNumber()
    {
        Statement statement = _machine.Program.GetFirstStatement();

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
            _machine.Initialize();

        _machine.RunProgram(statement, this);
        _console.WriteLine();
        _console.WriteLine("READY");
    }

    public void VisitListStatement(List statement)
    {
        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        _machine.ListProgram(lineNumber);
    }

    public void VisitRemStatement(Rem statement)
    {
        // do nothing
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


    public void VisitLoadStatement(Load statement)
    {
        _machine.NewProgram();
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = OpenFileDialog();

        if (string.IsNullOrEmpty(path)) return;

        _machine.LoadProgram(path);
        _console.WriteLine($"Loaded \"{path}\".");
    }

    public void VisitMergeStatement(Merge statement)
    {
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = OpenFileDialog();

        if (string.IsNullOrEmpty(path)) return;

        _machine.LoadProgram(path);
        _console.WriteLine($"Merged \"{path}\".");
    }

    public void VisitSaveStatement(Save statement)
    {
        string path = Evaluate(statement.Path);
        if (string.IsNullOrEmpty(path))
            path = SaveFileDialog();

        if (string.IsNullOrEmpty(path)) return;

        _machine.SaveProgram(path);
        _console.WriteLine($"Saved \"{path}\".");
    }
    public void VisitEndStatement(End statement)
    {
        _machine.HaltRun();
    }

    public void VisitForStatement(For statement)
    {
        dynamic startValue = Evaluate(statement.StartValue);
        AssignVariable(statement.Variable, startValue);

        dynamic endValue = Evaluate(statement.EndValue);
        dynamic stepValue = Evaluate(statement.StepValue);

        _machine.ForChecks.Push(new ForCheckCondition
        {
            Variable = statement.Variable,
            Start = (int)startValue,
            End = (int)endValue,
            Step = (int)stepValue,
            Next = statement.Next
        });
    }

    public void VisitGotoStatement(Goto statement)
    {
        Statement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOTO");
        _machine.SetNextStatement(jumpToStatement);
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

    public void VisitGosubStatement(Gosub statement)
    {
        if (statement.Next != null)
            _machine.ProgramStack.Push(statement.Next);
        else
            _machine.ProgramStack.Push(CurrentStatement.Next);

        Statement jumpToStatement = GetJumpToStatement(statement, statement.Location, "GOSUB");
        _machine.RunProgram(jumpToStatement, this);

        _machine.SetNextStatement(_machine.ProgramStack.Pop());
    }

    public void VisitIfStatement(If statement)
    {
        dynamic value = Evaluate(statement.Condition);
        dynamic check;

        if (value is int intVal)
            check = intVal == 1;
        else
            check = value;

        if (!check) return;

        switch (statement.ThenStatement)
        {
            case Goto gotoStatement:
                VisitGotoStatement(gotoStatement);
                break;
            case Gosub gosubStatement:
                VisitGosubStatement(gosubStatement);
                break;
            default:
                VisitThenStatement(statement);
                break;
        }
    }

    private void VisitThenStatement(If ifStatement)
    {
        Statement nextStatement = ifStatement.ThenStatement;
        while (nextStatement != null)
        {
            Execute(nextStatement);
            if (nextStatement is Goto) break;
            nextStatement = nextStatement.Next;
        }
    }

    public void VisitInputStatement(Input statement)
    {
        _sb = new StringBuilder();
        foreach (Expression expression in statement.Expressions)
            ProcessInputExpression(expression, statement.WriteNewline);
    }

    private void ProcessInputExpression(Expression expression, bool writeNewline)
    {
        switch (expression)
        {
            case Literal:
                WriteExpression(expression);
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
        if (value is null)
            AssignVariable(variable, null);
        else if (int.TryParse(value, out int intValue))
            AssignVariable(variable, intValue);
        else if (float.TryParse(value, out float floatValue))
            AssignVariable(variable, floatValue);
        else if (_machine.VariableExists(value))
        {
            dynamic lookup = _machine.GetVariable(value);
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

        GetCursorPosition();
    }

    private Statement GetStatementByLineNumber(int lineNumber)
    {
        return _machine.GetStatementByLineNumber(lineNumber);
    }

    public void VisitLetStatement(Let statement)
    {
        dynamic value = null;
        if (statement.Initializer != null)
            value = Evaluate(statement.Initializer);

        AssignVariable(statement.Variable, value);
    }

    public void VisitNewStatement(New statement)
    {
        NewProgram();
    }

    public void VisitClsStatement(Cls statement)
    {
        _console.Clear();
        Thread.Sleep(500);
    }

    public void VisitContStatement(Cont statement)
    {
        RunProgram(_machine.GetNextStatement(), false);
    }

    public void VisitDataStatement(Data statement)
    {
        foreach (Expression element in statement.DataElements)
            _machine.Data.Add(Evaluate(element));
    }

    private void DeleteStatement(int lineNumber)
    {
        ParsedLine programLine = _machine.Program.List().FirstOrDefault(l => l.LineNumber == lineNumber);
        if (programLine != null)
            _machine.Program.RemoveLine(programLine);
    }
    public void VisitDeleteStatement(Delete statement)
    {
        DeleteStatement(statement.LineToDelete);
    }

    private void NewProgram()
    {
        _machine.Program.Clear();
    }
}