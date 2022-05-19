using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements
{
    public class StatementList : Statement, IEnumerable<IStatement>
    {
        private readonly List<IListStatementDecorator> _statements = new();

        private IListStatementDecorator Decorate(IStatement statement)
        {
            return new ListStatementDecorator(statement);
        }

        public StatementList AddEnd(IStatement statement)
        {
            _statements.Add(Decorate(statement));
            return this;
        }

        public StatementList Insert(IStatement statement)
        {
            IListStatementDecorator original = _statements.FirstOrDefault(s => s.LineNumber == statement.LineNumber);
            if (original != null)
                return Replace(statement);

            IListStatementDecorator decorated = Decorate(statement);

            IListStatementDecorator predecessor = _statements.LastOrDefault(s => s.LineNumber < statement.LineNumber);
            if (predecessor != null)
            {
                predecessor.Next = decorated;
                int index = _statements.IndexOf(predecessor);
                _statements.Insert(Math.Min(index + 1, _statements.Count), decorated);
            }

            IListStatementDecorator successor = _statements.FirstOrDefault(s => s.LineNumber > statement.LineNumber);

            if (successor != null)
            {
                decorated.Next = successor;
                if (predecessor == null)
                {
                    int index = _statements.IndexOf(successor);
                    _statements.Insert(index, decorated);
                }
            }

            decorated.Parent = this;
            return this;
        }

        private StatementList Replace(IStatement statement)
        {
            IListStatementDecorator original = _statements.FirstOrDefault(s => s.LineNumber == statement.LineNumber);

            if (original == null)
                return Insert(statement);

            IListStatementDecorator decorated = Decorate(statement);
            int index = _statements.IndexOf(original);
            _statements.Insert(index, decorated);

            if (original.Previous is IListStatementDecorator predecessor)
                predecessor.Next = decorated;

            if (original.Next is IListStatementDecorator successor)
                decorated.Next = successor;

            decorated.Parent = this;

            return Remove(original);
        }

        public StatementList AddOrReplace(IStatement statement)
        {
            IListStatementDecorator original = _statements.FirstOrDefault(s => s.LineNumber == statement.LineNumber);

            if (original == null)
                Insert(statement);
            else
                Replace(statement);

            return this;
        }

        public StatementList Remove(IStatement statement)
        {
            IListStatementDecorator original = _statements.FirstOrDefault(s => s.LineNumber == statement.LineNumber);
            if (original == null) return this;

            _statements.Remove(original);

            original.Next = null;
            original.Previous = null;
            original.Parent = null;

            return this;
        }

        public IStatement this[int index]
        {
            get { return _statements[index]; }
            set { AddOrReplace(value); }
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IStatement> GetEnumerator()
        {
            return _statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _statements.GetEnumerator();
        }
    }
}
