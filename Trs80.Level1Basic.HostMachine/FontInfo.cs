using System.Runtime.InteropServices;

namespace Trs80.Level1Basic.HostMachine;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct FontInfo
{
    public int cbSize;
    public int FontIndex;
    public short FontWidth;
    public short FontSize;
    public int FontFamily;
    public int FontWeight;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string FontName;
}