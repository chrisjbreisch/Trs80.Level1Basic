namespace Trs80.Level1Basic.Common
{
    public class SourceLine
    {
        public SourceLine()
        {

        }

        public SourceLine(string original)
        {
            Original = original;
            Line = original.ToUpperInvariant();
        }
        public string Original { get; set; }
        public string Line { get; set; }
    }
}
