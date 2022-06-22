using System.Diagnostics.CodeAnalysis;

using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.Command.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ShutdownTrs80Command : ICommand<ShutdownTrs80Model>
{
    private readonly ITrs80 _trs80;
    private readonly ITrs80DataModel _sharedDataModel;
    public ShutdownTrs80Command(ITrs80 trs80, ITrs80DataModel sharedDataModel)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));
        _sharedDataModel = sharedDataModel ?? throw new ArgumentNullException(nameof(sharedDataModel));
    }

    public void Execute(ShutdownTrs80Model parameterObject)
    {
        _trs80.SetCurrentFont(_sharedDataModel.OriginalHostFont);
    }
}