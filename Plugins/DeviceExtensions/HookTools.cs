using System.ComponentModel;
using System.Diagnostics;
using Shared;
using SharpHook;
using SharpHook.Data;

namespace DeviceExtensions;

[SpeakUpTool]
public static class HookTools
{
    [Description("Enter the provided text")]
    public static UioHookResult EnterText(string text)
    {
        Debug.WriteLine($"Simulating text entry: {text}");
        var simulator = new EventSimulator();
        return simulator.SimulateTextEntry(text);
    }

    [Description("Press the specified key")]
    public static UioHookResult KeyPress(KeyCode keyCode)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateKeyPress(keyCode);
    }

    [Description("Release the specified key")]
    public static UioHookResult KeyRelease(KeyCode keyCode)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateKeyRelease(keyCode);
    }

    [Description("Press the specified key")]
    public static UioHookResult MousePress(MouseButton mouseButton)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateMousePress(mouseButton);
    }

    [Description("Release the specified key")]
    public static UioHookResult MouseRelease(MouseButton mouseButton)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateMouseRelease(mouseButton);
    }

    [Description("Move the mouse to X;Y coordinates")]
    public static UioHookResult MoveMouse(short x, short y)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateMouseMovement(x, y);
    }

    [Description("Scroll the mouse wheel with the specified rotation, direction, and scroll type")]
    public static UioHookResult MouseWheel(short rotation, MouseWheelScrollDirection direction, MouseWheelScrollType scrollType)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateMouseWheel(rotation, direction, scrollType);
    }
}