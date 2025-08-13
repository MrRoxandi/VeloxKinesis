using System.Runtime.InteropServices;

namespace VeloxKinesis.Native;

[StructLayout(LayoutKind.Sequential)]
internal struct INPUT
{
    public InputType type;
    public InputUnion u;
}

[StructLayout(LayoutKind.Explicit)]
internal struct InputUnion
{
    [FieldOffset(0)] public MOUSEINPUT mi;
    [FieldOffset(0)] public KEYBDINPUT ki;
    [FieldOffset(0)] public HARDWAREINPUT hi;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MOUSEINPUT
{
    public int dx;
    public int dy;
    public uint mouseData;
    public MouseEventF dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal struct KEYBDINPUT
{
    public VirtualKeyCode wVk;
    public ushort wScan;
    public KeyboardEventF dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal struct HARDWAREINPUT
{
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int X;
    public int Y;
}