using Trs80.Level1Basic.Console;

namespace Trs80.Level1Basic.CommandModels;

public class ConsoleDataModel : IConsoleDataModel
{
    public ConsoleFont? OriginalConsoleFont { get; set; }
}