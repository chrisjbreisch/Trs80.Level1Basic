using Trs80.Level1Basic.Parser.Statements;

namespace Trs80.Level1Basic.Parser.CommandPredicates
{
    public class StatementPredicate : ICommandPredicate
    {
        public StatementPredicate(IStatement value)
        {
            Value = value;
        }
        public IStatement Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}