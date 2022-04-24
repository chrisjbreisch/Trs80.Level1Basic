using System;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Scanner
{
    public static class ErrorHandling
    {
        public static bool HadRuntimeError { get; private set; }
        public static bool HadParsingError { get; private set; }
        public static void Error(int lineNumber, string text)
        {
            Report(lineNumber, null, text);
        }

        public static void Error(Token token, string message)
        {
            HadParsingError = true;
            if (token.Type == TokenType.EndOfLine)
                Report(token.LineNumber, " at end", message);
            else
                Report(token.LineNumber, " at '" + token.Lexeme + "'", message);
        }

        private static void Report(int lineNumber, string location, string message)
        {
            Console.Error.WriteLine($"{lineNumber}: {message} {location}");
        }
    }
}
