using System.Collections.Generic;
using Trs80.Level1Basic.Scanner.Tokens;

namespace Trs80.Level1Basic.Scanner
{
    public class ScannedStatement
    {
        public string OriginalText { get; set; }
        public List<IToken> Tokens { get; set; }
    }
}
