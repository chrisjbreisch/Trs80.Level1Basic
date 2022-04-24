namespace Trs80.Level1Basic.Scanner
{
    public interface IScanner
    {
        ScannedStatement Scan(string line);
    }
}