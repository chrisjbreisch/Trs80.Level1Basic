using Trs80.Level1Basic.Console;

namespace Trs80.Level1Basic.CommandModels;

public interface IConsoleDataModel
{
    ConsoleFont OriginalConsoleFont { get; set; }
}

public class ConsoleDataModel : IConsoleDataModel
{
    public ConsoleFont OriginalConsoleFont { get; set; } = new();
}