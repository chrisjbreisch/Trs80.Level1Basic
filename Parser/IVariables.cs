namespace Trs80.Level1Basic.Parser
{
    public interface IVariables
    {
        dynamic SetValue(string name, object value);
        dynamic GetValue(string name);
    }
}