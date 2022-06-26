namespace Trs80.Level1Basic.Common;

public sealed class AppSettings : IAppSettings
{
    public string FontName { get; set; }
    public short FontSize { get; set; }
    public bool DetailedErrors { get; set; }
}