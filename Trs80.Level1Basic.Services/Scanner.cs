using System;
using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.Domain;
using Trs80.Level1Basic.Exceptions;
using Trs80.Level1Basic.Services.Interpreter;

namespace Trs80.Level1Basic.Services;

public interface IScanner
{
    List<Token> ScanTokens(string source);
}

public class Scanner : IScanner
{
    private string _source;
    private List<Token> _tokens;
    private int TokenStart { get; set; }
    private int TokenLength => _currentIndex - TokenStart;
    private int _currentIndex;
    private readonly IBuiltinFunctions _builtins;

    private static readonly Dictionary<int, Dictionary<string, TokenType>> KeywordsByLetter =
        CreateKeywordsByLetterDictionary();
    private string _currentLine;

    public Scanner(IBuiltinFunctions builtins)
    {
        _builtins = builtins ?? throw new ArgumentNullException(nameof(builtins));
    }
    private string CurrentLine
    {
        get
        {
            if (_currentLine == null)
                GetCurrentLine();
            return _currentLine;
        }
    }

    private void GetCurrentLine()
    {
        string currentString = _source[TokenStart..];
        int endOfLine = currentString.IndexOf("\r\n", StringComparison.Ordinal);
        _currentLine = endOfLine < 0 ? currentString : currentString[..endOfLine];
    }

    private static Dictionary<int, Dictionary<string, TokenType>> CreateKeywordsByLetterDictionary()
    {
        return new Dictionary<int, Dictionary<string, TokenType>>
        {
            {
                2, new Dictionary<string, TokenType>
                {
                    {"at", TokenType.At},
                    {"d.", TokenType.Data},
                    {"e.", TokenType.End},
                    {"f.", TokenType.For},
                    {"g.", TokenType.Goto},
                    {"if", TokenType.If},
                    {"l.", TokenType.List},
                    {"n.", TokenType.N},
                    {"on", TokenType.On},
                    {"p.", TokenType.Print},
                    {"r.", TokenType.Run},
                    {"s.", TokenType.Step},
                    {"t.", TokenType.T},
                    {"to", TokenType.To},
                   // {"t.", TokenType.T},
                }
            },
            {
                3, new Dictionary<string, TokenType>
                {
                    {"cls", TokenType.Cls},
                    {"end", TokenType.End},
                    {"for", TokenType.For},
                    {"in.", TokenType.Input},
                    {"let", TokenType.Let},
                    {"new", TokenType.New},
                    {"rem", TokenType.Rem},
                    {"run", TokenType.Run},
                    {"st.", TokenType.Stop},
                    {"tab", TokenType.Tab},
                }
            },
            {
                4, new Dictionary<string, TokenType>
                {
                    {"cont", TokenType.Cont},
                    {"data", TokenType.Data},
                    {"gos.", TokenType.Gosub},
                    {"goto", TokenType.Goto},
                    {"list", TokenType.List},
                    {"load", TokenType.Load},
                    {"next", TokenType.Next},
                    {"read", TokenType.Read},
                    {"rea.", TokenType.Read},
                    {"ret.", TokenType.Return},
                    {"save", TokenType.Save},
                    {"step", TokenType.Step},
                    {"stop", TokenType.Stop},
                    {"then", TokenType.Then},
                }
            },
            {
                5, new Dictionary<string, TokenType>
                {
                    {"gosub", TokenType.Gosub},
                    {"input", TokenType.Input},
                    {"merge", TokenType.Merge},
                    {"print", TokenType.Print},
                    {"rest.", TokenType.Restore},
                }
            },
            {
                6, new Dictionary<string, TokenType>
                {
                    {"return", TokenType.Return},
                }
            },
            {
                7, new Dictionary<string, TokenType>
                {
                    {"restore", TokenType.Restore},
                }
            }
        };
    }

