using System.Collections.Generic;
using Trs80.Level1Basic.Exceptions;

namespace Trs80.Level1Basic.Services.Interpreter
{
    public class GlobalVariables
    {
        private readonly Dictionary<string, dynamic> _variables = new Dictionary<string, dynamic>();
        private readonly Dictionary<string, Dictionary<int, dynamic>> _arrays = new Dictionary<string, Dictionary<int, dynamic>>();

        internal dynamic Define(string name, dynamic value)
        {
            _variables.Add(GetVariableName(name), value);
            return value;
        }

        private bool IsStringName(string name)
        {
            return name.EndsWith("$");
        }
        private string GetVariableName(string name)
        {
            if (IsStringName(name))
                return name.Substring(0, 1).ToLower() + "$";

            return name.ToLower().Substring(0, 1);
        }
        internal dynamic Assign(string name, dynamic value)
        {
            value = ValidateValue(name, value);

            var lowerName = GetVariableName(name);
            if (!_variables.ContainsKey(lowerName))
                Define(lowerName, value);
            else
                _variables[lowerName] = value;

            return value;
        }

        private dynamic ValidateValue(string name, dynamic value)
        {
            if (IsStringName(name)) return value;
            
            if (!(value is string)) return value;

            if (value.Length == 1)
                return 0;

            throw new ValueOutOfRangeException(0, null);
        }

        public dynamic AssignArray(string name, int index, dynamic value)
        {
            var array = GetArray(name);

            if (!array.ContainsKey(index))
                array.Add(index, value);
            else
                array[index] = value;

            return value;
        }

        private Dictionary<int, dynamic> GetArray(string name)
        {
            var lowerName = GetVariableName(name);
            if (!_arrays.ContainsKey(lowerName))
                DefineArray(lowerName);
            var array = _arrays[lowerName];
            return array;
        }

        private void DefineArray(string lowerName)
        {
            _arrays.Add(lowerName, new Dictionary<int, dynamic>());
        }

        internal bool Exists(string name)
        {
            var lowerName = GetVariableName(name);
            return _variables.ContainsKey(lowerName);
        }

        internal dynamic Get(string name)
        {
            var lowerName = GetVariableName(name);
            if (_variables.ContainsKey(lowerName)) return _variables[lowerName];

            return IsStringName(lowerName) ? Define(lowerName, "") : Define(lowerName, 0);
        }

        public void Clear()
        {
            _variables.Clear();
            _arrays.Clear();
        }

        public dynamic GetArrayValue(string name, int index)
        {
            var array = GetArray(name);

            if (!array.ContainsKey(index))
                array.Add(index, 0);

            return array[index];
        }
    }
}
