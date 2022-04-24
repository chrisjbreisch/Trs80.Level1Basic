namespace Trs80.Level1Basic.Parser.Commands
{
    public class LoadCommand : ICommand
    {
        public string Path { get; set; }
        public LoadCommand(string path)
        {
            Path = path;
        }
    }
}