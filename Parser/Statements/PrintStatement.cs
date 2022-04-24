using System;
using Trs80.Level1Basic.Parser.Commands;
using Trs80.Level1Basic.Parser.Expressions;
using Trs80.Level1Basic.Utilities;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class PrintStatement : IStatement, ICommand
    {
        private readonly bool _addLineFeed;
        private readonly ITrs80Console _console;
        public PrintStatement(PrintExpression printText, ITrs80Console console, bool addLineFeed)
        {
            PrintText = printText ?? throw new ArgumentNullException(nameof(printText));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _addLineFeed = addLineFeed;
        }
        public PrintExpression PrintText { get; set; }

        public override string ToString()
        {
            return base.ToString() + " " + PrintText;
        }

        public void Execute()
        {
            if (_addLineFeed)
                _console.WriteLine(PrintText.Value);
            else
                _console.Write(PrintText.Value);
        }
    }
}