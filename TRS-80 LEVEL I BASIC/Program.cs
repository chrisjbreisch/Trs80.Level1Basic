using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
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
            InitializeWindow();

            var serviceProvider = SetupDi();

            _basicInterpreter = serviceProvider.GetRequiredService<IBasicInterpreter>();

            Console.WriteLine("Enter TRS-80 LEVEL 1 BASIC Commands or Type EXIT to Exit.");
            while (true)
            {

                Console.WriteLine("READY");
                Console.Write("> ");

                string inputLine = GetInputLine();
                
                if (string.IsNullOrEmpty(inputLine)) continue;
                if (inputLine.ToLower() == "exit") break;

                ExecuteLine(inputLine);
            }
        }

        private static string GetInputLine()
        {
            char[] input = new char[1024];
            int charCount = 0;

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

                input[charCount++] = key.KeyChar;
            }

            return charCount <= 0 ? string.Empty : new string(input, 0, charCount);
        }

        private static ServiceProvider SetupDi()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IScanner, Scanner>()
                .AddSingleton<IParser, Parser>()
                .AddSingleton<IBuiltinFunctions, BuiltinFunctions>()
                .AddSingleton<IBasicInterpreter, BasicInterpreter>()
                .AddSingleton<IBasicEnvironment, BasicEnvironment>()
                .AddTransient<ITrs80Console, Trs80Console>()
                .AddSingleton<IScreen, Screen>()
                .BuildServiceProvider();
            return serviceProvider;
        }

        private static void InitializeWindow()
        {
            Win32Api.SetCurrentFont("Lucida Console", 36);
            Console.SetWindowSize(64, 16);
            Console.SetBufferSize(64, 160);
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

        //public static void Error(int lineNumber, string text)
        //{
        //    Report(lineNumber, null, text);
        //}

        //public static void Error(Token token, string message)
        //{
        //    //HadParsingError = true;
        //    if (token.Type == TokenType.EndOfLine)
        //        Report(token.LineNumber, " at end", message);
        //    else
        //        Report(token.LineNumber, " at '" + token.Lexeme + "'", message);
        //}

        //private static void Report(int lineNumber, string location, string message)
        //{
        //    Console.Error.WriteLine($"{lineNumber}: {message} {location}");
        //}
    }
}
