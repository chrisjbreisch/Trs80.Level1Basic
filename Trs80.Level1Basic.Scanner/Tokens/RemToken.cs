namespace Trs80.Level1Basic.Scanner.Tokens
{
    public class RemToken : IKeywordToken
    {
        public RemToken(string remark)
        {
            Remark = remark;
        }
        public string Remark { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + Remark;
        }
    }
}