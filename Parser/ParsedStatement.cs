using Trs80.Level1Basic.Parser.CommandPredicates;
using Trs80.Level1Basic.Parser.Commands;

namespace Trs80.Level1Basic.Parser
{
    public class ParsedStatement
    {
        public string OriginalText { get; set; }
        public ICommand Command { get; set; }
        public ICommandPredicate Predicate { get; set; }

        public override string ToString()
        {
            return Command + " " + Predicate;
        }
    }
}
