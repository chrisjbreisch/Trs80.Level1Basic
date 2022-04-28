using Trs80.Level1Basic.Services;

namespace Trs80.Level1Basic.CommandModels
{
    public class SharedDataModel : ISharedDataModel
    {
        public ConsoleFont? OriginalConsoleFont { get; set; }
    }
}
