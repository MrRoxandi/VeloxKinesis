using System.Runtime.InteropServices;

namespace VeloxKinesis.Native;

internal static partial class NativeMethods
{
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

    [LibraryImport("user32.dll")]
    public static partial nint GetMessageExtraInfo();

    [LibraryImport("user32.dll")]
    public static partial uint MapVirtualKey(uint uCode, uint uMapType);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out POINT lpPoint);

    [LibraryImport("user32.dll")]
    public static partial int GetSystemMetrics(int nIndex);

    public const int SM_CXSCREEN = 0;
    public const int SM_CYSCREEN = 1;
}