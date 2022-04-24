namespace Trs80.Level1Basic.Parser.Expressions
{
    public class NullExpression : IExpression
    {
        public IExpression Left
        {
            get => null;
            set { /* do nothing */}
        }
        public IExpression Right 
        {
            get => null;
            set { /* do nothing */}
        }
        public object Value
        {
            get => null;
            set { /* do nothing */}
        }

        public void Initialize()
        {
            // do nothing
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}