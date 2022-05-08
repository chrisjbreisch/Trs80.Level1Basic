using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class GlobalVariables
{
    private readonly Dictionary<string, dynamic> _variables = new();
    private readonly Dictionary<string, Dictionary<int, dynamic>> _arrays = new();

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
            return name[..1].ToLower() + "$";

        return name.ToLower()[..1];
    }
    internal dynamic Assign(string name, dynamic value)
    {
        value = ValidateValue(name, value);

        string lowerName = GetVariableName(name);
        if (!_variables.ContainsKey(lowerName))
            Define(lowerName, value);
        else
            _variables[lowerName] = value;

        return value;
    }

    private dynamic ValidateValue(string name, dynamic value)
    {
        if (IsStringName(name)) return value;
            
        if (value is not string) return value;

        if (value.Length == 1)
            return 0;

        throw new ValueOutOfRangeException(0, "", null);
    }

    public dynamic AssignArray(string name, int index, dynamic value)
    {
        Dictionary<int, dynamic> array = GetArray(name);

        if (!array.ContainsKey(index))
            array.Add(index, value);
        else
            array[index] = value;

        return value;
    }

    private Dictionary<int, dynamic> GetArray(string name)
    {
        string lowerName = GetVariableName(name);
        if (!_arrays.ContainsKey(lowerName))
            DefineArray(lowerName);
        Dictionary<int, dynamic> array = _arrays[lowerName];
        return array;
    }

    private void DefineArray(string lowerName)
    {
        _arrays.Add(lowerName, new Dictionary<int, dynamic>());
    }

    internal bool Exists(string name)
    {
        string lowerName = GetVariableName(name);
        return _variables.ContainsKey(lowerName);
    }

    internal dynamic Get(string name)
    {
        string lowerName = GetVariableName(name);
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
        Dictionary<int, dynamic> array = GetArray(name);

        if (!array.ContainsKey(index))
            array.Add(index, 0);

        return array[index];
    }
}