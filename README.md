# VeloxKinesis

[![NuGet version](https://img.shields.io/nuget/v/VeloxKinesis.svg?style=for-the-badge)](https://www.nuget.org/packages/VeloxKinesis/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://github.com/YourUsername/VeloxKinesis?tab=MIT-1-ov-file)

**VeloxKinesis** is a high-performance, modern, and user-friendly .NET library for simulating keyboard and mouse input on Windows. It's built on the low-level `SendInput` WinAPI, providing a powerful and reliable way to automate user actions, create macros, or build testing tools.

## Features

- 🚀 **High Performance**: Directly uses the `SendInput` API to send input events in batches, minimizing overhead and ensuring maximum speed and reliability.
- 💡 **Dual API Design**:
  - **Simple Static API**: Get started instantly with the static `Kinesis` class for common, atomic actions like `KeyPress` or `MouseClick`.
  - **Powerful Fluent API**: Use the `KinesisBuilder` to chain complex sequences of events (e.g., hold Shift, type text, move the mouse, and click) into a single, efficient `SendInput` call.
- ⌨️ **Comprehensive Keyboard Control**: Simulate key presses, holds, and releases using virtual key codes. Easily type full strings, including Unicode characters.
- 🖱️ **Full Mouse Emulation**: Simulate mouse movement (relative and absolute), button clicks, double clicks, and vertical/horizontal scrolling.
- ⛓️ **Thread-Safe by Design**: The static `Kinesis` is fully thread-safe, allowing for safe use in multi-threaded applications.
- ✨ **Clean and Modern**: No-nonsense API that is intuitive and easy to integrate into any .NET project targeting Windows.
- 📝 **Well-Documented**: Includes XML comments for full IntelliSense support in your IDE.

## Installation

Install the package from NuGet Package Manager or via the .NET CLI:

```sh
dotnet add package VeloxKinesis
```

> **Note**: VeloxKinesis is a Windows-only library, as it relies on the User32 WinAPI.

## Quick Start: Static Simulator

The easiest way to simulate input is by using the static `Kinesis` class. It's perfect for simple, one-off actions.

```csharp
using VeloxKinesis;
using VeloxKinesis.Models; // For MouseButton
using System.Threading;

// --- Keyboard Simulation ---
Log.Info("Opening Notepad...");

// Press Win + R to open the Run dialog
Kinesis.Keyboard.KeyDown(VirtualKeyCode.LeftWin);
Kinesis.Keyboard.KeyPress(VirtualKeyCode.R);
Kinesis.Keyboard.KeyUp(VirtualKeyCode.LeftWin);

Thread.Sleep(500); // Give the dialog time to open

// Type "notepad" and press Enter
Kinesis.Keyboard.TypeText("notepad");
Kinesis.Keyboard.KeyPress(VirtualKeyCode.Enter);

Thread.Sleep(1000); // Give Notepad time to open

// Type a message
Kinesis.Keyboard.TypeText("Hello from VeloxKinesis!");

// --- Mouse Simulation ---
Log.Info("Moving mouse and clicking...");
// Move the mouse cursor to an absolute position
Kinesis.Mouse.MoveTo(500, 500);
Thread.Sleep(200);

// Move relatively from the current position
Kinesis.Mouse.MoveBy(50, 50);
Thread.Sleep(200);

// Click the left mouse button
Kinesis.Mouse.Click();

// Scroll down
Kinesis.Mouse.VerticalScroll(-240); // Negative value for scrolling down
```

## Advanced Usage: Fluent Input Builder

For complex sequences of actions, the `KinesisBuilder` provides a more powerful and efficient fluent API. It collects all your commands and sends them in a single batch, which is more performant and reliable for intricate automation tasks.

```csharp
using VeloxKinesis;
using VeloxKinesis.Models;

// Create a new builder instance
var builder = new KinesisBuilder();

// Build a sequence: Type "HELLO" in all caps, then type a sentence.
builder
    .AddKeyDown(VirtualKeyCode.Shift) // Hold Shift
    .AddKeyPress(VirtualKeyCode.H)
    .AddKeyPress(VirtualKeyCode.E)
    .AddKeyPress(VirtualKeyCode.L)
    .AddKeyPress(VirtualKeyCode.L)
    .AddKeyPress(VirtualKeyCode.O)
    .AddKeyUp(VirtualKeyCode.Shift)   // Release Left Shift
    .AddKeyPress(VirtualKeyCode.Space)
    .AddText("world, this is a fluent sequence!")
    .Send(); // Send all inputs at once

// Build another sequence: Right-click at a specific location
builder
    .AddMouseMoveTo(800, 600)
    .AddMouseClick(MouseButton.Right)
    .Send();
```

## API Overview

### `Kinesis` (Static)

The quick-and-easy way for atomic operations.

- `Kinesis.Keyboard`
  - `KeyDown(VirtualKeyCode)`
  - `KeyUp(VirtualKeyCode)`
  - `KeyPress(VirtualKeyCode)`
  - `TypeText(string)`
- `Kinesis.Mouse`
  - `MoveBy(dx, dy)`
  - `MoveTo(x, y)`
  - `ButtonDown(MouseButton)`
  - `ButtonUp(MouseButton)`
  - `Click(MouseButton)`
  - `DoubleClick(MouseButton)`
  - `VerticalScroll(amount)`
  - `HorizontalScroll(amount)`
  - `GetPosition()`

### `KinesisBuilder` (Instance)

The fluent way to build complex command sequences. Create an instance with `new KinesisBuilder()`.

- **Keyboard Methods**
  - `.AddKeyDown(VirtualKeyCode)`
  - `.AddKeyUp(VirtualKeyCode)`
  - `.AddKeyPress(VirtualKeyCode)`
  - `.AddText(string)`
- **Mouse Methods**
  - `.AddMouseMove(dx, dy)`
  - `.AddMouseMoveTo(x, y)`
  - `.AddMouseButtonDown(MouseButton)`
  - `.AddMouseButtonUp(MouseButton)`
  - `.AddMouseClick(MouseButton)`
  - `.AddMouseDoubleClick(MouseButton)`
  - `.AddMouseXClick(buttonId)` (for side buttons)
  - `.AddMouseVerticalScroll(amount)`
  - `.AddMouseHorizontalScroll(amount)`
- **Execution**
  - `.Send()`: Executes all added commands and clears the builder for reuse.

## Virtual Key Codes

`VeloxKinesis` provides the `VirtualKeyCode` enum, which contains a comprehensive, hardware-independent list of all standard keys. This ensures your code works reliably across different keyboard layouts.

```csharp
// Examples:
VirtualKeyCode.LeftShift     // Left Shift
VirtualKeyCode.RightCtrl     // Right Control
VirtualKeyCode.Enter         // Enter
VirtualKeyCode.A             // 'A' key
VirtualKeyCode.Numpad_5      // 5 on the numpad
VirtualKeyCode.F5            // F5 function key
```

## Contributing

Contributions are welcome! If you find a bug, have a feature request, or want to improve the code, please feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
