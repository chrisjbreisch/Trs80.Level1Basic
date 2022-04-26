using System;

namespace Trs80.Level1Basic.Services.Interpreter
{
    public class BasicFunctionImplementations
    {
        public int Int(dynamic value)
        {
            return (int)Math.Floor((float)value);
        }

        public string Tab(IBasicInterpreter interpreter, dynamic value)
        {
            interpreter.WriteToPosition(value);

            return string.Empty;
        }

        public dynamic Mem()
        {
            return int.MaxValue;
        }

        public dynamic Abs(dynamic value)
        {
            if (value is float fValue)
                return Math.Abs(fValue);
            return Math.Abs((int) value);
        }

        private static readonly Random Rand = new();
        public dynamic Rnd(int control)
        {
            if (control == 0)
                return (float) Rand.NextDouble();

            return (int) Math.Floor(control * Rand.NextDouble() + 1);
        }

        public string PadQuadrant(IBasicInterpreter interpreter)
        {
            return interpreter.PadQuadrant();
        }

        public object Set(IBasicInterpreter interpreter, int x, int y)
        {
            interpreter.Set(x, y);
            return null;
        }

        public object Reset(IBasicInterpreter interpreter, int x, int y)
        {
            interpreter.Reset(x, y);
            return null;

        }

        //Dictionary<int, dynamic> _theArray = new Dictionary<int, dynamic>();
        //public dynamic TheArray(int index)
        //{
        //    throw new NotImplementedException();
        //}

        public int Point(IBasicInterpreter interpreter, int x, int y)
        {
            return interpreter.Point(x, y) ? 1 : 0;
        }
    }
}
