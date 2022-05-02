using Trs80.Level1Basic.Parser.Statements;

namespace Trs80.Level1Basic
{
    public class ProgramStatement
    {
        public short LineNumber { get; set; }
        public string OriginalText { get; set; }
        public IStatement Statement { get; set; }

        public void Execute()
        {
            Statement.Execute();
        }
    }
}
