using System.Collections.Generic;
using Trs80.Level1Basic.Utilities;

namespace Trs80.Level1Basic
{
    public interface IBasicProgram
    {
        List<ProgramStatement> Statements { get; set; }
        void List(ITrs80Console console);
        void Run();
        void New();
        void End();
        void AddStatement(ProgramStatement statement);
        void DeleteStatement(short lineNumber);
        void SetNextStatement(short lineNumber);
        short GetSuccessor(short lineNumber);
    }
}