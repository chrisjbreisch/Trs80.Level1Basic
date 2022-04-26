namespace Trs80.Level1Basic.Command
{
    public interface ICommand<in TPo>
    {
        void Execute(TPo parameterObject);
    }
}
