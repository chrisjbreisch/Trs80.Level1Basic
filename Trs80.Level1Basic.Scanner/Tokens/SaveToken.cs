namespace Trs80.Level1Basic.Scanner.Tokens
{
    public class SaveToken : IKeywordToken
    {
        public string FileName { get; set; }
        public SaveToken(string fileName)
        {
            FileName = fileName;
        }
    }
}