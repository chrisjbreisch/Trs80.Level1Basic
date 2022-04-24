using System;
using Trs80.Level1Basic.Utilities;

namespace Trs80.Level1Basic.Parser.Statements
{
    public class InputStatement : IStatement
    {
        private readonly string _variableName;
        private readonly IVariables _variables;
        private readonly ITrs80Console _console;
        private readonly string _prompt;

        public InputStatement(string prompt, string variableName, IVariables variables, ITrs80Console console)
        {
            _prompt = prompt ?? string.Empty;
            _variableName = variableName;
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void Execute()
        {
            _console.Write($"{_prompt}? ");
            string input = _console.ReadLine();
            long value = long.Parse(input);
            _variables.SetValue(_variableName, value);
        }
    }
}
