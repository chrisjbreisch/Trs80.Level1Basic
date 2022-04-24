namespace Trs80.Level1Basic.Parser.Expressions
{
    public interface IExpression
    {
        IExpression Left { get; set; }
        IExpression Right { get; set; }
        dynamic Value { get; }
        void Initialize();
    }
}
