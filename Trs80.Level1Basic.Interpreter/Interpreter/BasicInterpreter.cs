using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.Interpreter.Exceptions;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Parser.Expressions;
using Trs80.Level1Basic.Interpreter.Parser.Statements;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Interpreter;

public class BasicInterpreter : IBasicInterpreter
{
    private readonly IConsole _console;
    private readonly IBasicEnvironment _environment;
    public int CursorX { get; private set; }
    public int CursorY { get; private set; }

    public BasicFunctionImplementations Functions { get; } = new();

    public BasicInterpreter(IConsole console, IBasicEnvironment environment)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
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

    public dynamic VisitCallExpression(Call call)
    {
        var arguments = call.Arguments.Select(argument => Evaluate(argument)).ToList();

        FunctionDefinition function = _environment
            .GetFunctionDefinition(call.Name.Lexeme).First(f => f.Arity == arguments.Count);

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

        // if (_sb.Length > 0 && _sb[^1] != ' ')
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

    public void VisitNextStatement(Next root)
    {
        ForCheckCondition checkCondition = GetCheckCondition(root);
        dynamic nextIndexerValue = IncrementIndexer(checkCondition);
        if (EndOfLoop(checkCondition, nextIndexerValue)) return;
        Loop(checkCondition);
    }

    private void Loop(ForCheckCondition checkCondition)
    {
        _environment.ForChecks.Push(checkCondition);
        _environment.SetNextStatement(checkCondition.Next);
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

    public void VisitOnStatement(On on)
    {
        int selector = (int)Math.Floor((float)Evaluate(on.Selector)) - 1;
        var locations = on.Locations.Select(location => Evaluate(location)).ToList();

        if (selector >= locations.Count || selector < 0) return;

        if (on.IsGosub)
        {
            _environment.ProgramStack.Push(on.Next);
            Expression location = new Literal(locations[selector]);
            Statement jumpToStatement = GetJumpToStatement(on, location, "GOSUB");
            _environment.RunProgram(jumpToStatement, this);

            _environment.SetNextStatement(_environment.ProgramStack.Pop());
            return;
        }

        Statement nextStatement = GetStatementByLineNumber(locations[selector]);
        
        if (nextStatement is null)
            throw new RuntimeStatementException(on.LineNumber, on.SourceLine,
                $"Can't 'GOTO' line {locations[selector]}.");

        _environment.SetNextStatement(nextStatement);
    }

    public void VisitPrintStatement(Print printStatement)
    {
        _sb = new StringBuilder();
        if (printStatement.AtPosition != null) PrintAt(printStatement.AtPosition);

        if (printStatement.Expressions is { Count: > 0 })
            foreach (Expression expression in printStatement.Expressions)
                WriteExpression(expression);

        string text = _sb.ToString();
        _console.Write(text);

        if (!printStatement.WriteNewline) return;

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

    public void VisitReplaceStatement(Replace root)
    {
        if (string.IsNullOrEmpty(root.Line.SourceLine))
            DeleteStatement(root.Line.LineNumber);
        else
            _environment.Program.ReplaceLine(root.Line);
    }

    public void VisitReadStatement(Read root)
    {
        foreach (Expression variable in root.Variables)
            AssignVariable(variable, _environment.Data.GetNext());
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
        return _environment.MemoryInUse();
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
        _environment.SetNextStatement(null);
    }

    public void VisitRunStatement(Run runStatement)
    {
        _environment.InitializeProgram();
        GetCursorPosition();

        int lineNumber = GetStartingLineNumber(runStatement.StartAtLineNumber);
        if (lineNumber < 0)
            lineNumber = GetFirstLineNumber();
        if (lineNumber < 0) return;

        _environment.LoadData(this);
        Statement firstStatement = GetStatementByLineNumber(lineNumber);
        if (firstStatement is null)
            throw new RuntimeStatementException(-1, runStatement.SourceLine, $"Can't start execution at {lineNumber}");

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

    public void VisitListStatement(List listStatement)
    {
        int lineNumber = GetStartingLineNumber(listStatement.StartAtLineNumber);
        _environment.ListProgram(lineNumber);
    }

    public void VisitRemStatement(Rem remStatement)
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


    public void VisitLoadStatement(Load loadStatement)
    {
        _environment.NewProgram();
        string path = Evaluate(loadStatement.Path);
        if (string.IsNullOrEmpty(path))
            path = OpenFileDialog();

        if (string.IsNullOrEmpty(path)) return;

        _environment.LoadProgram(path);
        _console.WriteLine($"Loaded \"{path}\".");
    }

    public void VisitMergeStatement(Merge mergeStatement)
    {
        string path = Evaluate(mergeStatement.Path);
        if (string.IsNullOrEmpty(path))
            path = OpenFileDialog();

        if (string.IsNullOrEmpty(path)) return;

        _environment.LoadProgram(path);
        _console.WriteLine($"Merged \"{path}\".");
    }

    public void VisitSaveStatement(Save saveStatement)
    {
        string path = Evaluate(saveStatement.Path);
        if (string.IsNullOrEmpty(path))
            path = SaveFileDialog();

        if (string.IsNullOrEmpty(path)) return;

        _environment.SaveProgram(path);
        _console.WriteLine($"Saved \"{path}\".");
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
        Statement jumpToStatement = GetJumpToStatement(gotoStatement, gotoStatement.Location, "GOTO");
        _environment.SetNextStatement(jumpToStatement);
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

    public void VisitGosubStatement(Gosub gosubStatement)
    {
        if (gosubStatement.Next != null)
            _environment.ProgramStack.Push(gosubStatement.Next);
        else
            _environment.ProgramStack.Push(CurrentStatement.Next);

        Statement jumpToStatement = GetJumpToStatement(gosubStatement, gosubStatement.Location, "GOSUB");
        _environment.RunProgram(jumpToStatement, this);

        _environment.SetNextStatement(_environment.ProgramStack.Pop());
    }

    public void VisitIfStatement(If ifStatement)
    {
        dynamic value = Evaluate(ifStatement.Condition);
        dynamic check;

        if (value is int intVal)
            check = intVal == 1;
        else
            check = value;

        if (!check) return;

        switch (ifStatement.ThenStatement)
        {
            case Goto gotoStatement:
                VisitGotoStatement(gotoStatement);
                break;
            case Gosub gosubStatement:
                VisitGosubStatement(gosubStatement);
                break;
            default:
                VisitThenStatement(ifStatement);
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

    public void VisitInputStatement(Input inputStatement)
    {
        _sb = new StringBuilder();
        foreach (Expression expression in inputStatement.Expressions)
            ProcessInputExpression(expression, inputStatement.WriteNewline);
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

        GetCursorPosition();
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
        Thread.Sleep(500);
    }

    public void VisitContStatement(Cont root)
    {
        RunProgram(_environment.GetNextStatement(), false);
    }

    public void VisitDataStatement(Data data)
    {
        foreach (Expression element in data.DataElements)
            _environment.Data.Add(Evaluate(element));
    }

    private void DeleteStatement(int lineNumber)
    {
        ParsedLine programLine = _environment.Program.List().FirstOrDefault(l => l.LineNumber == lineNumber);
        if (programLine != null)
            _environment.Program.RemoveLine(programLine);
    }
    public void VisitDeleteStatement(Delete root)
    {
        DeleteStatement(root.LineToDelete);
    }

    private void NewProgram()
    {
        _environment.Program.Clear();
    }
}