using System;
using System.IO;
using Trs80.Level1Basic.Parser;
using Trs80.Level1Basic.Parser.CommandPredicates;
using Trs80.Level1Basic.Parser.Commands;
using Trs80.Level1Basic.Parser.Statements;
using Trs80.Level1Basic.Utilities;

namespace Trs80.Level1Basic
{
    public class BasicEnvironment : IBasicEnvironment
    {
        private readonly IBasicProgram _program;
        private readonly IParser _parser;
        private readonly ITrs80Console _console;

        public BasicEnvironment(IParser parser, INotifier notifier, IBasicProgram basicProgram, ITrs80Console console)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _program = basicProgram ?? throw new ArgumentNullException(nameof(basicProgram));

            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));
            notifier.EnvironmentNotification += EnvironmentNotification;

            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        private void EnvironmentNotification(object sender, EnvironmentNotificationEventArgs e)
        {
            switch (e.Notification)
            {
                case Notification.EndProgram:
                    EndProgram();
                    break;
                case Notification.Goto:
                    Goto(e.LineNumber);
                    break;
                case Notification.GotoSuccessor:
                    GotoSuccessor(e.LineNumber);
                    break;
            }
        }

        private void GotoSuccessor(short lineNumber)
        {
            short successor = _program.GetSuccessor(lineNumber);
            _program.SetNextStatement(successor);
        }

        private void Goto(in short lineNumber)
        {
            _program.SetNextStatement(lineNumber);
        }

        public void ExecuteLine(string line)
        {
            var parsedLine = _parser.Parse(line);
            switch (parsedLine.Command)
            {
                case BadStatementCommand _:
                    _console.WriteLine("HOW?");
                    break;
                case StatementCommand statementCommand when parsedLine.Predicate is NullPredicate:
                    DeleteStatement(statementCommand.LineNumber);
                    break;
                case StatementCommand statementCommand:
                    AddStatement(parsedLine, statementCommand);
                    break;
                default:
                    ExecuteCommand(parsedLine.Command);
                    break;
            }
        }

        private void AddStatement(ParsedStatement line, StatementCommand statementCommand)
        {
            _program.AddStatement(CreateProgramStatement(line, statementCommand));
        }

        private ProgramStatement CreateProgramStatement(ParsedStatement line, StatementCommand statementCommand)
        {
            var programStatement = new ProgramStatement
            {
                LineNumber = statementCommand.LineNumber, OriginalText = line.OriginalText
            };

            if (line.Predicate is StatementPredicate statement) 
                programStatement.Statement = statement.Value;

            return programStatement;
        }

        private void DeleteStatement(short lineNumber)
        {
            _program.DeleteStatement(lineNumber);
        }

        private void ExecuteCommand(ICommand lineCommand)
        {
            switch (lineCommand)
            {
                case NewCommand _:
                    NewProgram();
                    break;
                case RunCommand _:
                    RunProgram();
                    break;
                case ListCommand _:
                    ListProgram();
                    break;
                case SaveCommand saveCommand:
                    SaveProgram(saveCommand.Path);
                    break;
                case LoadCommand loadCommand:
                    LoadProgram(loadCommand.Path);
                    break;
                case BadCommand _:
                    _console.WriteLine("WHAT?");
                    break;
                case PrintStatement printStatement:
                    printStatement.Execute();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void LoadProgram(string path)
        {
            NewProgram();
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
                ExecuteLine(reader.ReadLine());
        }

        private void SaveProgram(string path)
        {
            var oldWriter = _console.InternalWriter;
            using var newWriter = new StreamWriter(path);
            _console.InternalWriter = newWriter;
            _program.List(_console);
            _console.InternalWriter = oldWriter;
        }

        private void ListProgram()
        {
            _program.List(_console);
        }

        private void RunProgram()
        {
            _program.Run();
        }

        private void NewProgram()
        {
            _program.New();
        }

        private void EndProgram()
        {
            _program.End();
        }
    }
}
