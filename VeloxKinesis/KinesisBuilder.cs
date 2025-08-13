using System.ComponentModel;
using System.Runtime.InteropServices;
using VeloxKinesis.Models;
using VeloxKinesis.Native;

namespace VeloxKinesis;

/// <summary>
/// Provides a fluent API to build a sequence of input events.
/// This class is not thread-safe. A new instance should be used for each sequence of events or by each thread.
/// </summary>
public class KinesisBuilder
{
    private readonly List<INPUT> _inputs = [];

    /// <summary>
    /// Adds a key-down event to the sequence for the specified virtual key.
    /// </summary>
    /// <param name="keyCode">The virtual key to press.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddKeyDown(VirtualKeyCode keyCode) => AddKey(keyCode, KeyboardEventF.ScanCode);

    /// <summary>
    /// Adds a key-up event to the sequence for the specified virtual key.
    /// </summary>
    /// <param name="keyCode">The virtual key to release.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddKeyUp(VirtualKeyCode keyCode) => AddKey(keyCode, KeyboardEventF.ScanCode | KeyboardEventF.KeyUp);

    /// <summary>
    /// Adds a key-press event (key-down followed by key-up) to the sequence.
    /// </summary>
    /// <param name="keyCode">The virtual key to press.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddKeyPress(VirtualKeyCode keyCode) => AddKeyDown(keyCode).AddKeyUp(keyCode);

    /// <summary>
    /// Adds a sequence of Unicode character key presses to simulate typing text.
    /// </summary>
    /// <param name="text">The text to type.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddText(string text)
    {
        foreach (char ch in text)
        {
            AddCharacter(ch);
        }
        return this;
    }

    /// <summary>
    /// Adds a mouse-button-down event to the sequence.
    /// </summary>
    /// <param name="button">The mouse button to press.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseButtonDown(MouseButton button) => AddMouseButton(button, isDown: true);

    /// <summary>
    /// Adds a mouse-button-up event to the sequence.
    /// </summary>
    /// <param name="button">The mouse button to release.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseButtonUp(MouseButton button) => AddMouseButton(button, isDown: false);

    /// <summary>
    /// Adds a mouse-click event (button-down followed by button-up) to the sequence.
    /// </summary>
    /// <param name="button">The mouse button to click.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseClick(MouseButton button) => AddMouseButtonDown(button).AddMouseButtonUp(button);

    /// <summary>
    /// Adds a mouse-double-click event to the sequence.
    /// </summary>
    /// <param name="button">The mouse button to double-click.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseDoubleClick(MouseButton button) => AddMouseClick(button).AddMouseClick(button);

    /// <summary>
    /// Adds an X mouse-button-down event to the sequence (e.g., for side buttons).
    /// </summary>
    /// <param name="xButtonId">The ID of the X button (1 or 2).</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseXButtonDown(int xButtonId) => AddMouseXButton(xButtonId, isDown: true);

    /// <summary>
    /// Adds an X mouse-button-up event to the sequence.
    /// </summary>
    /// <param name="xButtonId">The ID of the X button (1 or 2).</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseXButtonUp(int xButtonId) => AddMouseXButton(xButtonId, isDown: false);

    /// <summary>
    /// Adds an X mouse-button-click event to the sequence.
    /// </summary>
    /// <param name="xButtonId">The ID of the X button to click (1 or 2).</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseXClick(int xButtonId) => AddMouseXButtonDown(xButtonId).AddMouseXButtonUp(xButtonId);

    /// <summary>
    /// Adds a relative mouse movement to the sequence.
    /// </summary>
    /// <param name="dx">The horizontal offset to move.</param>
    /// <param name="dy">The vertical offset to move.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseMove(int dx, int dy) => AddMouseInput(dx, dy, 0, MouseEventF.Move);

    /// <summary>
    /// Adds an absolute mouse movement to a specific screen coordinate.
    /// </summary>
    /// <param name="x">The absolute horizontal (X) screen coordinate.</param>
    /// <param name="y">The absolute vertical (Y) screen coordinate.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseMoveTo(int x, int y)
    {
        var (absX, absY) = ToAbsoluteCoordinates(x, y);
        return AddMouseInput(absX, absY, 0, MouseEventF.Move | MouseEventF.Absolute);
    }

    /// <summary>
    /// Adds a vertical mouse wheel scroll to the sequence.
    /// </summary>
    /// <param name="scrollAmount">The amount to scroll. Positive for up, negative for down. One unit is 120.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseVerticalScroll(int scrollAmount) => AddMouseInput(0, 0, (uint)scrollAmount, MouseEventF.VerticalWheel);

