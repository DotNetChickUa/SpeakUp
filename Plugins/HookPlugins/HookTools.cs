using System.ComponentModel;
using Shared;
using SharpHook;
using SharpHook.Data;

namespace HookPlugins;

[SpeakUpTool]
public static class HookTools
{
    [Description("Enter the provided text")]
    public static UioHookResult EnterText(string text)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateTextEntry(text);
    }

    [Description("Move the mouse to X;Y coordinates")]
    public static UioHookResult MoveMouse(short x, short y)
    {
        var simulator = new EventSimulator();
        return simulator.SimulateMouseMovement(x, y);
    }
}