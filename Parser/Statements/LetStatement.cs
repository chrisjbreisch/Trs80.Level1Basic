using System;
using Trs80.Level1Basic.Parser.Expressions;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class LetStatement : IStatement
    {
        private readonly AssignmentExpression _assignment;
        public LetStatement(AssignmentExpression assignment)
        {
            _assignment = assignment ?? throw new ArgumentNullException(nameof(assignment));
        }

        public void Execute()
        {
            _assignment.Execute();
        }
    }
}