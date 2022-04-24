using System;
using Trs80.Level1Basic.Parser.CommandPredicates;
using Trs80.Level1Basic.Parser.Commands;
using Trs80.Level1Basic.Parser.Expressions;
using Trs80.Level1Basic.Parser.Statements;
using Trs80.Level1Basic.Scanner;
using Trs80.Level1Basic.Scanner.Tokens;
using Trs80.Level1Basic.Utilities;

namespace Trs80.Level1Basic.Parser
{
    public class Parser : IParser
    {
        private ScannedStatement _scannedLine;
        private IToken _currentToken;
        private int _tokenIndex;
        private readonly IScanner _scanner;
        private readonly INotifier _notifier;
        private readonly IVariables _variables;
        private readonly ITrs80Console _console;
        private readonly IForCheckConditions _forCheckConditions;

        public Parser(IScanner scanner, INotifier notifier, IVariables variables, IForCheckConditions forCheckConditions,
            ITrs80Console console)
        {
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _forCheckConditions = forCheckConditions ?? throw new ArgumentNullException(nameof(forCheckConditions));
        }
        public ParsedStatement Parse(string line)
        {
            GetFirstToken(line);

            var command = ParseCommand();

            short lineNumber = -1;
            if (command is IStatementCommand statementCommand)
                lineNumber = statementCommand.LineNumber;
            var parsedLine = new ParsedStatement
            {
                OriginalText = _scannedLine.OriginalText,
                Command = command,
                Predicate = ParseCommandPredicate(lineNumber)
            };
            return parsedLine;
        }

        private void GetFirstToken(string line)
        {
            _scannedLine = _scanner.Scan(line);
            _tokenIndex = 0;
            _currentToken = _scannedLine.Tokens[_tokenIndex];
        }

        private void GetNextToken()
        {
            _currentToken = PeekNextToken();
            _tokenIndex++;
        }

        private IToken PeekNextToken()
        {
            return _tokenIndex < _scannedLine.Tokens.Count - 1 ? _scannedLine.Tokens[_tokenIndex + 1] : new EndOfLineToken();
        }

        private ICommandPredicate ParseCommandPredicate(short lineNumber)
        {
            if (EndOfLine()) return new NullPredicate();
            GetNextToken();
            switch (_currentToken)
            {
                case PrintToken _:
                    return ParsePrintPredicate();
                case RemToken remToken:
                    return ParseRemPredicate(remToken);
                case EndToken _:
                    return ParseEndPredicate();
                case LetterToken _:
                    return ParseAssignmentPredicte();
                case LetToken _:
                    return ParseLetPredicate();
                case GotoToken _:
                    return ParseGotoPredicate();
                case IfToken _:
                    return ParseIfPredicate();
                case ForToken _:
                    return ParseForPredicate(lineNumber);
                case NextToken _:
                    return ParseNextPredicate();
                case InputToken _:
                    return ParseInputPredicate();
                case BadToken _:
                    return new NullPredicate();
                case EndOfLineToken _:
                    return new NullPredicate();
                default:
                    throw new InvalidOperationException();
            }
        }

        private ICommandPredicate ParseNextPredicate()
        {
            GetNextToken();
            string variableName = string.Empty;
            if (_currentToken is LetterToken letter)
                variableName = letter.Value;

            return new StatementPredicate(new NextStatement(variableName, _forCheckConditions, _variables, _notifier));
        }

        private ICommandPredicate ParseForPredicate(short lineNumber)
        {
            GetNextToken();
            
            if (!(_currentToken is LetterToken variable))
                throw new InvalidOperationException();
            GetNextToken();
            if (!(_currentToken is EqualsToken))
                throw new InvalidOperationException();
            GetNextToken();
            var startValue = ParseSimpleExpression();
            GetNextToken();
            if (!(_currentToken is ToToken))
                throw new InvalidOperationException();
            GetNextToken();
            var endValue = ParseSimpleExpression();
            GetNextToken();
            IExpression stepValue = new LiteralExpression<int>(1);
            if (_currentToken is StepToken)
            {
                GetNextToken();
                stepValue = ParseSimpleExpression();
            }

            var checkCondition = new ForCheckCondition
            {
                EndValue = endValue, LineNumber = lineNumber, Step = stepValue, StartValue = startValue,
                VariableName = variable.Value
            };
            return new StatementPredicate(new ForStatement(_forCheckConditions, checkCondition, _variables));
        }

        private ICommandPredicate ParseRemPredicate(RemToken remToken)
        {
            GetNextToken();
            return new StatementPredicate(new RemStatement(remToken.Remark));
        }

