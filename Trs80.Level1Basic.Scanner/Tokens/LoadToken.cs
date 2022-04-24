namespace Trs80.Level1Basic.Scanner.Tokens
{
    public class LoadToken : IKeywordToken
    {
        public string FileName { get; set; }
        public LoadToken(string fileName)
        {
            FileName = fileName;
        }
    }
}