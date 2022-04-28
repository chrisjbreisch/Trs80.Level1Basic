using Trs80.Level1Basic.Services;

namespace Trs80.Level1Basic.CommandModels;

public interface ISharedDataModel
{
    ConsoleFont? OriginalConsoleFont { get; set; }
}