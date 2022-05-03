using Trs80.Level1Basic.Interpreter;

namespace Trs80.Level1Basic.CommandModels;

public interface IConsoleDataModel
{
    ConsoleFont? OriginalConsoleFont { get; set; }
}