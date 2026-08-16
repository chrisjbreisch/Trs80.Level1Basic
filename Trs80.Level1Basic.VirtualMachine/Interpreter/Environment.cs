using System;
using System.Collections.Generic;

using Trs80.Level1Basic.VirtualMachine.Exceptions;
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
    private readonly Dictionary<string, Dictionary<string, dynamic>> _matrixArrays = new();
    private readonly Dictionary<string, int[]> _arrayDimensions = new();
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
        EnsureArrayExists(normalizedName);

        if (normalizedName.Length == 1 && variableType == VariableType.String)
        {
            _declaredTypes[$"{normalizedName}$"] = VariableType.String;
            EnsureArrayExists($"{normalizedName}$");
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

    public void SetArrayDimensions(string name, int dimension1)
    {
        SetArrayDimensions(name, dimension1, null);
    }

    public void SetArrayDimensions(string name, int dimension1, int? dimension2)
    {
        if (dimension1 < 0 || dimension2 < -1)
            throw new ValueOutOfRangeException(-1, string.Empty, "Array dimension cannot be negative.");

        string normalizedName = NormalizeName(name);
        EnsureArrayExists(normalizedName);
        _arrayDimensions[normalizedName] = dimension2.HasValue
            ? new[] { dimension1, dimension2.Value }
            : new[] { dimension1 };
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
        if (normalizedName.EndsWith('$')) return VariableType.String;
        if (normalizedName.EndsWith('%')) return VariableType.Integer;
        if (normalizedName.EndsWith('!')) return VariableType.Single;
        if (normalizedName.EndsWith('#')) return VariableType.Double;

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
        string normalizedName = NormalizeName(name);
        ValidateArrayIndex(normalizedName, index);
        EnsureArrayExists(normalizedName);

        Dictionary<int, dynamic> array = GetArray(normalizedName);
        value = CastValue(value, GetDeclaredType(normalizedName));

        if (!array.ContainsKey(index))
            array.Add(index, value);
        else
            array[index] = value;

        return value;
    }

    public dynamic AssignArray(string name, int index, int index2, dynamic value)
    {
        string normalizedName = NormalizeName(name);
        ValidateArrayIndex(normalizedName, index, index2);
        EnsureArrayExists(normalizedName);

        string matrixKey = BuildMatrixKey(index, index2);
        Dictionary<string, dynamic> matrix = GetMatrixArray(normalizedName);
        value = CastValue(value, GetDeclaredType(normalizedName));

        if (!matrix.ContainsKey(matrixKey))
            matrix.Add(matrixKey, value);
        else
            matrix[matrixKey] = value;

        return value;
    }

    private Dictionary<int, dynamic> GetArray(string name)
    {
        string normalizedName = NormalizeName(name);
        return _arrays[normalizedName];
    }

    private Dictionary<string, dynamic> GetMatrixArray(string name)
    {
        string normalizedName = NormalizeName(name);
        if (!_matrixArrays.ContainsKey(normalizedName))
            _matrixArrays[normalizedName] = new Dictionary<string, dynamic>();

        return _matrixArrays[normalizedName];
    }

    private string BuildMatrixKey(int index, int index2)
    {
        return $"{index},{index2}";
    }

    private void DefineArray(string name)
    {
        string normalizedName = NormalizeName(name);
        if (!_arrays.ContainsKey(normalizedName))
            _arrays.Add(normalizedName, new Dictionary<int, dynamic>());
    }

    private void EnsureArrayExists(string name)
    {
        string normalizedName = NormalizeName(name);
        if (string.IsNullOrEmpty(normalizedName)) return;

        DefineArray(normalizedName);
        if (normalizedName.Length == 1)
            DefineArray($"{normalizedName}$");
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

           string normalizedName = name.ToUpperInvariant();
        char suffix = normalizedName[^1] is '$' or '%' or '!' or '#' ? normalizedName[^1] : '\0';
        string baseName = suffix == '\0' ? normalizedName : normalizedName[..^1];
           baseName = baseName[..Math.Min(2, baseName.Length)];

        return suffix == '\0' ? baseName : $"{baseName}{suffix}";
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
        string normalizedName = NormalizeName(name);
        ValidateArrayIndex(normalizedName, index);
        EnsureArrayExists(normalizedName);

        Dictionary<int, dynamic> array = GetArray(normalizedName);

        if (!array.ContainsKey(index))
            array.Add(index, DefaultArrayValue(normalizedName));

        return array[index];
    }

    public dynamic GetArrayValue(string name, int index, int index2)
    {
        string normalizedName = NormalizeName(name);
        ValidateArrayIndex(normalizedName, index, index2);
        EnsureArrayExists(normalizedName);

        string matrixKey = BuildMatrixKey(index, index2);
        Dictionary<string, dynamic> matrix = GetMatrixArray(normalizedName);

        if (!matrix.ContainsKey(matrixKey))
            matrix.Add(matrixKey, DefaultArrayValue(normalizedName));

        return matrix[matrixKey];
    }

    private dynamic DefaultArrayValue(string name)
    {
        return GetDeclaredType(name) == VariableType.String ? string.Empty : 0;
    }

    private void ValidateArrayIndex(string name, int index, int? index2 = null)
    {
        if (index < 0 || index2 is < 0)
            throw new ValueOutOfRangeException(-1, string.Empty, "Array subscript out of range.");

        if (!_arrayDimensions.TryGetValue(name, out int[] dimensions))
            return;

        if (index > dimensions[0] || index2.HasValue != (dimensions.Length == 2))
            throw new ValueOutOfRangeException(-1, string.Empty, "Array subscript out of range.");

        if (index2.HasValue && index2.Value > dimensions[1])
            throw new ValueOutOfRangeException(-1, string.Empty, "Array subscript out of range.");
    }
}