    /// <summary>
    /// Adds a horizontal mouse wheel scroll to the sequence.
    /// </summary>
    /// <param name="scrollAmount">The amount to scroll. Positive for right, negative for left. One unit is 120.</param>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    public KinesisBuilder AddMouseHorizontalScroll(int scrollAmount) => AddMouseInput(0, 0, (uint)scrollAmount, MouseEventF.HorizontalWheel);


    /// <summary>
    /// Sends all accumulated input events in the sequence to the system and clears the sequence.
    /// </summary>
    /// <returns>The current <see cref="KinesisBuilder"/> instance for fluent chaining.</returns>
    /// <exception cref="Win32Exception">Thrown if the system fails to process one or more input events.</exception>
    public KinesisBuilder Send()
    {
        if (_inputs.Count == 0) return this;

        var inputArray = _inputs.ToArray();
        uint successfulInputs = NativeMethods.SendInput((uint)inputArray.Length, inputArray, Marshal.SizeOf<INPUT>());
        _inputs.Clear();

        if (successfulInputs != inputArray.Length)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to send all specified input events.");
        }

        return this;
    }

    private KinesisBuilder AddKey(VirtualKeyCode keyCode, KeyboardEventF flags)
    {
        var input = new INPUT
        {
            type = InputType.Keyboard,
            u = {
                ki = new KEYBDINPUT
                {
                    wVk = keyCode,
                    wScan = 0,
                    dwFlags = IsExtendedKey(keyCode) ? flags : (flags & ~KeyboardEventF.ExtendedKey),
                    time = 0,
                    dwExtraInfo = NativeMethods.GetMessageExtraInfo()
                }
            }
        };
        _inputs.Add(input);
        return this;
    }

    private KinesisBuilder AddCharacter(char character)
    {
        var down = new INPUT
        {
            type = InputType.Keyboard,
            u = { ki = new KEYBDINPUT { wScan = character, dwFlags = KeyboardEventF.Unicode } }
        };
        var up = new INPUT
        {
            type = InputType.Keyboard,
            u = { ki = new KEYBDINPUT { wScan = character, dwFlags = KeyboardEventF.Unicode | KeyboardEventF.KeyUp } }
        };
        _inputs.Add(down);
        _inputs.Add(up);
        return this;
    }

    private KinesisBuilder AddMouseButton(MouseButton button, bool isDown) => AddMouseInput(0, 0, 0, ToMouseEventF(button, isDown));

    private KinesisBuilder AddMouseXButton(int xButtonId, bool isDown)
    {
        if (xButtonId != 1 && xButtonId != 2) throw new ArgumentOutOfRangeException(nameof(xButtonId));
        return AddMouseInput(0, 0, (uint)xButtonId, isDown ? MouseEventF.XDown : MouseEventF.XUp);
    }

    private KinesisBuilder AddMouseInput(int dx, int dy, uint mouseData, MouseEventF flags)
    {
        var input = new INPUT
        {
            type = InputType.Mouse,
            u = {
                mi = new MOUSEINPUT
                {
                    dx = dx,
                    dy = dy,
                    mouseData = mouseData,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = NativeMethods.GetMessageExtraInfo()
                }
            }
        };
        _inputs.Add(input);
        return this;
    }

    private static (int x, int y) ToAbsoluteCoordinates(int x, int y)
    {
        int screenWidth = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN) - 1;
        int screenHeight = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN) - 1;
        return (x * 65535 / (screenWidth > 0 ? screenWidth : 1), y * 65535 / (screenHeight > 0 ? screenHeight : 1));
    }

    private static bool IsExtendedKey(VirtualKeyCode keyCode) => keyCode switch
    {

        VirtualKeyCode.RightCtrl or VirtualKeyCode.RightAlt => true,
        VirtualKeyCode.Divide => true,
        VirtualKeyCode.Insert or VirtualKeyCode.Delete or VirtualKeyCode.Home or VirtualKeyCode.End
            or VirtualKeyCode.PageUp or VirtualKeyCode.PageDown => true,
        VirtualKeyCode.UpArrow or VirtualKeyCode.DownArrow or VirtualKeyCode.LeftArrow or VirtualKeyCode.RightArrow => true,
        VirtualKeyCode.LeftWin or VirtualKeyCode.RightWin or VirtualKeyCode.Apps => true,
        VirtualKeyCode.PrintScreen or VirtualKeyCode.Numlock or VirtualKeyCode.Sleep => true,
        _ => false
    };

    private static MouseEventF ToMouseEventF(MouseButton button, bool isDown) => button switch
    {
        MouseButton.Left => isDown ? MouseEventF.LeftDown : MouseEventF.LeftUp,
        MouseButton.Right => isDown ? MouseEventF.RightDown : MouseEventF.RightUp,
        MouseButton.Middle => isDown ? MouseEventF.MiddleDown : MouseEventF.MiddleUp,
        _ => throw new ArgumentOutOfRangeException(nameof(button)),
    };
}