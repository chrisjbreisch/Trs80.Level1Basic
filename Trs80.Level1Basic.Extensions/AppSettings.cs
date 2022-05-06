﻿
namespace Trs80.Level1Basic.Common;

public interface IAppSettings
{
    string FontName { get; set; }
    short FontSize { get; set; }
}

public sealed class AppSettings : IAppSettings
{
    public string FontName { get; set; }
    public short FontSize { get; set; }
}