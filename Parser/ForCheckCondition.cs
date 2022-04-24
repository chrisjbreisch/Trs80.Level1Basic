using Trs80.Level1Basic.Parser.Expressions;

namespace Trs80.Level1Basic.Parser
{
    public class ForCheckCondition
    {
        public string VariableName { get; set; }
        public short LineNumber { get; set; }
        public IExpression Step { get; set; }
        public IExpression StartValue { get; set; }
        public IExpression EndValue { get; set; }
    }
}
