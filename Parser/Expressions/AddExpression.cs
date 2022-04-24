using System;

namespace Trs80.Level1Basic.Parser.Expressions
{
    public class AddExpression : Expression
    {
        public AddExpression(IExpression left, IExpression right) : base(left, right)
        {
        }

        public static ILeafExpression CreateInstanceOf(Type t)
        {
            var literalType = typeof(LiteralExpression<>);

            var constructorClass = literalType.MakeGenericType(t);

            return (ILeafExpression)Activator.CreateInstance(constructorClass);
        }

        protected override dynamic Evaluate()
        {
            return Left.Value + Right.Value;
            //var answer = Left.Value + Right.Value;
            //ILeafExpression lit = CreateInstanceOf(answer.GetType());

            //lit.Value = answer;
            
            ////if (!IsCalculated)
            //    Value = Left.Value + Right.Value; ;

            //return Value;
        }
    }
}