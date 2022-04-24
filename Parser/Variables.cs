using System.Collections.Generic;

namespace Trs80.Level1Basic.Parser
{
    public class Variables : IVariables
    {
        private readonly Dictionary<string, object> _variableList = new Dictionary<string, object>();

        public dynamic SetValue(string name, dynamic value)
        {
            string lowerName = name.ToLower();
            if (_variableList.ContainsKey(lowerName))
                _variableList[lowerName] = value;
            else
                _variableList.Add(lowerName, value);

            return value;
        }

        public dynamic GetValue(string name)
        {
            string lowerName = name.ToLower();
            int value = 0;
            if (lowerName == "mem")
                value = int.MaxValue;
            if (!_variableList.ContainsKey(lowerName))
                SetValue(name, value);

            return _variableList[lowerName];
        }
    }
}
