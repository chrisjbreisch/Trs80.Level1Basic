using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.VirtualMachine.Interpreter;

public class Environment
{
    public enum VariableType
    {
        Integer,
        Single,
        Double,
        String
    }

    private readonly Dictionary<string, dynamic> _variables = new();
    private readonly Dictionary<string, Dictionary<int, dynamic>> _arrays = new();
    private readonly Dictionary<string, VariableType> _declaredTypes = new();
    private const string names = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public Environment()
    {
        foreach (char name in names)
        {
            Define(name.ToString(), 0);
            Define($"{name}$", "");
            DefineArray(name.ToString());
            _declaredTypes[name.ToString()] = VariableType.Single;
            _declaredTypes[$"{name}$"] = VariableType.String;
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

    public void SetVariableType(string name, TokenType type)
    {
        string normalizedName = NormalizeName(name);
        if (string.IsNullOrEmpty(normalizedName)) return;

        VariableType variableType = type switch
        {
            TokenType.DefInt => VariableType.Integer,
            TokenType.DefSng => VariableType.Single,
            TokenType.DefDbl => VariableType.Double,
            TokenType.DefStr => VariableType.String,
            _ => _declaredTypes.TryGetValue(normalizedName, out VariableType existing) ? existing : VariableType.Single
        };

        _declaredTypes[normalizedName] = variableType;

        if (normalizedName.Length == 1 && variableType == VariableType.String)
        {
            _declaredTypes[$"{normalizedName}$"] = VariableType.String;
            _variables[$"{normalizedName}$"] = _variables.TryGetValue(normalizedName, out dynamic current) ? current : "";
            _variables[normalizedName] = _variables[$"{normalizedName}$"];
            return;
        }

        if (normalizedName.Length == 1 && variableType != VariableType.String)
            _declaredTypes[$"{normalizedName}$"] = VariableType.String;
    }

    public bool IsStringVariable(string name)
    {
        return GetDeclaredType(name) == VariableType.String;
    }

    internal dynamic Set(string name, dynamic value)
    {
        string normalizedName = NormalizeName(name);
        if (string.IsNullOrEmpty(normalizedName)) return value;

        VariableType targetType = GetDeclaredType(normalizedName);
        value = CastValue(value, targetType);

        _variables[normalizedName] = value;

        if (normalizedName.Length == 1 && targetType == VariableType.String)
            _variables[$"{normalizedName}$"] = value;

        return value;
    }

    private dynamic CastValue(dynamic value, VariableType targetType)
    {
        switch (targetType)
        {
            case VariableType.Integer:
                if (value is null) return 0;
                if (value is string text)
                {
                    if (int.TryParse(text, out int parsedInt)) return parsedInt;
                    if (float.TryParse(text, out float parsedFloatFromString)) return (int)parsedFloatFromString;
                    return 0;
                }
                if (value is float floatValueInt) return (int)floatValueInt;
                if (value is double doubleValueInt) return (int)doubleValueInt;
                if (value is bool boolValueInt) return boolValueInt ? 1 : 0;
                return (int)value;
            case VariableType.Single:
                if (value is null) return 0f;
                if (value is string textValue)
                {
                    if (float.TryParse(textValue, out float parsedFloat)) return parsedFloat;
                    if (int.TryParse(textValue, out int parsedIntFromString)) return parsedIntFromString;
                    return 0f;
                }
                if (value is double doubleValueSingle) return (float)doubleValueSingle;
                if (value is int intValueSingle) return intValueSingle;
                return value;
            case VariableType.Double:
                if (value is null) return 0d;
                if (value is string textDouble)
                {
                    if (double.TryParse(textDouble, out double parsedDouble)) return parsedDouble;
                    if (float.TryParse(textDouble, out float parsedFloatFromDouble)) return parsedFloatFromDouble;
                    return 0d;
                }
                if (value is float floatValueDouble) return (double)floatValueDouble;
                if (value is int intValueDouble) return (double)intValueDouble;
                return value;
            case VariableType.String:
                return value is null ? string.Empty : value.ToString();
            default:
                return value;
        }
    }

    private VariableType GetDeclaredType(string name)
    {
        string normalizedName = NormalizeName(name);
        if (normalizedName.EndsWith('$'))
            return VariableType.String;

        if (_declaredTypes.TryGetValue(normalizedName, out VariableType declaredType))
            return declaredType;

        if (_declaredTypes.TryGetValue($"{normalizedName}$", out VariableType stringDeclaredType))
            return stringDeclaredType;

        return VariableType.Single;
    }

    private dynamic ValidateValue(dynamic value, bool isString)
    {
        if (isString) return value;

        if (value is string)
            return 0;

        return value switch
        {
            int => value,
            float => value,
            _ => value
        };
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
        string normalizedName = NormalizeName(name);
        if (string.IsNullOrEmpty(normalizedName)) return null;

        if (normalizedName.EndsWith('$'))
            return _variables.TryGetValue(normalizedName, out dynamic stringValue) ? stringValue : string.Empty;

        if (_declaredTypes.TryGetValue(normalizedName, out VariableType declaredType) && declaredType == VariableType.String)
            return _variables.TryGetValue($"{normalizedName}$", out dynamic aliasValue) ? aliasValue : _variables[normalizedName];

        return _variables.TryGetValue(normalizedName, out dynamic value) ? value : 0;
    }

    private string NormalizeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;
        if (name.Length == 2 && name[1] == '$') return name.ToUpperInvariant();
        if (name.Length == 1) return name.ToUpperInvariant();
        return name.ToUpperInvariant();
    }

    public void InitializeVariables()
    {
        foreach (string name in _arrays.Keys)
        {
            _arrays[name] = new Dictionary<int, dynamic>();
            Set(name, 0);
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