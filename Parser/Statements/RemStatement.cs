namespace Trs80.Level1Basic.Parser.Statements
{
    public class RemStatement : IStatement
    {
        public RemStatement(string remark)
        {
            Remark = remark;
        }
        public string Remark { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + Remark;
        }

        public void Execute()
        {
            // do nothing
        }
    }
}