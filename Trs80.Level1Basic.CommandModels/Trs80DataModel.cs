using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.CommandModels;

public class Trs80DataModel : ITrs80DataModel
{
    public HostFont OriginalHostFont { get; set; } = new();
}