using System;
using System.Collections.Generic;
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
    private readonly Dictionary<string, DefFunction> _userFunctions = new(StringComparer.OrdinalIgnoreCase);
    private readonly IMachine _machine;
    private readonly ITrs80 _trs80;
    private readonly ITrs80Api _trs80Api;
    private readonly IProgram _program;
    private readonly IHost _host;
    private readonly IAppSettings _appSettings;
    private readonly BasicLanguageLevel _basicLevel;
    private readonly PendingEditRequest _pendingEdit;
    private bool _omitBlankLineBeforePrompt;
    private IStatement _errorHandlerStatement;
    private IStatement _errorResumeStatement;
    private bool _handlingError;
    private bool _traceEnabled;

    public Interpreter(IHost host, ITrs80 trs80, ITrs80Api trs80Api,
        IMachine machine, IProgram program, IAppSettings appSettings,
        PendingEditRequest pendingEdit)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _trs80Api = trs80Api ?? throw new ArgumentNullException(nameof(trs80Api));
        _machine = machine ?? throw new ArgumentNullException(nameof(machine));
        _program = program ?? throw new ArgumentNullException(nameof(program));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _basicLevel = _appSettings.BasicLevel;
        _pendingEdit = pendingEdit ?? throw new ArgumentNullException(nameof(pendingEdit));
    }

    private dynamic Divide(dynamic left, dynamic right)
    {
        if (right == 0)
            throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, "Divide by zero");

        if (_basicLevel == BasicLanguageLevel.Level1)
            return (float)left / right;

        return left is double || right is double ? (double)left / right : (float)left / right;
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
            if (ex is ParseException parseException && parseException.LineNumber >= 0)
                _machine.Set("ERL", parseException.LineNumber);

            if (statement.LineNumber < 0 && statement is not Run
                && ex is ValueOutOfRangeException valueOutOfRange
                && valueOutOfRange.Message == "Divide by zero")
            {
                _machine.Set("ERL", 65535);
                _trs80.WriteLine("?/0 ERROR");
                return;
            }

            ExceptionHandler.HandleError(_trs80, _appSettings, ex, _pendingEdit);
        }

    }

    private dynamic Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    public dynamic VisitArrayExpression(Array expression)
    {
        dynamic index = Evaluate(expression.Index);
        if (index > _trs80Api.GetMaxArrayIndex())
            throw new ProgramTooLargeException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, expression.LinePosition, "Insufficient memory.");

        try
        {
        if (expression.Index2 == null)
                return _machine.Get(expression.Name.Lexeme, index);

            dynamic index2 = Evaluate(expression.Index2);
            if (index2 > _trs80Api.GetMaxArrayIndex())
                throw new ProgramTooLargeException(_program.CurrentStatement.LineNumber,
                    _program.CurrentStatement.SourceLine, expression.LinePosition, "Insufficient memory.");

            if (expression.Index3 is not null)
            {
                dynamic index3 = Evaluate(expression.Index3);
                if (index3 > _trs80Api.GetMaxArrayIndex())
                    throw new ProgramTooLargeException(_program.CurrentStatement.LineNumber,
                        _program.CurrentStatement.SourceLine, expression.LinePosition, "Insufficient memory.");

                return _machine.Get(expression.Name.Lexeme, index, index2, index3);
            }

            return _machine.Get(expression.Name.Lexeme, index, index2);
        }
        catch (ValueOutOfRangeException exception)
        {
            throw new RuntimeStatementException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, expression.LinePosition, exception.Message);
        }
    }

    private void Assign(Expression expression, dynamic value)
    {
        switch (expression)
        {
            case Identifier identifier:
                if (_machine.IsStringVariable(identifier.Name.Lexeme) != (value is string))
                    throw new TypeMismatchException(_program.CurrentStatement.LineNumber,
                        _program.CurrentStatement.SourceLine,
                        identifier.LinePosition - identifier.Name.Lexeme.TrimEnd('$', '%', '!', '#').Length,
                        "Assignment target and value types do not match.");

                _machine.Set(identifier.Name.Lexeme, value);
                break;
            case Array array:
                {
                    if (_machine.IsStringVariable(array.Name.Lexeme) != (value is string))
                        throw new TypeMismatchException(_program.CurrentStatement.LineNumber,
                            _program.CurrentStatement.SourceLine,
                            array.LinePosition,
                            "Assignment target and value types do not match.");

                        try
                    {
                            dynamic index = Evaluate(array.Index);
                            if (array.Index2 == null)
                            {
                                _machine.Set(array.Name.Lexeme, index, value);
                                break;
                            }

                            dynamic index2 = Evaluate(array.Index2);
                            if (array.Index3 is not null)
                            {
                                dynamic index3 = Evaluate(array.Index3);
                                _machine.Set(array.Name.Lexeme, index, index2, index3, value);
                            }
                            else
                                _machine.Set(array.Name.Lexeme, index, index2, value);
                    }
                        catch (ValueOutOfRangeException exception)
                        {
                            throw new RuntimeStatementException(_program.CurrentStatement.LineNumber,
                                _program.CurrentStatement.SourceLine, array.LinePosition, exception.Message);
                        }
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
            TokenType.Slash => Divide(left, right),
            TokenType.Mod => right == 0 ? throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber, _program.CurrentStatement.SourceLine, "Divide by zero") : left % right,
            TokenType.Star => (left is bool && right is bool) ? left && right : left * right,
            TokenType.Caret => Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right)),
            TokenType.And => TruthValue(IsTruthy(left) && IsTruthy(right)),
            TokenType.Or => TruthValue(IsTruthy(left) || IsTruthy(right)),
            TokenType.Xor => TruthValue(IsTruthy(left) ^ IsTruthy(right)),
            TokenType.Eqv => TruthValue(IsTruthy(left) == IsTruthy(right)),
            TokenType.Imp => TruthValue(!IsTruthy(left) || IsTruthy(right)),
            TokenType.GreaterThan => TruthValue(CompareValues(left, right) > 0),
            TokenType.GreaterThanOrEqual => TruthValue(CompareValues(left, right) >= 0),
            TokenType.LessThan => TruthValue(CompareValues(left, right) < 0),
            TokenType.LessThanOrEqual => TruthValue(CompareValues(left, right) <= 0),
            TokenType.NotEqual => TruthValue(!IsEqual(left, right)),
            TokenType.Equal => TruthValue(IsEqual(left, right)),
            _ => null
        };
    }

    public dynamic VisitCallExpression(Call expression)
    {
        var arguments = expression.Arguments.Select(argument => Evaluate(argument)).ToList();

        if (expression.Callee == null)
        {
            if (!_userFunctions.TryGetValue(expression.Name, out DefFunction function) || arguments.Count != 1)
                throw new RuntimeExpressionException(_program.CurrentStatement.LineNumber, _program.CurrentStatement.SourceLine,
                    expression.LinePosition, "Unknown user-defined function.");

            dynamic previous = _machine.Get(function.Parameter);
            _machine.Set(function.Parameter, arguments[0]);
            try
            {
                return Evaluate(function.Body);
            }
            finally
            {
                _machine.Set(function.Parameter, previous);
            }
        }

        return expression.Callee.Call(_trs80Api, arguments);
    }

    public dynamic VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    public dynamic VisitIdentifierExpression(Identifier expression)
    {
        string name = expression.Name.Lexeme;
        dynamic value = _machine.Get(name);
        if (name.EndsWith('%') && value is int integerValue
            && (integerValue < short.MinValue || integerValue > short.MaxValue))
            throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, "Integer value out of range.");
        if (name.EndsWith('!') && value is float singleValue
            && Math.Abs(singleValue) > 1.701411E+38f)
            throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, "Single value out of range.");
        if (name.EndsWith('#') && value is double doubleValue
            && Math.Abs(doubleValue) > 1.701411834544556E+38)
            throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber,
                _program.CurrentStatement.SourceLine, "Double value out of range.");

        return value;
    }

    public dynamic VisitLiteralExpression(Literal expression)
    {
        switch (expression.Value)
        {
            case float:
                return expression.Value;
            case double:
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

        if (expression.UnaryOperator.Type == TokenType.Not)
            return TruthValue(!IsTruthy(right));

        CheckNumericOperand(expression.UnaryOperator, right);
        return expression.UnaryOperator.Type == TokenType.Plus ? right : -1 * right;
    }

    private void CheckNumericOperand(Token operatorType, dynamic operand)
    {
        switch (operand)
        {
            case float:
            case int:
            case double:
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
            case string when right is string && (operatorType.Type == TokenType.Plus || IsRelational(operatorType.Type)):
            case float when right is float:
            case float when right is int:
            case float when right is double:
            case int when right is float:
            case int when right is int:
            case int when right is double:
            case double when right is double:
            case double when right is float:
            case double when right is int:
                return;
            default:
                throw new TypeMismatchException(_program.CurrentStatement.LineNumber,
                    _program.CurrentStatement.SourceLine, operatorType.LinePosition,
                    "Operands are of incompatible types.");
        }
    }

    private static bool IsTruthy(dynamic value)
    {
        return value switch
        {
            null => false,
            bool boolValue => boolValue,
            int intValue => intValue != 0,
            float floatValue => floatValue != 0,
            double doubleValue => doubleValue != 0,
            _ => false
        };
    }

    private static int TruthValue(bool value) => value ? -1 : 0;

    private static bool IsRelational(TokenType type)
    {
        return type is TokenType.GreaterThan or TokenType.GreaterThanOrEqual
            or TokenType.LessThan or TokenType.LessThanOrEqual
            or TokenType.NotEqual or TokenType.Equal;
    }

    private static int CompareValues(dynamic left, dynamic right)
    {
        if (left is string leftString && right is string rightString)
            return string.CompareOrdinal(leftString, rightString);

        return Convert.ToDouble(left, System.Globalization.CultureInfo.InvariantCulture)
            .CompareTo(Convert.ToDouble(right, System.Globalization.CultureInfo.InvariantCulture));
    }

    private static bool IsEqual(dynamic left, dynamic right)
    {
        if (left == null && right == null) return true;
        if (left is int or float or double && right is int or float or double)
            return Convert.ToDouble(left, System.Globalization.CultureInfo.InvariantCulture)
                == Convert.ToDouble(right, System.Globalization.CultureInfo.InvariantCulture);

        return left != null && (bool)left.Equals(right);
    }

    private string Stringify(dynamic value)
    {
        StringBuilder sb = new();
        if (value is >= 0 or float and >= 0 or double and >= 0)
            sb.Append(' ');

        switch (value)
        {
            case float:
                sb.Append(StringifyFloat(value));
                break;
            case double:
                sb.Append(StringifyDouble(value));
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

        if (value is (int or float or double))
            sb.Append(' ');

        _machine.CursorX += sb.Length;
        return sb.ToString();
    }

    private string StringifyDouble(double value)
    {
        if (value == .01)
            return ".01";

        switch (value)
        {
            case 0:
                return "0";
            case < .05 and > -.05:
                return value.ToString("0.#####E+00");
            case < 1 and > -1:
                return value.ToString(".#######");
            case >= 1e20 or <= -1e20:
                // Use limited precision scientific notation for extremely large numbers
                return value.ToString("0.#####E+00");
            default:
                return value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    private string StringifyInt(int value)
    {
        return value is <= 1000000 and >= -1000000 ? value.ToString() : value.ToString("0.#####E+00");
    }

    private string StringifyFloat(float value)
    {
        if (value == .01f)
            return ".01";

        switch (value)
        {
            case 0:
                return "0";
            case < .05f and > -.05f:
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
        if (_traceEnabled && statement.LineNumber >= 0)
            _trs80.Write($"({statement.LineNumber})");
        try
        {
            statement.Accept(this);
        }
        catch (Exception exception) when (CanHandleError(statement, exception))
        {
            _machine.Set("ERL", statement.LineNumber);
            _errorResumeStatement = _machine.GetNextStatement(statement);
            _handlingError = true;
            _machine.SetNextStatement(_errorHandlerStatement);
        }
    }

    private bool CanHandleError(IStatement statement, Exception exception)
    {
        return statement.LineNumber >= 0
            && _errorHandlerStatement is not null
            && !_handlingError
            && exception is not ReturnFromGosub
            && exception is not ParseException;
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
        {
            _machine.Initialize();
            RegisterUserFunctions();
            ResetErrorState();
            _traceEnabled = false;
        }

        _machine.RunStatementList(statement, this);

        WritePrompt();
    }

    private void RegisterUserFunctions()
    {
        _userFunctions.Clear();
        foreach (IStatement statement in _machine.Program.List())
        {
            if (statement is DefFunction function)
                _userFunctions[function.Name] = function;
        }
    }

    public Void VisitDataStatement(Data statement)
    {
        foreach (Expression element in statement.DataElements)
            _machine.Data.Add(Evaluate(element), statement.LineNumber);

        return null!;
    }

    public Void VisitBeepStatement(Beep statement)
    {
        _host.Beep();
        return null!;
    }

    public Void VisitClearStatement(Clear statement)
    {
        _machine.Initialize();
        if (statement.Length is not null)
            _machine.SetStringCapacity((int)Evaluate(statement.Length));

        return null!;
    }

    public Void VisitDefTypeStatement(DefType statement)
    {
        foreach (string name in statement.Names)
            _machine.SetVariableType(name, statement.Type);

        return null!;
    }

    public Void VisitDefFunctionStatement(DefFunction statement)
    {
        _userFunctions[statement.Name] = statement;
        return null!;
    }

    public Void VisitDimStatement(Dim statement)
    {
        foreach (Array array in statement.Dimensions)
        {
            string name = array.Name.Lexeme;
            int index = (int)Evaluate(array.Index);

            if (array.Index2 == null)
            {
                _machine.SetArrayDimensions(name, index);
                if (!_machine.Exists(name))
                    _machine.Set(name, index, 0);
                else
                    _machine.Set(name, index, _machine.Get(name, index));
                continue;
            }

            int index2 = (int)Evaluate(array.Index2);
            if (array.Index3 is not null)
            {
                int index3 = (int)Evaluate(array.Index3);
                _machine.SetArrayDimensions(name, index, index2, index3);
                _machine.Set(name, index, index2, index3, 0);
                continue;
            }

            _machine.SetArrayDimensions(name, index, index2);
            if (!_machine.Exists(name))
                _machine.Set(name, index, index2, 0);
            else
                _machine.Set(name, index, index2, _machine.Get(name, index, index2));
        }

        return null!;
    }

    public Void VisitDeleteStatement(Delete statement)
    {
        DeleteStatement(statement.LineToDelete, statement.EndLineToDelete);

        return null!;
    }

    private void DeleteStatement(int startLine, int? endLine)
    {
        int lastLine = endLine ?? startLine;
        int firstLine = Math.Min(startLine, lastLine);
        lastLine = Math.Max(startLine, lastLine);
        List<IStatement> statements = _machine.Program.List()
            .Where(statement => statement.LineNumber >= firstLine && statement.LineNumber <= lastLine)
            .ToList();

        foreach (IStatement statement in statements)
            _machine.Program.RemoveStatement(statement);
    }

    public Void VisitEndStatement(End statement)
    {
        _machine.SetNextStatement(null);
        _machine.HaltRun();

        return null!;
    }

    public Void VisitForStatement(For statement)
    {
        if (statement.Identifier is not Identifier && statement.Identifier is not Array)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Identifier.LinePosition, "Expected variable after 'FOR'.");

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
                int nextIndex = lan.Next.IdentifierNames.FindIndex(identifierName =>
                    string.Equals(identifierName, statement.IdentifierName.Lexeme,
                        StringComparison.OrdinalIgnoreCase));
                if (lan.Next.IdentifierNames.Count > 0 && nextIndex < 0)
                    throw;

                current = IncrementIndexer(statement.Identifier, step);

                if (current > end && nextIndex >= 0 &&
                    nextIndex < lan.Next.IdentifierNames.Count - 1)
                    throw new LoopAfterNext(new Next(
                        lan.Next.IdentifierNames.Skip(nextIndex + 1).ToList()));
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
            throw new UndefinedLineException(statement.LineNumber, statement.SourceLine,
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
        if (statement.LineNumber < 0)
            _machine.RunStatementList(jumpToStatement, this);
        else
            _machine.SetNextStatement(jumpToStatement);

        return null!;
    }

    public Void VisitIfStatement(If statement)
    {
        dynamic logicalExpression = Evaluate(statement.Condition);
        if (logicalExpression is string)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.ThenPosition, "Cannot convert string to logical expression.");

        if (!IsTruthy(logicalExpression))
        {
            if (statement.ElseBranch != null)
                _machine.RunCompoundStatement(statement.ElseBranch, this);
            return null!;
        }

        if (statement.ThenException != null)
            throw statement.ThenException;

        _machine.RunCompoundStatement(statement.ThenBranch, this);

        return null!;
    }

    public Void VisitInputStatement(Input statement)
    {
        if (TryProcessCommaSeparatedInput(statement.Expressions))
            return null!;

        foreach (Expression expression in statement.Expressions)
            ProcessInputExpression(expression);

        return null!;
    }

    private bool TryProcessCommaSeparatedInput(List<Expression> expressions)
    {
        List<Expression> variables = expressions
            .Where(expression => expression is Identifier or Array)
            .ToList();
        bool hasOnlyVariablesAndSeparators = variables.Count > 1 &&
            expressions.Any(expression => expression is Call) &&
            expressions.All(expression => expression is Identifier or Array or Call);

        if (!hasOnlyVariablesAndSeparators)
            return false;

        _trs80.Write("? ");
        string[] values = (_trs80.ReadLine() ?? string.Empty).Split(',');
        for (int index = 0; index < variables.Count; index++)
        {
            if (index < values.Length && !string.IsNullOrWhiteSpace(values[index]))
                AssignInputValue(variables[index], values[index].Trim());
            else
                GetInputValue(variables[index], "?? ");
        }

        return true;
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

    private void GetInputValue(Expression identifier, string prompt = "? ")
    {
        _trs80.Write(prompt);

        string value = _trs80.ReadLine();
        AssignInputValue(identifier, value);
    }

    private void AssignInputValue(Expression identifier, string value)
    {
        try
        {
            if (value is null)
                Assign(identifier, null);
            else if (int.TryParse(value, out int intValue))
            {
                string targetName = identifier switch
                {
                    Identifier variable => variable.Name.Lexeme,
                    Array array => array.Name.Lexeme,
                    _ => string.Empty
                };
                bool integerTarget = _machine.IsIntegerVariable(targetName)
                    || _program.CurrentStatement.SourceLine.Contains($"{targetName}%",
                        StringComparison.OrdinalIgnoreCase);
                if (integerTarget &&
                    (intValue < short.MinValue || intValue > short.MaxValue))
                    throw new ValueOutOfRangeException(-1, string.Empty, "Integer value out of range.");

                Assign(identifier, intValue);
            }
            else if (float.TryParse(value, out float floatValue))
                Assign(identifier, floatValue);
            else if (_machine.Exists(value))
            {
                dynamic lookup = _machine.Get(value);
                Assign(identifier, lookup);
            }
            else
                Assign(identifier, value);
        }
        catch (ValueOutOfRangeException)
        {
            if (_program.CurrentStatement.LineNumber >= 0 && _errorHandlerStatement is not null)
                throw;

            _trs80.WriteLine("WHAT?");
            GetInputValue(identifier);
        }
        catch (TypeMismatchException)
        {
            if (_program.CurrentStatement.LineNumber >= 0 && _errorHandlerStatement is not null)
                throw;

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
        if (statement.IsLastLine)
        {
            if (_program.LastLineNumber.HasValue)
                _machine.ListProgram(_program.LastLineNumber.Value, _program.LastLineNumber.Value);

            WritePrompt();
            return null!;
        }

        int lineNumber = GetStartingLineNumber(statement.StartAtLineNumber);
        int? endLineNumber = statement.EndAtLineNumber is null
            ? null
            : GetStartingLineNumber(statement.EndAtLineNumber);
        _machine.ListProgram(lineNumber, endLineNumber);
        WritePrompt();

        return null!;
    }

    public Void VisitLprintStatement(Lprint statement)
    {
        var output = new StringBuilder();
        if (statement.AtPosition != null)
            output.Append(Stringify(Evaluate(statement.AtPosition)));

        foreach (Expression expression in statement.Expressions)
            output.Append(Stringify(Evaluate(expression)));

        if (statement.WriteNewline)
            output.AppendLine();

        _host.Print(output.ToString());
        return null!;
    }

    public Void VisitLlistStatement(Llist statement)
    {
        int firstLine = GetStartingLineNumber(statement.StartAtLineNumber);
        int? endLine = statement.EndAtLineNumber is null
            ? null
            : GetStartingLineNumber(statement.EndAtLineNumber);
        int lastLine = endLine ?? int.MaxValue;
        int normalizedFirstLine = Math.Min(firstLine, lastLine);
        int normalizedLastLine = Math.Max(firstLine, lastLine);
        var output = new StringBuilder();

        foreach (IStatement programStatement in _machine.Program.List()
            .Where(programStatement => programStatement.LineNumber >= normalizedFirstLine &&
                programStatement.LineNumber <= normalizedLastLine))
        {
            output.AppendLine(programStatement.LineNumber >= 0
                ? $" {programStatement.LineNumber}  {programStatement.SourceLine}"
                : programStatement.SourceLine);
        }

        _host.Print(output.ToString());
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
        if (statement.Path is not Literal literalPath)
            throw new ParseException(statement.LineNumber, statement.SourceLine,
                statement.Path.LinePosition, "Path must be a quoted string.");
        string path = literalPath.Value;

        if (string.IsNullOrEmpty(path))
            path = _host.GetFileNameForLoad();

        if (string.IsNullOrEmpty(path)) return null!;

    _machine.NewProgram();
        ResetErrorState();
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

    public Void VisitMidAssignmentStatement(MidAssignment statement)
    {
        string value = (string)Evaluate(statement.Value);
        string current = (string)_machine.Get(statement.Target.Name.Lexeme);
        int start = (int)Evaluate(statement.Start);
        int length = statement.Length is null ? value.Length : (int)Evaluate(statement.Length);

        _machine.Set(statement.Target.Name.Lexeme, _trs80Api.MidAssign(current, start, length, value));
        return null!;
    }

    public Void VisitNewStatement(New statement)
    {
        _machine.Program.Clear();
        _machine.SetNextStatement(null);
        ResetErrorState();
        _userFunctions.Clear();

        return null!;
    }

    private void ResetErrorState()
    {
        _machine.Set("ERL", 0);
        _errorHandlerStatement = null;
        _errorResumeStatement = null;
        _handlingError = false;
    }

    public Void VisitNextStatement(Next statement)
    {
        throw new LoopAfterNext(statement);
    }

    public Void VisitTronStatement(Tron statement)
    {
        _traceEnabled = true;
        return null!;
    }

    public Void VisitTroffStatement(Troff statement)
    {
        _traceEnabled = false;
        return null!;
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

        if (selector >= locations.Count || selector < 0)
        {
            if (statement.LineNumber >= 0)
                throw new FlowControlException(statement.LineNumber, statement.SourceLine,
                    statement.Selector.LinePosition, "ON selector is outside the target list.");

            return null!;
        }

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
            throw new UndefinedLineException(statement.LineNumber, statement.SourceLine,
                linePositions[selector], $"Can't 'GOTO' line {locations[selector]}.");

        if (statement.LineNumber < 0)
            _machine.RunStatementList(nextStatement, this);
        else
            _machine.SetNextStatement(nextStatement);

        return null!;
    }

    public Void VisitOnErrorStatement(OnError statement)
    {
        int lineNumber = (int)Evaluate(statement.Location);
        if (lineNumber == 0)
        {
            _errorHandlerStatement = null;
            _errorResumeStatement = null;
            _handlingError = false;
            return null!;
        }

        _errorHandlerStatement = GetStatementByLineNumber(lineNumber);
        if (_errorHandlerStatement is null)
            throw new UndefinedLineException(statement.LineNumber, statement.SourceLine,
                statement.Location.LinePosition, $"Can't GOTO line {lineNumber}");

        return null!;
    }

    public Void VisitResumeStatement(Resume statement)
    {
        if (_errorResumeStatement is null)
            throw new FlowControlException(statement.LineNumber, statement.SourceLine,
                0, "RESUME without an active error.");

        if (statement.Location is null)
            _machine.SetNextStatement(_errorResumeStatement);
        else
        {
            int lineNumber = (int)Evaluate(statement.Location);
            IStatement resumeStatement = GetStatementByLineNumber(lineNumber);
            if (resumeStatement is null)
                throw new UndefinedLineException(statement.LineNumber, statement.SourceLine,
                    statement.Location.LinePosition, $"Can't GOTO line {lineNumber}");

            _machine.SetNextStatement(resumeStatement);
        }
        _errorResumeStatement = null;
        _handlingError = false;
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
        if (statement.ParseException?.Message.Contains("Expected ')'", StringComparison.Ordinal) == true)
            throw statement.ParseException;

        if (statement.AtPosition != null)
            PrintAt(statement.AtPosition);

        if (statement.UsingFormat is not null)
        {
            string image = Convert.ToString(Evaluate(statement.UsingFormat),
                System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            foreach (Expression expression in statement.Expressions)
                _trs80.Write(FormatUsing(image, Evaluate(expression)));
        }
        else if (statement.Expressions is { Count: > 0 })
            foreach (Expression expression in statement.Expressions)
                _trs80.Write(Stringify(Evaluate(expression)));

        if (!statement.WriteNewline && statement.ParseException == null) return null!;

        _trs80.WriteLine();
        _machine.CursorX = 0;
        _machine.CursorY++;

        if (statement.ParseException != null)
            throw statement.ParseException;

        return null!;
    }

    private string FormatUsing(string image, dynamic value)
    {
        if (value is string)
            return FormatUsingString(image, (string)value);

        int firstDigit = image.IndexOf('#');
        if (firstDigit < 0)
            return image;

        int lastDigit = image.LastIndexOf('#');
        int decimalPoint = image.IndexOf('.', firstDigit, lastDigit - firstDigit + 1);
        int fractionalDigits = decimalPoint < 0 ? 0 : image[(decimalPoint + 1)..(lastDigit + 1)].Count(character => character == '#');
        int integerDigits = image[firstDigit..(decimalPoint < 0 ? lastDigit + 1 : decimalPoint)]
            .Count(character => character == '#');

        decimal number = Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
        bool negative = number < 0;
        number = Math.Round(Math.Abs(number), fractionalDigits, MidpointRounding.AwayFromZero);
        string numeric = number.ToString($"F{fractionalDigits}", System.Globalization.CultureInfo.InvariantCulture);
        string[] parts = numeric.Split('.');
        if (parts[0].Length > integerDigits)
            return new string('%', lastDigit - firstDigit + 1);

        string integerPart = parts[0].PadLeft(integerDigits);
        if (image.Contains(','))
            integerPart = AddUsingCommas(integerPart);

        string prefix = image[..firstDigit];
        string suffix = image[(lastDigit + 1)..];
        bool leadingSign = prefix.EndsWith('+') || prefix.EndsWith('-');
        bool trailingSign = suffix.StartsWith('+') || suffix.StartsWith('-');
        string sign = negative ? "-" : "+";
        if (leadingSign)
            prefix = prefix[..^1] + (negative || prefix[^1] == '+' ? sign : " ");
        if (trailingSign)
            suffix = (negative || suffix[0] == '+' ? sign : " ") + suffix[1..];

        string result = prefix + integerPart;
        if (fractionalDigits > 0)
            result += "." + parts[1];
        result += suffix;

        if (image.Contains("$$", StringComparison.Ordinal))
        {
            result = result.Replace("$$", "$", StringComparison.Ordinal);
            int currencyPosition = result.IndexOf('$');
            result = result.Remove(currencyPosition, 1);
            result = result.Insert(result.IndexOfAny("0123456789".ToCharArray()), "$");
        }

        if (negative && !leadingSign && !trailingSign)
            result = result.Insert(0, "-");

        return result;
    }

    private static string AddUsingCommas(string integerPart)
    {
        int firstDigit = integerPart.IndexOfAny("0123456789".ToCharArray());
        if (firstDigit < 0) return integerPart;

        string digits = integerPart[firstDigit..];
        for (int index = digits.Length - 3; index > 0; index -= 3)
            digits = digits.Insert(index, ",");

        return integerPart[..firstDigit] + digits;
    }

    private static string FormatUsingString(string image, string value)
    {
        int width = image switch
        {
            "!" => 1,
            "%%" => 2,
            _ when image.StartsWith('%') && image.EndsWith('%') => image.Length - 2,
            _ => 0
        };

        return width > 0 ? value.PadRight(width)[..width] : image;
    }

    private void PrintAt(Expression position)
    {
        int displayPosition = NormalizeDisplayPosition(Evaluate(position));
        int row = displayPosition / 64;
        int column = displayPosition % 64;

        _trs80.SetCursorPosition(column, row);

        _machine.CursorX = column;
        _machine.CursorY = row;
    }

    private static int NormalizeDisplayPosition(dynamic value)
    {
        int normalized = (int)Math.Floor(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture));
        const int screenSize = 64 * 16;
        return ((normalized % screenSize) + screenSize) % screenSize;
    }

    public Void VisitReadStatement(Read statement)
    {
        foreach (Expression variable in statement.Variables)
        {
            (Token target, int linePosition) = variable switch
            {
                Identifier identifier => (identifier.Name,
                    identifier.LinePosition - identifier.Name.Lexeme.TrimEnd('$', '%', '!', '#').Length),
                Array array => (array.Name, array.Name.LinePosition),
                _ => (null, 0)
            };
            dynamic value;
            try
            {
                value = _machine.Data.GetNext();
            }
            catch (OutOfDataException exception)
            {
                throw new OutOfDataException(_program.CurrentStatement.LineNumber,
                    _program.CurrentStatement.SourceLine, linePosition, exception.Message);
            }

            if (target != null && _machine.IsStringVariable(target.Lexeme) != (value is string))
                throw new TypeMismatchException(_program.CurrentStatement.LineNumber,
                    _program.CurrentStatement.SourceLine, linePosition,
                    "READ source and target types do not match.");
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
    public Void VisitRestoreStatement(Restore statement)
#pragma warning restore S927 // Parameter names should match base declaration and other partial definitions
    {
        if (statement.Location is null)
            _machine.Data.MoveFirst();
        else
            _machine.Data.MoveToLine((int)Evaluate(statement.Location));

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
        if (!_omitBlankLineBeforePrompt)
            _trs80.WriteLine();

        _omitBlankLineBeforePrompt = false;
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
        _trs80.WriteLine($"BREAK IN {statement.LineNumber}");
        _omitBlankLineBeforePrompt = statement.LineNumber >= 0;
        _machine.HaltRun();

        return null!;
    }

    public Void VisitOutStatement(Out statement)
    {
        return null!;
    }

    public Void VisitWaitStatement(Wait statement)
    {
        return null!;
    }

    public Void VisitSystemStatement(SystemStatement statement)
    {
        _machine.HaltRun();
        return null!;
    }

    public Void VisitResetStatement(ResetStatement statement)
    {
        _trs80.Reset((float)Evaluate(statement.X), (float)Evaluate(statement.Y));
        return null!;
    }

    public Void VisitSetStatement(SetStatement statement)
    {
        _trs80.Set((float)Evaluate(statement.X), (float)Evaluate(statement.Y));
        return null!;
    }

    public Void VisitPokeStatement(PokeStatement statement)
    {
        _trs80Api.Poke((int)Evaluate(statement.Address), (int)Evaluate(statement.Value));
        return null!;
    }
}