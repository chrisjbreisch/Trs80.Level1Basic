using System.Collections.Generic;
using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Environment
{
    private readonly Dictionary<string, dynamic> _variables = new();
    private readonly Dictionary<string, Dictionary<int, dynamic>> _arrays = new();
    private const string names = "abcdefghijklmnopqrstuvwxyz";

    public Environment()
    {
        foreach (char name in names)
        {
            Define(name.ToString(), 0);
            Define($"{name}$", "");
            DefineArray(name.ToString());
        }
    }

    private bool IsString(string name)
    {
        return name.EndsWith('$');
    }

    private void Define(string name, dynamic value)
    {
        _variables.Add(name, value);
    }

    internal dynamic Set(string name, dynamic value)
    {
        value = ValidateValue(value, IsString(name));

        _variables[name] = value;

        return value;
    }

    private dynamic ValidateValue(dynamic value, bool isString)
    {
        if (isString) return value;
            
        if (value is not string) return value;

        //if (value.Length == 1)
        //    return 0;

        //throw new ValueOutOfRangeException(0, "", null);
        return 0;
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

    internal dynamic Get(string name)
    {
        return _variables[name];
    }

    public void InitializeVariables()
    {
        foreach (string name in _arrays.Keys)
        {
            _arrays[name] = new Dictionary<int, dynamic>();
            Set(name.ToString(), 0);
            Set($"{name}$", "");
        }
    }

    public dynamic GetArrayValue(string name, int index)
    {
        Dictionary<int, dynamic> array = GetArray(name);

        if (!array.ContainsKey(index))
            array.Add(index, 0);

        return array[index];
    }
}