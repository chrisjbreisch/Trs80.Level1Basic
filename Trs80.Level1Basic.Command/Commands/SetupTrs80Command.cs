using Trs80.Level1Basic.CommandModels;
using Trs80.Level1Basic.VirtualMachine.Machine;

namespace Trs80.Level1Basic.Command.Commands;

public class SetupTrs80Command : ICommand<SetupTrs80Model>
{
    private readonly ITrs80 _trs80;

    public SetupTrs80Command(ITrs80 trs80, ITrs80DataModel trs80DataModel)
    {
        _trs80 = trs80 ?? throw new ArgumentNullException(nameof(trs80));

        if (trs80DataModel is null) throw new ArgumentNullException(nameof(trs80DataModel));
        trs80DataModel.OriginalHostFont = _trs80.GetCurrentFont();
    }

    public void Execute(SetupTrs80Model model)
    {
        InitializeWindow();
        WriteBanner();
        WritePrompt();
    }

    private void InitializeWindow()
    {
        _trs80.InitializeWindow();
    }

    private void WriteBanner()
    {
        _trs80.WriteLine("TRS-80 Model I Level I BASIC Interpreter by Chris J. Breisch");
        _trs80.WriteLine("Copyright 2022 - Apache 2.0 license");
    }

    private void WritePrompt()
    {
        _trs80.WriteLine();
        _trs80.WriteLine("READY");
    }
}