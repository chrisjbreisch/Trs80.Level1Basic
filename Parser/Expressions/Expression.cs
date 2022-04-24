using System;

namespace Trs80.Level1Basic.Parser.Expressions
{
    
    public abstract class Expression : IExpression
    {

        //protected bool IsCalculated;

        protected Expression(IExpression left, IExpression right)
        {
            Left = left ?? new NullExpression();
            Right = right ?? new NullExpression();
            ExpressionList.AddExpression(this);
        }
        public IExpression Left { get; set; }
        public IExpression Right { get; set; }
        protected object CalculatedValue;
        protected abstract dynamic Evaluate();

        protected Type GetPromotedType(Type left, Type right)
        {
            if (left.FullName == "System.Double" || right.FullName == "System.Double") return typeof(double);
            if (left.FullName == "System.Float" || right.FullName == "System.Float") return typeof(float);
            if (left.FullName == "System.Uint64" || right.FullName == "System.Uint64") return typeof(ulong);
            if (left.FullName == "System.Int64" || right.FullName == "System.Int64") return typeof(long);
            if (left.FullName == "System.Uint32" && (right.FullName == "System.Sbyte" || right.FullName == "System.Int16" || right.FullName == "System.Int32")) return typeof(long);
            if (right.FullName == "System.Uint32" && (left.FullName == "System.Sbyte" || left.FullName == "System.Int16" || left.FullName == "System.Int32")) return typeof(long);
            if (left.FullName == "System.Uint32" || right.FullName == "System.Uint32") return typeof(uint);
            return typeof(long);
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        private dynamic _value;
        public dynamic Value
        {
            get
            {
                _value = Evaluate();
                return _value;
            }
            set => _value = value;
        }
        //{
        //    get
        //    {
        //        //if (!IsCalculated)
        //        {
        //            CalculatedValue = Evaluate();
        //            //IsCalculated = true;
        //        }
        //        return CalculatedValue;
        //    }
        //    set
        //    {
        //        CalculatedValue = value;
        //        //IsCalculated = true;
        //    }
        //}
        public void Initialize()
        {
            //IsCalculated = false;
        }
    }
}
