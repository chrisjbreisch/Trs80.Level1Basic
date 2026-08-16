using System;
using System.Collections.Generic;
using System.Linq;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.VirtualMachine.Scanner;

public class Scanner : IScanner
{
    private string _source;
    private string _original;
    private List<Token> _tokens;
    private int TokenStart { get; set; }
    private int TokenLength => _currentIndex - TokenStart;
    private int _currentIndex;
    private readonly ITrs80 _trs80;
    private readonly INativeFunctions _natives;
    private readonly IAppSettings _appSettings;
    private int _startOfLine;

    private static readonly Dictionary<int, Dictionary<string, TokenType>> KeywordsByLetter =
        CreateKeywordsByLetterDictionary();
    private string _currentLine;

    public Scanner(ITrs80 trs80, INativeFunctions natives, IAppSettings appSettings)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _natives = natives ?? throw new ArgumentNullException(nameof(natives));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }


    public List<Token> ScanTokens(SourceLine source)
    {
        if (source.Line == null) return null;
        
        Initialize();

        _source = source.Line;
        _original = source.Original;

        try
        {

            while (!IsAtEnd())
            {
                TokenStart = _currentIndex;
                ScanToken();
            }


            _tokens.Add(new Token(TokenType.EndOfLine, "", null, CurrentLine, 
                GetLinePosition() - TokenStart + _currentIndex ));
            return _tokens;
        }
        catch (Exception ex)
        {
            ExceptionHandler.HandleError(_trs80, _appSettings, ex);
            return null;
        }
    }

    private int GetLinePosition()
    {
        switch (_tokens.Count)
        {
            case 0:
                return 0;
            case 1:
                _startOfLine = TokenStart;
                return 0;
            default:
                return TokenStart - _startOfLine;
        }
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
                    {"OR", TokenType.Or},
                    {"P.", TokenType.Print},
                    {"R.", TokenType.R},
                    {"S.", TokenType.Step},
                    {"XOR", TokenType.Xor},
                    {"T.", TokenType.T},
                    {"TO", TokenType.To},
                }
            },
            {
                3, new Dictionary<string, TokenType>
                {
                    {"AND", TokenType.And},
                    {"ATN", TokenType.Identifier},
                    {"CLS", TokenType.Cls},
                    {"DIM", TokenType.Dim},
                    {"END", TokenType.End},
                    {"EQV", TokenType.Eqv},
                    {"FOR", TokenType.For},
                    {"IMP", TokenType.Imp},
                    {"IN.", TokenType.Input},
                    {"LET", TokenType.Let},
                    {"LO.", TokenType.Load},
                    {"ME.", TokenType.Merge},
                    {"MOD", TokenType.Mod},
                    {"NEW", TokenType.New},
                    {"NOT", TokenType.Not},
                    {"OUT", TokenType.Out},
                    {"REM", TokenType.Rem},
                    {"RUN", TokenType.Run},
                    {"SA.", TokenType.Save},
                    {"SET", TokenType.Set},
                    {"ST.", TokenType.Stop},
                    {"XOR", TokenType.Xor},
                }
            },
            {
                4, new Dictionary<string, TokenType>
                {
                    {"BEEP", TokenType.Beep},
                    {"CONT", TokenType.Cont},
                    {"DATA", TokenType.Data},
                    {"DEFD", TokenType.DefDbl},
                    {"DEFI", TokenType.DefInt},
                    {"DEFS", TokenType.DefSng},
                    {"DEL.", TokenType.Delete},
                    {"ELSE", TokenType.Else},
                    {"GOS.", TokenType.Gosub},
                    {"GOTO", TokenType.Goto},
                    {"LIST", TokenType.List},
                    {"LOAD", TokenType.Load},
                    {"NEXT", TokenType.Next},
                    {"POKE", TokenType.Poke},
                    {"READ", TokenType.Read},
                    {"REA.", TokenType.Read},
                    {"RET.", TokenType.Return},
                    {"SAVE", TokenType.Save},
                    {"STEP", TokenType.Step},
                    {"STOP", TokenType.Stop},
                    {"THEN", TokenType.Then},
                    {"WAIT", TokenType.Wait},
                }
            },
            {
                5, new Dictionary<string, TokenType>
                {
                    {"CLEAR", TokenType.Clear},
                    {"CLOAD", TokenType.Load},
                    {"GOSUB", TokenType.Gosub},
                    {"INPUT", TokenType.Input},
                    {"LLIST", TokenType.Llist},
                    {"MERGE", TokenType.Merge},
                    {"PRINT", TokenType.Print},
                    {"REST.", TokenType.Restore},
                    {"RESET", TokenType.Reset},
                    {"CSAVE", TokenType.Save},
                }
            },
            {
                6, new Dictionary<string, TokenType>
                {
                    {"DEFDBL", TokenType.DefDbl},
                    {"DELETE", TokenType.Delete},
                    {"DEFINT", TokenType.DefInt},
                    {"DEFSNG", TokenType.DefSng},
                    {"DEFSTR", TokenType.DefStr},
                    {"LPRINT", TokenType.Lprint},
                    {"RETURN", TokenType.Return},
                    {"SYSTEM", TokenType.System},
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
            case '^':
                AddToken(TokenType.Caret);
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
            case '?':
                AddToken(TokenType.Print);
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
            case '\'':
                CreateApostropheRemarkToken();
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
                if (c == '.' && !IsDigit(Peek()))
                    AddToken(TokenType.Identifier, ".");
                else if (IsDigit(c) || c == '.')
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
        if (IsTypeSuffix(Peek()))
        {
            Advance();
            AddUnknownIdentifierToken();
        }
        else if (IsDigit(Peek()))
        {
            while (IsIdentifierCharacter(Peek()))
                Advance();
            if (IsTypeSuffix(Peek()))
                Advance();
            AddUnknownIdentifierToken();
        }
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
        if (_source.Length - TokenStart >= 3 && _source.Substring(TokenStart, 3) == "ATN")
        {
            Advance();
            Add3CharToken();
            return;
        }

        while (IsIdentifierCharacter(Peek()))
            Advance();

        try
        {
            AddKeywordToken();
        }
        catch
        {
            AddUnknownIdentifierToken();
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
        return IsAtEnd() || (!IsIdentifierCharacter(Peek()) && Peek() != '.');
    }

    private static bool IsIdentifierCharacter(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private static bool IsTypeSuffix(char c)
    {
        return c is '$' or '%' or '!' or '#';
    }

    private void AddUnknownIdentifierToken()
    {
        string identifier = _source.Substring(TokenStart, TokenLength);

        if (TokenLength > 1 && IsTypeSuffix(Peek()))
        {
            Advance();
            identifier = _source.Substring(TokenStart, TokenLength);
        }

        if (TokenLength == 1 || _natives.Get(identifier) != null)
            AddToken(TokenType.Identifier, identifier);
        else
            AddToken(TokenType.Identifier, identifier);
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
        if (IsTypeSuffix(Peek()))
        {
            AddUnknownIdentifierToken();
            return;
        }

        switch (keyword)
        {
            case TokenType.Rem:
                CreateRemarkToken(keyword);
                break;
            case TokenType.Data:
                CreateDataTokens(keyword);
                break;
            default:
                AddToken(keyword);
                break;
        }
    }

    private void Add3CharToken()
    {
        TokenType keyword = GetKeywordAtPosition();
        if (keyword == TokenType.Backup) return;
        if (IsTypeSuffix(Peek()))
        {
            AddUnknownIdentifierToken();
            return;
        }

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
        if (IsTypeSuffix(Peek()))
        {
            AddUnknownIdentifierToken();
            return;
        }

        if (HasLongerKeywordMatch())
        {
            Advance();
            Add5PlusCharsToken();
            return;
        }

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
        if (HasLongerKeywordMatch())
        {
            Advance();
            Add6PlusCharsToken();
            return;
        }

        AddKeywordToken();
    }

    private void Add6CharToken()
    {
        if (HasLongerKeywordMatch())
        {
            Advance();
            Add7CharToken();
            return;
        }

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
                while (!IsAtEnd() && (IsIdentifierCharacter(Peek()) || Peek() == '.'))
                    Advance();

                AddUnknownIdentifierToken();
            }
        }
    }

    private bool HasLongerKeywordMatch()
    {
        int remaining = _source.Length - TokenStart;
        if (remaining <= TokenLength) return false;

        for (int candidateLength = TokenLength + 1; candidateLength <= remaining; candidateLength++)
        {
            if (!KeywordsByLetter.TryGetValue(candidateLength, out Dictionary<string, TokenType> candidates))
                continue;

            string candidate = _source.Substring(TokenStart, candidateLength);
            if (candidates.ContainsKey(candidate))
                return true;
        }

        return false;
    }

    private TokenType GetKeywordAtPosition()
    {
        int maxLength = Math.Min(_source.Length - TokenStart, KeywordsByLetter.Keys.Max());
        for (int candidateLength = maxLength; candidateLength >= 2; candidateLength--)
        {
            if (!KeywordsByLetter.TryGetValue(candidateLength, out Dictionary<string, TokenType> candidates))
                continue;

            string key = _source.Substring(TokenStart, candidateLength);
            if (!candidates.TryGetValue(key, out TokenType keyword))
                continue;

            _currentIndex = TokenStart + candidateLength;
            return keyword;
        }

        string fallbackKey = _source.Substring(TokenStart, TokenLength);
        try
        {
            TokenType keyword = KeywordsByLetter[TokenLength][fallbackKey];
            return keyword;
        }
        catch
        {
            if (IsAlpha(Peek())) throw;

            fallbackKey = _source.Substring(TokenStart + 1, TokenLength - 1);
            if (!KeywordsByLetter[TokenLength - 1].ContainsKey(fallbackKey)) throw;

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

    private void CreateApostropheRemarkToken()
    {
        while (Peek() != '\r' && !IsAtEnd())
            Advance();

        string remark = _source.Substring(TokenStart + 1, TokenLength - 1);
        AddToken(TokenType.Rem, remark);
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

        if ((Peek() == 'E' || Peek() == 'e' || Peek() == 'D' || Peek() == 'd') &&
            (IsDigit(PeekNext()) || PeekNext() == '+' || PeekNext() == '-'))
        {
            isInt = false;
            Advance();
            if (!IsDigit(Peek()))
                Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        if (Peek() is '#' or '!' or '%')
            Advance();

        object value;
        string number = _source.Substring(TokenStart, TokenLength);
        bool integerSuffix = number.EndsWith('%');
        bool singleSuffix = number.EndsWith('!');
        if (number.EndsWith('#') || number.EndsWith('!') || number.EndsWith('%'))
            number = number[..^1];
        if (_source[TokenStart..(TokenStart + TokenLength)].EndsWith('#')
            || number.Contains('D') || number.Contains('d')
            || (!singleSuffix && HasMoreThanSevenSignificantDigits(number)))
            value = double.Parse(number.Replace('D', 'E').Replace('d', 'e'),
                System.Globalization.CultureInfo.InvariantCulture);
        else if (isInt)
        {
            if (int.TryParse(number, out int integerValue))
            {
                bool negativeBoundary = integerValue == short.MaxValue + 1
                    && TokenStart > 0
                    && _source[TokenStart - 1] == '-';
                if (integerSuffix
                    && (integerValue < short.MinValue || integerValue > short.MaxValue)
                    && !negativeBoundary)
                    throw new ValueOutOfRangeException(-1, string.Empty, "Integer value out of range.");

                value = integerValue;
            }
            else
                value = float.Parse(number);
        }
        else
            value = float.Parse(number, System.Globalization.CultureInfo.InvariantCulture);

        AddToken(TokenType.Number, value);
    }

    private static bool HasMoreThanSevenSignificantDigits(string number)
    {
        int exponentIndex = number.IndexOfAny(['E', 'e']);
        string mantissa = exponentIndex >= 0 ? number[..exponentIndex] : number;
        string digits = mantissa.Replace(".", string.Empty).TrimStart('0');
        return digits.Length > 7;
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
        bool endQuote = false;

        while (Peek() != '"' && !IsAtEnd())
            Advance();

        if (!IsAtEnd())
        {
            endQuote = true;
            Advance();
        }

        string value = _original.Substring(TokenStart + 1, TokenLength - (endQuote ? 2 : 1));
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
        _tokens.Add(new Token(type, text, literal, CurrentLine, GetLinePosition()));
    }
}