        private ICommandPredicate ParseInputPredicate()
        {
            GetNextToken();
            string prompt = string.Empty;
            if (_currentToken is LiteralToken<string> promptToken)
            {
                prompt = promptToken.Value;
                GetNextToken();
                GetNextToken();
            }

            if (!(_currentToken is LetterToken letterToken))
                throw new InvalidOperationException();
            return new StatementPredicate(new InputStatement(prompt, letterToken.Value, _variables, _console));
        }

        private ICommandPredicate ParseIfPredicate()
        {
            GetNextToken();
            var conditionalExpression = ParseSimpleExpression();
            var peek = PeekNextToken();
            if (!(peek is ThenToken))
                throw new InvalidOperationException();
            GetNextToken();
            GetNextToken();
            if (!(_currentToken is LiteralToken<int> thenToken))
                throw new InvalidOperationException();
            return new StatementPredicate(new IfStatement(conditionalExpression, (short) thenToken.Value, _notifier));
        }

        private ICommandPredicate ParseGotoPredicate()
        {
            GetNextToken();
            if (!(_currentToken is LiteralToken<int> gotoToken))
                throw new InvalidOperationException();
            return new StatementPredicate(new GotoStatement((short) gotoToken.Value, _notifier));
        }

        private ICommandPredicate ParseEndPredicate()
        {
            GetNextToken();
            return new StatementPredicate(new EndStatement(_notifier));
        }

        private ICommandPredicate ParseLetPredicate()
        {
            GetNextToken();
            return ParseCommandPredicate(-1);
        }

        private ICommandPredicate ParseAssignmentPredicte()
        {
            var assignmentExpression = ParseAssignmentExpression();
            return new StatementPredicate(new LetStatement(assignmentExpression));
        }

        private PrintStatement ParsePrintStatement()
        {
            GetNextToken();
            var printExpression = ParsePrintExpression();
            bool addLineFeed = !(printExpression is IPrintWithoutNewLine);
            return new PrintStatement(printExpression, _console, addLineFeed);

        }

        private ICommandPredicate ParsePrintPredicate()
        {
            return new StatementPredicate(ParsePrintStatement());
        }

        private PrintExpression ParsePrintExpression()
        {
            var left = ParseExpression();
            
            GetNextToken();
            if (EndOfLine()) return new PrintExpression(left, null);

            PrintExpression right;
            switch (_currentToken)
            {
                case CommaToken _:
                    GetNextToken();
                    right = ParsePrintExpression();
                    return new CommaPrintExpression(left, right);
                case SemiColonToken _:
                    GetNextToken();
                    right = ParsePrintExpression();
                    var subLeft = right.Left;
                    var subRight = right.Right;
                    if (right is IPrintWithoutNewLine || (subLeft is NullExpression && subRight is NullExpression))
                        return new SemiColonWithoutNewLinePrintExpression(left, right);
                    return new SemiColonPrintExpression(left, right);
                default:
                    throw new InvalidOperationException();
            }
        }

        private bool EndOfLine()
        {
            return _currentToken is EndOfLineToken;
        }

        private AssignmentExpression ParseAssignmentExpression()
        {
            string variableName = ((LetterToken)_currentToken).Value;
            GetNextToken();
            if (!(_currentToken is EqualsToken))
                throw new InvalidOperationException();

            GetNextToken();
            var value = ParseExpression();

            return new AssignmentExpression(_variables, new VariableExpression(_variables, variableName), value);
        }

        private IExpression ParseFactor()
        {
            switch (_currentToken)
            {
                case null:
                    return new NullExpression();
                case MinusToken _:
                    GetNextToken();
                    return new MultiplyExpression(new LiteralExpression<int>(-1), ParseFactor());
                case LiteralToken<int> numberToken:
                    return new LiteralExpression<int>(numberToken.Value);
                case LiteralToken<string> stringToken:
                    return new LiteralExpression<string>(stringToken.Value);
                case LiteralToken<float> doubleToken:
                    return new LiteralExpression<float>(doubleToken.Value);
                case LetterToken variableToken:
                    return new VariableExpression(_variables, variableToken.Value);
                case AlphaNumericToken wordVariableToken:
                    if (wordVariableToken.Value.ToLower() == "mem")
                        return new VariableExpression(_variables, wordVariableToken.Value);
                    return new NullExpression();
                case LeftParenToken _:
                    GetNextToken();
                    var expression = ParseExpression();
                    var token = PeekNextToken();
                    if (!(token is RighParenToken))
                        throw new InvalidOperationException();
                    GetNextToken();
                    return expression;
                default:
                    return new NullExpression();
            }
        }

