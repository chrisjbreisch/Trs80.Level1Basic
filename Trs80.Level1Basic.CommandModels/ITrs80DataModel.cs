using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.CommandModels;

public interface ITrs80DataModel
{
    HostFont OriginalHostFont { get; set; }
}