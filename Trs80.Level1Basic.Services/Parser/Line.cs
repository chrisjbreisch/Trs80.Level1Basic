using System.Collections.Generic;
using Trs80.Level1Basic.Services.Parser.Statements;

namespace Trs80.Level1Basic.Services.Parser
{
    public class Line
    {
        public int LineNumber { get; set; }
        public string SourceLine { get; set; }

        public List<Statement> Statements { get; set; }
    }
}
