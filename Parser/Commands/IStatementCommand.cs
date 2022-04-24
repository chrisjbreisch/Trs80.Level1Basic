namespace Trs80.Level1Basic.Parser.Commands
{
    public interface IStatementCommand  : ICommand
    {
        short LineNumber { get; set; }
    }
}
