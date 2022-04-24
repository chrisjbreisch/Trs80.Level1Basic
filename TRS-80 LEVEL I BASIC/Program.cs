using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Trs80.Level1Basic.Domain;
using Trs80.Level1Basic.Graphics;
using Trs80.Level1Basic.Services;
using Trs80.Level1Basic.Services.Interpreter;
using Trs80.Level1Basic.Services.Parser;

namespace Trs80.Level1Basic
{
    internal class Program
    {
        private static IBasicInterpreter _basicInterpreter;

        private static void Main()
        {
            //Console.SetWindowSize(64, 16);
            //Console.SetBufferSize(64, 16);
            //set font: https://stackoverflow.com/questions/20631634/changing-font-in-a-console-window-in-c-sharp
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IScanner, Scanner>()
                .AddSingleton<IParser, Parser>()
                .AddSingleton<IBuiltinFunctions, BuiltinFunctions>()
                .AddSingleton<IBasicInterpreter, BasicInterpreter>()
                .AddSingleton<IBasicEnvironment, BasicEnvironment>()
                .AddTransient<ITrs80Console, Trs80Console>()
                .AddSingleton<IScreen, Screen>()
                .BuildServiceProvider();

            _basicInterpreter = serviceProvider.GetRequiredService<IBasicInterpreter>();

            Console.WriteLine("Enter TRS-80 LEVEL 1 BASIC Commands or Type EXIT to Exit.");
            while (true)
            {
                int charCount = 0;
                var input = new char[1024];

                Console.WriteLine("READY");
                Console.Write("> ");
                while (true)
                {
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        Console.Write(" \b");
                        input[charCount--] = '\0';
                        continue;
                    }
                    char c = key.KeyChar;
                    //Console.Write($"\b{c}");
                    input[charCount++] = c;
                }

                if (charCount <= 0) continue;

                string inputLine = new string(input, 0, charCount);
                if (inputLine.ToLower() == "exit") break;

                ExecuteLine(inputLine);
            }
        }

        private static void ExecuteLine(string inputLine)
        {
            try
            {
                _basicInterpreter.Interpret(inputLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SORRY");
                if (Debugger.IsAttached)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        public static void Error(int lineNumber, string text)
        {
            Report(lineNumber, null, text);
        }

        public static void Error(Token token, string message)
        {
            //HadParsingError = true;
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
