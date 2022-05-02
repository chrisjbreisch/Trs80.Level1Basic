using System;
using System.Collections.Generic;
using System.Linq;
using Trs80.Level1Basic.Parser.Expressions;
using Trs80.Level1Basic.Utilities;

namespace Trs80.Level1Basic
{
    public class BasicProgram : IBasicProgram
    {
        public List<ProgramStatement> Statements { get; set; } 
        private bool _programTerminated = true;
        private short _minLineNumber = short.MaxValue;
        private short _maxLineNumber = short.MinValue;

        public BasicProgram()
        {
            Statements = new List<ProgramStatement>();
        }
        public void List(ITrs80Console console)
        {
            foreach (var statement in Statements) 
                console.WriteLine(statement.OriginalText);
        }


        public void Run()
        {
            _programTerminated = false;
            ExpressionList.Initialize();

            var statement = Statements.FirstOrDefault();
            while (statement != null)
            {
                SetNextStatement(statement);
                statement.Execute();
                statement = GetNextStatement();
            } 
        }

        private int _nextIndex;
        private ProgramStatement GetNextStatement()
        {
            if (_programTerminated || _nextIndex < 0) return null;

            return Statements[_nextIndex];
        }

        public void SetNextStatement(short lineNumber)
        {
            var statement = Statements.FirstOrDefault(l => l.LineNumber == lineNumber);
            if (statement == null)
            {
                Console.WriteLine("SORRY");
                _programTerminated = true;
            } 
            else
                _nextIndex = Statements.IndexOf(statement);
        }

        public short GetSuccessor(short lineNumber)
        {
            var statement = Statements.FirstOrDefault(l => l.LineNumber > lineNumber);
            return statement?.LineNumber ?? (short)-1;
        }
        private void SetNextStatement(ProgramStatement predecessor)
        {
            var statement = Statements.FirstOrDefault(l => l.LineNumber > predecessor.LineNumber);
            if (statement == null) 
                _nextIndex = -1;
            else
                _nextIndex = Statements.IndexOf(statement);
        }

        public void New()
        {
            Statements.Clear();
        }

        public void End()
        {
            _programTerminated = true;
        }

        public void AddStatement(ProgramStatement statement)
        {
            DeleteStatement(statement.LineNumber);
            if (Statements.Count == 0)
                InsertFirstStatement(statement);
            else if (statement.LineNumber < _minLineNumber)
                InsertStatementAtBeginning(statement);
            else if (statement.LineNumber > _maxLineNumber)
                InsertStatementAtEnd(statement);
            else
                InsertStatementInMiddle(statement);
        }
        
        private void InsertStatementInMiddle(ProgramStatement statement)
        {
            var successor = Statements.FirstOrDefault(l => l.LineNumber > statement.LineNumber);
            int index = Statements.IndexOf(successor);
            Statements.Insert(index, statement);
        }

        private void InsertStatementAtEnd(ProgramStatement statement)
        {
            Statements.Add(statement);
            _maxLineNumber = statement.LineNumber;
        }

        private void InsertStatementAtBeginning(ProgramStatement statement)
        {
            Statements.Insert(0, statement);
            _minLineNumber = statement.LineNumber;
        }

        private void InsertFirstStatement(ProgramStatement statement)
        {
            Statements.Add(statement);
            _minLineNumber = statement.LineNumber;
            _maxLineNumber = statement.LineNumber;
        }

        public void DeleteStatement(short lineNumber)
        {
            var statement = Statements.FirstOrDefault(l => l.LineNumber == lineNumber);
            if (statement == null) return;

            Statements.Remove(statement);

            if (Statements.Count == 0)
            {
                _minLineNumber = short.MaxValue;
                _maxLineNumber = short.MinValue;
            }
            else
            {
                if (statement.LineNumber == _minLineNumber)
                    _minLineNumber = Statements.First().LineNumber;
                if (statement.LineNumber == _maxLineNumber)
                    _maxLineNumber = Statements.Last().LineNumber;
            }
        }
    }
}
