using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Environment
{
    private readonly Dictionary<string, dynamic> _variables = new();
    private readonly Dictionary<string, Dictionary<int, dynamic>> _arrays = new();

    internal dynamic Define(string name, dynamic value)
    {
        _variables.Add(name, value);
        return value;
    }

    internal dynamic Assign(bool isString, string name, dynamic value)
    {
        value = ValidateValue(value, isString);

        if (!_variables.ContainsKey(name))
            Define(name, value);
        else
            _variables[name] = value;

        return value;
    }

    private dynamic ValidateValue(dynamic value, bool isString)
    {
        if (isString) return value;
            
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
        if (!_arrays.ContainsKey(name))
            DefineArray(name);
        Dictionary<int, dynamic> array = _arrays[name];
        return array;
    }

    private void DefineArray(string name)
    {
        _arrays.Add(name, new Dictionary<int, dynamic>());
    }

    internal bool Exists(string name)
    {
        return _variables.ContainsKey(name);
    }

    internal dynamic Get(bool isString, string name)
    {
        if (_variables.ContainsKey(name)) return _variables[name];

        return isString ? Define(name, "") : Define(name, 0);
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