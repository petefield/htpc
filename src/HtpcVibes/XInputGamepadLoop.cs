using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using SharpDX.XInput;

namespace HtpcVibes;

public sealed class XInputGamepadLoop : IDisposable
{
    private readonly Window _win;
    private readonly Controller _ctl = new Controller(UserIndex.One);
    private Thread? _thread;
    private bool _running;
    private DateTime _bPressedAt = DateTime.MinValue;

    public event Action? QuitRequested;

    public XInputGamepadLoop(Window win) => _win = win;

    public void Start()
    {
        if (_running || !_ctl.IsConnected) return;
        _running = true;
        _thread = new Thread(Loop) { IsBackground = true };
        _thread.Start();
    }

    private void SendKey(Key k) => Dispatcher.UIThread.Post(() =>
        _win.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = k }));

    private void Loop()
    {
        GamepadButtonFlags last = 0;
        const short DEAD = 16000;

        while (_running)
        {
            var s = _ctl.GetState().Gamepad;
            var buttons = s.Buttons;

            if (buttons.HasFlag(GamepadButtonFlags.DPadLeft)  || s.LeftThumbX < -DEAD) SendKey(Key.Left);
            if (buttons.HasFlag(GamepadButtonFlags.DPadRight) || s.LeftThumbX >  DEAD) SendKey(Key.Right);
            if (buttons.HasFlag(GamepadButtonFlags.DPadUp)    || s.LeftThumbY >  DEAD) SendKey(Key.Up);
            if (buttons.HasFlag(GamepadButtonFlags.DPadDown)  || s.LeftThumbY < -DEAD) SendKey(Key.Down);

            if (!last.HasFlag(GamepadButtonFlags.A) && buttons.HasFlag(GamepadButtonFlags.A)) SendKey(Key.Enter);

            bool b = buttons.HasFlag(GamepadButtonFlags.B);
            bool bLast = last.HasFlag(GamepadButtonFlags.B);
            if (b && !bLast) _bPressedAt = DateTime.UtcNow;
            if (!b && bLast) _bPressedAt = DateTime.MinValue;
            if (b && _bPressedAt != DateTime.MinValue && (DateTime.UtcNow - _bPressedAt).TotalMilliseconds > 1500)
            {
                _bPressedAt = DateTime.MinValue;
                QuitRequested?.Invoke();
            }

            last = buttons;
            Thread.Sleep(120);
        }
    }

    public void Stop() { _running = false; _thread?.Join(300); }
    public void Dispose() => Stop();
}