        private IExpression ParseTerm()
        {
            var left = ParseFactor();
            if (EndOfLine()) return left;

            var peek = PeekNextToken();
            if (!(peek is IMultiplyOperatorToken)) return left;
            switch (peek)
            {
                case StarToken _:
                    GetNextToken();
                    GetNextToken();
                    return new MultiplyExpression(left, ParseTerm());
                case SlashToken _:
                    GetNextToken();
                    GetNextToken();
                    return new DivideExpression(left, ParseTerm());
                default:
                    throw new InvalidOperationException();
            }
        }

        private IExpression ParseSimpleExpression()
        {
            var left = ParseTerm();
            if (EndOfLine()) return left;

            var peek = PeekNextToken();
            if (!(peek is IAddOperatorToken)) return left;

            switch (peek)
            {
                case PlusToken _:
                    GetNextToken();
                    GetNextToken();
                    return new AddExpression(left, ParseExpression());
                case MinusToken _:
                    GetNextToken();
                    GetNextToken();
                    return new SubtractExpression(left, ParseExpression());
                default:
                    throw new InvalidOperationException();
            }
        }

        private IExpression ParseExpression()
        {
            var left = ParseSimpleExpression();

            if (EndOfLine()) return left;

            var peek = PeekNextToken();
            if (!(peek is IRelationalOperatorToken)) return left;
            switch (peek)
            {
                case LessThanToken _:
                    return ParseLessThanToken(left);
                case GreaterThanToken _:
                    return ParseGreaterThanToken(left);
                case EqualsToken _:
                    return ParseEqualsToken(left);
                default:
                    throw new InvalidOperationException();
            }
        }


        private IExpression ParseEqualsToken(IExpression left)
        {
            GetNextToken();
            GetNextToken();
            return new EqualExpression(left, ParseSimpleExpression());
        }

        private IExpression ParseGreaterThanToken(IExpression left)
        {
            IToken peek;
            GetNextToken();
            peek = PeekNextToken();
            switch (peek)
            {
                case EqualsToken _:
                    GetNextToken();
                    GetNextToken();
                    return new GreaterThanOrEqualExpression(left, ParseSimpleExpression());
                default:
                    GetNextToken();
                    return new GreaterThanExpression(left, ParseSimpleExpression());
            }
        }

        private IExpression ParseLessThanToken(IExpression left)
        {
            IToken peek;
            GetNextToken();
            peek = PeekNextToken();
            switch (peek)
            {
                case EqualsToken _:
                    GetNextToken();
                    GetNextToken();
                    return new LessThanOrEqualExpression(left, ParseSimpleExpression());
                case GreaterThanToken _:
                    GetNextToken();
                    GetNextToken();
                    return new NotEqualExpression(left, ParseSimpleExpression());
                default:
                    GetNextToken();
                    return new LessThanExpression(left, ParseSimpleExpression());
            }
        }

        private ICommand ParseCommand()
        {
            switch (_currentToken)
            {
                case IKeywordToken _:
                    return ParseKeyword();
                case LiteralToken<int> _:
                    return ParseLineNumber();
                case BadToken _:
                    return new BadCommand();
                case AlphaNumericToken _:
                    return new BadCommand();
                default:
                    throw new InvalidOperationException();
            }
        }

        private ICommand ParseLineNumber()
        {
            if (!(_currentToken is LiteralToken<int> lineNumberToken))
                throw new InvalidOperationException();

            if (lineNumberToken.Value < 0)
                return new BadStatementCommand((short)lineNumberToken.Value);
            return new StatementCommand((short)lineNumberToken.Value);
        }

        private ICommand ParseKeyword()
        {
            switch (_currentToken)
            {
                case NewToken _:
                    return new NewCommand();
                case ListToken _:
                    return new ListCommand();
                case RunToken _:
                    return new RunCommand();
                case LoadToken loadToken:
                    return new LoadCommand(loadToken.FileName);
                case SaveToken saveToken:
                    return new SaveCommand(saveToken.FileName);
                case PrintToken _:
                    return ParsePrintStatement();
                default:
                    return new BadCommand();
            }
        }

        //private string BuildPath()
        //{
        //    var sb = new StringBuilder();
        //    GetNextToken();
        //    while (!EndOfLine())
        //    {
        //        switch (_currentToken)
        //        {
        //            case LetterToken letterToken:
        //                sb.Append(letterToken.Value);
        //                break;
        //            case ColonToken _:
        //                sb.Append(":");
        //                break;
        //            case BackSlashToken _:
        //                sb.Append("\\");
        //                break;
        //            case AlphaNumericToken alphaNumericToken:
        //                sb.Append(alphaNumericToken.Value);
        //                break;
        //            case PeriodToken _:
        //                sb.Append(".");
        //                break;
        //            case IKeywordToken keywordToken:

        //            default:
        //                throw new InvalidOperationException();
        //        }
        //        GetNextToken();
        //    }
        //    return sb.ToString();
        //}
    }
}
