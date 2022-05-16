using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.CommandModels;

public interface ITrs80DataModel
{
    ConsoleFont OriginalConsoleFont { get; set; }
}

public class Trs80DataModel : ITrs80DataModel
{
    public ConsoleFont OriginalConsoleFont { get; set; } = new();
}