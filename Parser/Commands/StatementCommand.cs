namespace Trs80.Level1Basic.Parser.Commands
{
    public class StatementCommand : IStatementCommand
    {
        public StatementCommand(short lineNumber)
        {
            LineNumber = lineNumber;
        }
        public short LineNumber { get; set; }

        public override string ToString()
        {
            return base.ToString() + " " + LineNumber;
        }
    }
}