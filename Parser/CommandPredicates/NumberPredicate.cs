namespace Trs80.Level1Basic.Parser.CommandPredicates
{
    public class NumberPredicate : ICommandPredicate
    {
        public NumberPredicate(long value)
        {
            Value = value;
        }
        public long Value { get; set; }
    }
}