namespace Trs80.Level1Basic.Parser.Commands
{
    public class SaveCommand : ICommand
    {
        public string Path { get; set; }
        public SaveCommand(string path)
        {
            Path = path;
        }
    }
}