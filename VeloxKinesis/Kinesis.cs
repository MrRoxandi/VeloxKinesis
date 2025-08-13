using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using VeloxKinesis.Models;
using VeloxKinesis.Native;

namespace VeloxKinesis;

/// <summary>
/// Provides static methods to simulate keyboard and mouse input.
/// All methods in this class are thread-safe.
/// </summary>
public static class Kinesis
{
    /// <summary>
    /// Provides methods for simulating keyboard input.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// Simulates a key down event for the specified virtual key code.
        /// </summary>
        /// <param name="keyCode">The virtual key code to press.</param>
        public static void KeyDown(VirtualKeyCode keyCode) => new KinesisBuilder().AddKeyDown(keyCode).Send();

        /// <summary>
        /// Simulates a key up event for the specified virtual key code.
        /// </summary>
        /// <param name="keyCode">The virtual key code to release.</param>
        public static void KeyUp(VirtualKeyCode keyCode) => new KinesisBuilder().AddKeyUp(keyCode).Send();

        /// <summary>
        /// Simulates a key press (down and up) for the specified virtual key code.
        /// </summary>
        /// <param name="keyCode">The virtual key code to press.</param>
        public static void KeyPress(VirtualKeyCode keyCode) => new KinesisBuilder().AddKeyPress(keyCode).Send();

        /// <summary>
        /// Simulates typing a sequence of characters.
        /// </summary>
        /// <param name="text">The text to type.</param>
        public static void TypeText(string text) => new KinesisBuilder().AddText(text).Send();
    }

    /// <summary>
    /// Provides methods for simulating mouse input.
    /// </summary>
    public static class Mouse
    {
        /// <summary>
        /// Simulates moving the mouse cursor by a specified offset.
        /// </summary>
        /// <param name="dx">The horizontal offset in pixels.</param>
        /// <param name="dy">The vertical offset in pixels.</param>
        public static void MoveBy(int dx, int dy) => new KinesisBuilder().AddMouseMove(dx, dy).Send();

        /// <summary>
        /// Simulates moving the mouse cursor to a specified absolute screen coordinate.
        /// </summary>
        /// <param name="x">The absolute horizontal coordinate (X).</param>
        /// <param name="y">The absolute vertical coordinate (Y).</param>
        public static void MoveTo(int x, int y) => new KinesisBuilder().AddMouseMoveTo(x, y).Send();

        /// <summary>
        /// Simulates a mouse button down event.
        /// </summary>
        /// <param name="button">The mouse button to press.</param>
        public static void ButtonDown(MouseButton button) => new KinesisBuilder().AddMouseButtonDown(button).Send();

        /// <summary>
        /// Simulates a mouse button up event.
        /// </summary>
        /// <param name="button">The mouse button to release.</param>
        public static void ButtonUp(MouseButton button) => new KinesisBuilder().AddMouseButtonUp(button).Send();

        /// <summary>
        /// Simulates a single click of a mouse button.
        /// </summary>
        /// <param name="button">The mouse button to click. Defaults to Left.</param>
        public static void Click(MouseButton button = MouseButton.Left) => new KinesisBuilder().AddMouseClick(button).Send();

        /// <summary>
        /// Simulates a double click of a mouse button.
        /// </summary>
        /// <param name="button">The mouse button to double click. Defaults to Left.</param>
        public static void DoubleClick(MouseButton button = MouseButton.Left) => new KinesisBuilder().AddMouseDoubleClick(button).Send();

        /// <summary>
        /// Simulates vertical mouse wheel scrolling.
        /// </summary>
        /// <param name="scrollAmount">The amount to scroll. A positive value scrolls up, a negative value scrolls down.</param>
        public static void VerticalScroll(int scrollAmount) => new KinesisBuilder().AddMouseVerticalScroll(scrollAmount).Send();

        /// <summary>
        /// Simulates horizontal mouse wheel scrolling.
        /// </summary>
        /// <param name="scrollAmount">The amount to scroll. A positive value scrolls right, a negative value scrolls left.</param>
        public static void HorizontalScroll(int scrollAmount) => new KinesisBuilder().AddMouseHorizontalScroll(scrollAmount).Send();

        /// <summary>
        /// Gets the current position of the mouse cursor on the screen.
        /// </summary>
        /// <returns>A <see cref="Point"/> representing the cursor's coordinates.</returns>
        /// <exception cref="Win32Exception">Thrown if the cursor position cannot be retrieved.</exception>
        public static Point GetPosition()
        {
            if (NativeMethods.GetCursorPos(out var point))
            {
                return new Point(point.X, point.Y);
            }
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to get cursor position.");
        }
    }
}