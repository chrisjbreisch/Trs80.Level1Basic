using Trs80.Level1Basic.Console;

namespace Trs80.Level1Basic.CommandModels;

public interface IConsoleDataModel
{
    ConsoleFont? OriginalConsoleFont { get; set; }
}