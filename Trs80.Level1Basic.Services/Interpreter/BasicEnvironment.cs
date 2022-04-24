using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Trs80.Level1Basic.Services.Parser;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Interpreter
{
    public class BasicEnvironment : IBasicEnvironment
    {
        private readonly GlobalVariables _globals = new GlobalVariables();

        public Stack<ForCheckCondition> ForChecks { get; } = new Stack<ForCheckCondition>();
        public Stack<Statement> ProgramStack { get; } = new Stack<Statement>();
        private readonly IBuiltinFunctions _builtins;
        public DataElements Data { get; } = new DataElements();
        public List<Line> ProgramLines { get; set; } = new List<Line>();
        public List<Statement> ProgramStatements { get; set; } = new List<Statement>();

        private readonly ITrs80Console _console;
        private readonly IParser _parser;

        public BasicEnvironment(ITrs80Console console, IParser parser, IBuiltinFunctions builtins)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _builtins = builtins ?? throw new ArgumentNullException(nameof(builtins));
        }

        public bool Halted => _runHalted;

        public dynamic AssignVariable(string name, dynamic value)
        {
            return _globals.Assign(name, value);
        }

        public dynamic AssignArray(string name, int index, dynamic value)
        {
            return _globals.AssignArray(name, index, value);
        }

        public dynamic GetVariable(string name)
        {
            return _globals.Get(name);
        }

        public bool VariableExists(string name)
        {
            return _globals.Exists(name);
        }

        public FunctionDefinition GetFunctionDefinition(string name)
        {
            try
            {
                return _builtins.Get(name);
            }
            catch
            {
                return null;
            }
        }

        public void GetProgramStatements()
        {
            ProgramStatements.Clear();
            _sorted = false;
            SortProgram();

            Statement last = null;
            foreach (var statement in ProgramLines.SelectMany(line => line.Statements))
            {
                if (last != null)
                    last.Next = statement;
                ProgramStatements.Add(statement);
                last = statement;
            }
        }

        private bool _sorted = false;
        public void ReplaceProgramLine(Line line)
        {
            ProgramLines.RemoveAll(l => l.LineNumber == line.LineNumber);
            ProgramLines.Add(line);
            _sorted = false;
        }

        public void SortProgram()
        {
            if (_sorted) return;

            ProgramLines = ProgramLines.OrderBy(l => l.LineNumber).ToList();
            _sorted = true;
        }

        public void ListProgram(int lineNumber)
        {
            SortProgram();

            foreach (var line in ProgramLines.Where(s => s.LineNumber >= lineNumber))
                _console.WriteLine($"{line.SourceLine}");
        }

        public void SaveProgram(string path)
        {
            var oldWriter = _console.InternalWriter;
            using var newWriter = new StreamWriter(path);
            _console.InternalWriter = newWriter;

            ListProgram(0);

            _console.InternalWriter = oldWriter;
        }

        public void LoadProgram(string path)
        {
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
                ProgramLines.Add(_parser.Parse(reader.ReadLine()));

            _sorted = false;
            SortProgram();
        }

        public void NewProgram()
        {
            ProgramLines.Clear();
            Initialize();
        }

        private bool _runHalted = false;
        private Statement _nextStatement;
        public void RunProgram(Statement statement, IBasicInterpreter interpreter)
        {
            _runHalted = false;

            while (statement != null && !_runHalted)
            {
                _nextStatement = statement.Next;
                interpreter.Execute(statement);
                statement = _nextStatement;
            }
        }


        public Statement GetStatementByLineNumber(int lineNumber)
        {
            return ProgramStatements.FirstOrDefault(s => s.LineNumber >= lineNumber && !(s is Data));
        }


        public void SetNextStatement(Statement statement)
        {
            _nextStatement = statement;
        }

        public void HaltRun()
        {
            _runHalted = true;
        }

        public void LoadData(IBasicInterpreter interpreter)
        {
            Data.Clear();

            foreach (var dataStatement in ProgramLines.SelectMany(s => s.Statements).Where(s => s is Data))
                interpreter.Execute(dataStatement);
        }

        public Statement GetNextStatement()
        {
            return _nextStatement;
        }

        public void Initialize()
        {
            _globals.Clear();
            Data.MoveFirst();
            SortProgram();
        }

        public dynamic GetArrayValue(string name, int index)
        {
            return _globals.GetArrayValue(name, index);
        }
    }
}
