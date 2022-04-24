using System;
using System.Collections.Generic;
using System.Linq;
using Trs80.Level1Basic.Scanner.Tokens;

namespace Trs80.Level1Basic.Scanner
{
    public class Scanner : IScanner
    {
        private int _scanPosition;
        private string _line;
        static readonly char[] Operators = { '=', '*', '/', '+', '-', '<', '>'};
        static readonly char[] Separators = { ',', ';', ':', '\\', '.' };
        
        public ScannedStatement Scan(string line)
        {
            var scannedLine = new ScannedStatement {OriginalText = line};
            var tokens = new List<IToken>();

            char currentChar = PeekFirstChar(line);
            while (!EndOfLine())
            {
                while (char.IsWhiteSpace(currentChar))
                    currentChar = GetNextNonWhiteSpace();

                if (char.IsDigit(currentChar))
                    tokens.Add(ScanNumber());
                else if (currentChar == '"')
                    tokens.Add(ScanQuotedString());
                else if (Operators.Contains(currentChar))
                    tokens.Add(ScanOperator());
                else if (Separators.Contains(currentChar))
                    tokens.Add(ScanSeparator());
                else if (char.IsLetter(currentChar))
                    tokens.Add(ScanAlpha());
                else if (currentChar == '(')
                    tokens.Add(ScanLeftParen());
                else if (currentChar == ')')
                    tokens.Add(ScanRightParen());
                else
                    throw new InvalidOperationException();

                currentChar = PeekChar();
            }
            tokens.Add(new EndOfLineToken());

            scannedLine.Tokens = tokens;
            return scannedLine;
        }

        private IToken ScanRightParen()
        {
            ConsumeChar();
            return new RighParenToken();
        }

        private IToken ScanLeftParen()
        {
            ConsumeChar();
            return new LeftParenToken();
        }

        private char PeekFirstChar(string line)
        {
            _scanPosition = 0;
            _line = line;
            char currentChar = PeekChar();
            return currentChar;
        }

        private IToken ScanSeparator()
        {
            char c = ConsumeChar();
            switch (c)
            {
                case ',':
                    return new CommaToken();
                case ';':
                    return new SemiColonToken();
                case ':':
                    return new ColonToken();
                case '\\':
                    return new BackSlashToken();
                case '.':
                    return new PeriodToken();
                default:
                    throw new InvalidOperationException();
            }
        }

        private IToken ScanOperator()
        {
            char c = ConsumeChar();
            switch (c)
            {
                case '=':
                    return new EqualsToken();
                case '*':
                    return new StarToken();
                case '/':
                    return new SlashToken();
                case '+':
                    return new PlusToken();
                case '-':
                    return new MinusToken();
                case '<':
                    return new LessThanToken();
                case '>':
                    return new GreaterThanToken();
                default:
                    throw new InvalidOperationException();
            }
        }

        private char GetNextNonWhiteSpace()
        {
            while (char.IsWhiteSpace(PeekChar()))
                ConsumeChar();

            return PeekChar();
        }

        private IToken ScanAlpha()
        {
            string alphaNumeric = GetNextAlphaNumeric();

            switch (alphaNumeric.ToLower())
            {
                case "new":
                    return new NewToken();
                case "run":
                    return new RunToken();
                case "list":
                    return new ListToken();
                case "print":
                    return new PrintToken();
                case "end":
                    return new EndToken();
                case "let":
                    return new LetToken();
                case "goto":
                    return new GotoToken();
                case "if":
                    return new IfToken();
                case "then":
                    return new ThenToken();
                case "input":
                    return new InputToken();
                case "for":
                    return new ForToken();
                case "to":
                    return new ToToken();
                case "step":
                    return new StepToken();
                case "next":
                    return new NextToken();
                case "rem":
                    string remark = GetRestOfLine();
                    if (!string.IsNullOrEmpty(remark))
                        remark = remark.Substring(1);
                    return new RemToken(remark);
                case "load":
                    string loadFile = GetRestOfLine();
                    if (!string.IsNullOrEmpty(loadFile))
                        loadFile = loadFile.Substring(1);
                    return new LoadToken(loadFile);
                case "save":
                    string saveFile = GetRestOfLine();
                    if (!string.IsNullOrEmpty(saveFile))
                        saveFile = saveFile.Substring(1);
                    return new SaveToken(saveFile);
                default:
                    if (alphaNumeric.Length == 1)
                        return new LetterToken(alphaNumeric);
                    else
                        return new AlphaNumericToken(alphaNumeric);
            }
        }

        private string GetRestOfLine()
        {
            string rest = _line.Substring(_scanPosition);
            _scanPosition = _line.Length;
            return rest;
        }

        private string GetNextAlphaNumeric()
        {
            var characters = new char[1024];
            int index = 0;
            while (char.IsLetterOrDigit(PeekChar()))
                characters[index++] = ConsumeChar();

            string word = new string(characters, 0, index);
            return word;
        }

        private IToken ScanQuotedString()
        {
            var characters = new char[_line.Length];
            int index = 0;
            
            ConsumeChar();

            char currentChar = ConsumeChar();
            while (currentChar != '"')
            {
                characters[index++] = currentChar;
                
                if (EndOfLine())
                    throw new InvalidOperationException();

                currentChar = ConsumeChar();
            }
            return new LiteralToken<string>(new string(characters, 0, index));
        }

        private IToken ScanNumber()
        {
            bool isInteger = true;
            int startPosition = _scanPosition;
            while (char.IsDigit(PeekChar()))
                ConsumeChar();

            char currentChar = PeekChar();
            if (currentChar == '.')
            {
                isInteger = false;
                ConsumeChar();
                currentChar = PeekChar();
            }

            if (currentChar == 'e' || currentChar == 'E')
            {
                isInteger = false;
                ConsumeChar();
                currentChar = PeekChar();
                if (currentChar == '-' || currentChar == '+')
                    ConsumeChar();
            }
            if (!isInteger)
                while (char.IsDigit(PeekChar()))
                    ConsumeChar();

            if (isInteger)
                return new LiteralToken<int>(int.Parse(_line.Substring(startPosition, _scanPosition - startPosition)));

            return new LiteralToken<float>(float.Parse(_line.Substring(startPosition, _scanPosition - startPosition)));
        }

        private bool EndOfLine()
        {
            return _scanPosition >= _line.Length;
        }
        
        private char ConsumeChar()
        {
            char value = PeekChar();
            _scanPosition++;
            return value;
        }
        
        private char PeekChar()
        {
            return EndOfLine() ? '\0' : _line[_scanPosition];
        }
    }
}
