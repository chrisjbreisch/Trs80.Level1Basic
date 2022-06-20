using System;
using System.Collections.Generic;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Machine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;

namespace Trs80.Level1Basic.VirtualMachine.Scanner;

public class Scanner : IScanner
{
    private string _source;
    private string _original;
    private List<Token> _tokens;
    private int TokenStart { get; set; }
    private int TokenLength => _currentIndex - TokenStart;
    private int _currentIndex;
    private readonly INativeFunctions _natives;

    private static readonly Dictionary<int, Dictionary<string, TokenType>> KeywordsByLetter =
        CreateKeywordsByLetterDictionary();
    private string _currentLine;

    public Scanner(INativeFunctions natives)
    {
        _natives = natives ?? throw new ArgumentNullException(nameof(natives));
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
                    {"AT", TokenType.At},
                    {"A.", TokenType.A},
                    {"C.", TokenType.Cont},
                    {"D.", TokenType.Data},
                    {"E.", TokenType.End},
                    {"F.", TokenType.For},
                    {"G.", TokenType.Goto},
                    {"IF", TokenType.If},
                    {"L.", TokenType.List},
                    {"N.", TokenType.N},
                    {"ON", TokenType.On},
                    {"P.", TokenType.Print},
                    {"R.", TokenType.R},
                    {"S.", TokenType.Step},
                    {"T.", TokenType.T},
                    {"TO", TokenType.To},
                }
            },
            {
                3, new Dictionary<string, TokenType>
                {
                    {"CLS", TokenType.Cls},
                    {"END", TokenType.End},
                    {"FOR", TokenType.For},
                    {"IN.", TokenType.Input},
                    {"LET", TokenType.Let},
                    {"NEW", TokenType.New},
                    {"REM", TokenType.Rem},
                    {"RUN", TokenType.Run},
                    {"ST.", TokenType.Stop},
                }
            },
            {
                4, new Dictionary<string, TokenType>
                {
                    {"CONT", TokenType.Cont},
                    {"DATA", TokenType.Data},
                    {"GOS.", TokenType.Gosub},
                    {"GOTO", TokenType.Goto},
                    {"LIST", TokenType.List},
                    {"LOAD", TokenType.Load},
                    {"NEXT", TokenType.Next},
                    {"READ", TokenType.Read},
                    {"REA.", TokenType.Read},
                    {"RET.", TokenType.Return},
                    {"SAVE", TokenType.Save},
                    {"STEP", TokenType.Step},
                    {"STOP", TokenType.Stop},
                    {"THEN", TokenType.Then},
                }
            },
            {
                5, new Dictionary<string, TokenType>
                {
                    {"GOSUB", TokenType.Gosub},
                    {"INPUT", TokenType.Input},
                    {"MERGE", TokenType.Merge},
                    {"PRINT", TokenType.Print},
                    {"REST.", TokenType.Restore},
                }
            },
            {
                6, new Dictionary<string, TokenType>
                {
                    {"RETURN", TokenType.Return},
                }
            },
            {
                7, new Dictionary<string, TokenType>
                {
                    {"RESTORE", TokenType.Restore},
                }
            }
        };
    }

    public List<Token> ScanTokens(SourceLine source)
    {
        Initialize();

        _source = source.Line;
        _original = source.Original;

        while (!IsAtEnd())
        {
            TokenStart = _currentIndex;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EndOfLine, "", null, CurrentLine, _currentIndex));
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

    private static bool IsAlpha(char c)
    {
        return char.IsLetter(c);
    }

    private void GetKeywordOrIdentifier()
    {
        if (Peek() == '$')
            AddStringIdentifierToken();
        else if (IsAlpha(Peek()) || Peek() == '.')
            // makeToken
            Add2PlusCharsToken();
        else
            AddIdentifierToken();
    }

    private void AddIdentifierToken()
    {
        string id = _source.Substring(TokenStart, 1);
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

        if (TokenLength == 1 || _natives.Get(identifier) != null)
            AddToken(TokenType.Identifier, identifier);
        else 
        {
            if (!IsAtEnd())
            {
                char letter = Peek();
                while (letter != ':' && !IsAtEnd())
                {
                    Advance();
                    letter = Peek();
                }

                identifier = _source.Substring(TokenStart, TokenLength);
            }
            AddToken(TokenType.Identifier, identifier);
        }
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
        TokenType keyword = GetKeywordAtPosition();
        if (keyword == TokenType.Backup) return;

        AddToken(keyword);
    }

    private void Add3CharToken()
    {
        TokenType keyword = GetKeywordAtPosition();
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
        TokenType keyword = GetKeywordAtPosition();
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
        string key = _source.Substring(TokenStart, TokenLength);
        try
        {
            TokenType keyword = KeywordsByLetter[TokenLength][key];
            return keyword;
        }
        catch
        {
            if (IsAlpha(Peek())) throw;

            // try backing up
            key = _source.Substring(TokenStart + 1, TokenLength - 1);
            if (!KeywordsByLetter[TokenLength - 1].ContainsKey(key)) throw;

            AddToken(TokenType.Identifier, _source.Substring(TokenStart, 1));
            _currentIndex = TokenStart + 1;
            return TokenType.Backup;
        }
    }

    private void AddStringIdentifierToken()
    {
        Advance();
        string id = _source.Substring(TokenStart, 2);
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
                AddToken(TokenType.String, _original.Substring(TokenStart, TokenLength));
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

    private static bool IsDigit(char c)
    {
        return char.IsDigit(c);
    }

    private void GetString()
    {
        while (Peek() != '"' && !IsAtEnd())
            Advance();

        if (IsAtEnd())
            throw new ScanException("Unterminated string.");

        Advance();

        string value = _original.Substring(TokenStart + 1, TokenLength - 2);
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
        _tokens.Add(new Token(type, text, literal, CurrentLine, TokenStart));
    }
}