    public List<Token> ScanTokens(string source)
    {
        Initialize();

        _source = source;

        while (!IsAtEnd())
        {
            TokenStart = _currentIndex;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EndOfLine, "", null, CurrentLine));
        return _tokens;
    }

    private void Initialize()
    {
        _source = null;
        _currentLine = null;
        _tokens = new List<Token>();
        TokenStart = 0;
        _currentIndex = 0;
    }

    private bool IsAtEnd()
    {
        return _currentIndex >= _source.Length;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case '(':
                AddToken(TokenType.LeftParen);
                break;
            case ')':
                AddToken(TokenType.RightParen);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case '-':
                AddToken(TokenType.Minus);
                break;
            case '+':
                AddToken(TokenType.Plus);
                break;
            case ';':
                AddToken(TokenType.Semicolon);
                break;
            case ':':
                AddToken(TokenType.Colon);
                break;
            case '*':
                AddToken(TokenType.Star);
                break;
            case '/':
                AddToken(TokenType.Slash);
                break;
            case '=':
                if (Match('>'))
                    AddToken(TokenType.GreaterThanOrEqual);
                else if (Match('<'))
                    AddToken(TokenType.LessThanOrEqual);
                else
                    AddToken(TokenType.Equal);
                break;
            case '"':
                GetString();
                break;
            case '<':
                if (Match('='))
                    AddToken(TokenType.LessThanOrEqual);
                else if (Match('>'))
                    AddToken(TokenType.NotEqual);
                else
                    AddToken(TokenType.LessThan);
                break;
            case '>':
                if (Match('='))
                    AddToken(TokenType.GreaterThanOrEqual);
                else if (Match('<'))
                    AddToken(TokenType.NotEqual);
                else
                    AddToken(TokenType.GreaterThan);
                break;
            case ' ':
            case '\t':
                break;
            case '\r':
                if (Match('\n'))
                    _currentLine = null;
                break;
            default:
                if (IsDigit(c) || c == '.')
                    GetNumber(c);
                else if (IsAlpha(c))
                    GetKeywordOrIdentifier();
                else
                    throw new ScanException("Unexpected character.");
                break;
        }
    }

    private bool IsAlpha(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }

    private void GetKeywordOrIdentifier()
    {
        if (Peek() == '$')
            AddStringIdentifierToken();
        else if (IsAlpha(Peek()) || Peek() == '.')
            Add2PlusCharsToken();
        else
            AddIdentifierToken();
    }

    private void AddIdentifierToken()
    {
        string id = _source.Substring(TokenStart, 1).ToLower();
        AddToken(TokenType.Identifier, id);
    }

    private void Add2PlusCharsToken()
    {
        Advance();
        try
        {
            AddKeywordToken();
        }
        catch
        {
            Add3PlusCharsToken();
        }
    }

    private void Add3PlusCharsToken()
    {
        if (AtIdentifierEnd())
            AddUnknownIdentifierToken();
        else
        {
            Advance();
            try
            {
                Add3CharToken();
            }
            catch
            {
                Add4PlusCharsToken();
            }
        }
    }

    private void Add4PlusCharsToken()
    {
        if (AtIdentifierEnd())
            AddUnknownIdentifierToken();
        else
        {
            Advance();
            try
            {
                Add4CharToken();
            }
            catch
            {
                Add5PlusCharsToken();
            }
        }
    }

    private void Add5PlusCharsToken()
    {
        if (AtIdentifierEnd())
            AddUnknownIdentifierToken();
        else
        {
            Advance();
            try
            {
                Add5CharToken();
            }
            catch
            {
                Add6PlusCharsToken();
            }
        }
    }

    private bool AtIdentifierEnd()
    {
        return IsAtEnd() || (!IsAlpha(Peek()) && Peek() != '.');
    }

    private void AddUnknownIdentifierToken()
    {
        string identifier = _source.Substring(TokenStart, TokenLength);

        if (TokenLength > 1 && Peek() == '$')
        {
            Advance();
            identifier = _source.Substring(TokenStart, TokenLength);
        }

        if (TokenLength == 1 || _builtins.Get(identifier) != null)
            AddToken(TokenType.Identifier, identifier);
        else if (!KeywordsByLetter
                     .Select(d => d.Value)
                     .Any(kw => kw.ContainsKey(identifier)))
            AddToken(TokenType.String, identifier);
            //throw new ScanException(
            //    _source.Substring(0, TokenStart + 1) + "?" +
            //    _source.Substring(TokenStart + 1, _source.Length - TokenStart - 1));
    }

    private void Add6PlusCharsToken()
    {
        if (AtIdentifierEnd())
            AddUnknownIdentifierToken();
        else
        {
            Advance();
            try
            {
                Add6CharToken();
            }
            catch
            {
                Add7CharToken();
            }
        }
    }

    private void AddKeywordToken()
    {
        var keyword = GetKeywordAtPosition();
        if (keyword == TokenType.Backup) return;

        AddToken(keyword);
    }

    private void Add3CharToken()
    {
        var keyword = GetKeywordAtPosition();
        if (keyword == TokenType.Backup) return;

        switch (keyword)
        {
            case TokenType.Rem:
                CreateRemarkToken(keyword);
                break;
            default:
                AddToken(keyword);
                break;
        }
    }

    private void Add4CharToken()
    {
        var keyword = GetKeywordAtPosition();
        if (keyword == TokenType.Backup) return;

        switch (keyword)
        {
            case TokenType.Data:
                CreateDataTokens(keyword);
                break;
            default:
                AddToken(keyword);
                break;
        }
    }

    private void Add5CharToken()
    {
        AddKeywordToken();
    }

    private void Add6CharToken()
    {
        AddKeywordToken();
    }

    private void Add7CharToken()
    {
        if (AtIdentifierEnd())
            AddUnknownIdentifierToken();
        else
        {
            Advance();
            try
            {
                AddKeywordToken();
            }
            catch
            {
                // try backing up
                AddToken(TokenType.Identifier, _source.Substring(TokenStart, 1));
                _currentIndex = TokenStart + 1;
                GetKeywordOrIdentifier();
            }
        }
    }

    private TokenType GetKeywordAtPosition()
    {
        string key = _source.Substring(TokenStart, TokenLength).ToLower();
        try
        {
            var keyword = KeywordsByLetter[TokenLength][key];
            return keyword;
        }
        catch
        {
            if (IsAlpha(Peek())) throw;

            // try backing up
            key = _source.Substring(TokenStart + 1, TokenLength - 1).ToLower();
            if (!KeywordsByLetter[TokenLength - 1].ContainsKey(key)) throw;

            AddToken(TokenType.Identifier, _source.Substring(TokenStart, 1));
            _currentIndex = TokenStart + 1;
            return TokenType.Backup;
        }
    }

    private void AddStringIdentifierToken()
    {
        Advance();
        string id = _source.Substring(TokenStart, 2).ToLower();
        AddToken(TokenType.Identifier, id);
    }

    private void CreateDataTokens(TokenType keyword)
    {
        AddToken(keyword);
        TokenStart = ++_currentIndex;
        while (!IsAtEnd())
        {
            while (Peek() != ',' && !IsAtEnd())
                Advance();
            string element = _source.Substring(TokenStart, TokenLength);
            if (int.TryParse(element, out int intValue))
                AddToken(TokenType.Number, intValue);
            else if (float.TryParse(element, out float floatValue))
                AddToken(TokenType.Number, floatValue);
            else
                AddToken(TokenType.String, element);
            if (!IsAtEnd())
            {
                AddToken(TokenType.Comma);
                while (Peek() == ',' || Peek() == ' ' || Peek() == '\t')
                    Advance();
            }

            TokenStart = _currentIndex;
        }
    }

    private void CreateRemarkToken(TokenType keyword)
    {
        while (Peek() != '\r' && !IsAtEnd())
            Advance();
        if (!IsAtEnd() && Peek() == '\n')
            Advance();
        string remark = _source.Substring(TokenStart + 4, TokenLength - 4);
        AddToken(keyword, remark);
    }

    private void GetNumber(char c)
    {
        bool isInt = c != '.';

        while (IsDigit(Peek()))
            Advance();

        if (Peek() == '.' && IsDigit(PeekNext()) && isInt)
        {
            isInt = false;
            Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        if ((Peek() == 'E' || Peek() == 'e') &&
            (IsDigit(PeekNext()) || PeekNext() == '+' || PeekNext() == '-'))
        {
            isInt = false;
            Advance();
            if (!IsDigit(Peek()))
                Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        object value;
        if (isInt)
            value = int.Parse(_source.Substring(TokenStart, TokenLength));
        else
            value = float.Parse(_source.Substring(TokenStart, TokenLength));

        AddToken(TokenType.Number, value);
    }

    private char PeekNext()
    {
        return _currentIndex + 1 >= _source.Length ? '\0' : _source[_currentIndex + 1];
    }

    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private void GetString()
    {
        while (Peek() != '"' && !IsAtEnd())
            Advance();

        if (IsAtEnd())
            throw new ScanException("Unterminated string.");

        Advance();

        string value = _source.Substring(TokenStart + 1, TokenLength - 2);
        AddToken(TokenType.String, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_currentIndex] != expected) return false;

        _currentIndex++;
        return true;
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_currentIndex];
    }

    private char Advance()
    {
        return _source[_currentIndex++];
    }

    private void AddToken(TokenType type, dynamic literal = null)
    {
        string text = _source.Substring(TokenStart, TokenLength);
        _tokens.Add(new Token(type, text, literal, CurrentLine));
    }
}