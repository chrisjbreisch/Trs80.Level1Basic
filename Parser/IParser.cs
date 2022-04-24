namespace Trs80.Level1Basic.Parser
{
    public interface IParser
    {
        ParsedStatement Parse(string line);
    }
}