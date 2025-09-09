using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using SDL2;

namespace HtpcVibes;

public sealed class SdlGamepadLoop : IDisposable
{
    private readonly Window _win;
    private Thread? _thread;
    private bool _running;
    private IntPtr _controller = IntPtr.Zero;
    private DateTime _bPressedAt = DateTime.MinValue;

    public event Action? QuitRequested;

    public SdlGamepadLoop(Window win) => _win = win;

    public void Start()
    {
        if (_running) return;
        if (SDL.SDL_Init(SDL.SDL_INIT_GAMECONTROLLER) != 0) return;

        for (int i = 0; i < SDL.SDL_NumJoysticks(); i++)
        {
            if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_TRUE)
            {
                _controller = SDL.SDL_GameControllerOpen(i);
                break;
            }
        }
        if (_controller == IntPtr.Zero) return;

        _running = true;
        _thread = new Thread(Loop) { IsBackground = true };
        _thread.Start();
    }

    private void SendKey(Key k) =>
        Dispatcher.UIThread.Post(() =>
        {
            _win.RaiseEvent(new KeyEventArgs
            {
                RoutedEvent = InputElement.KeyDownEvent,
                Key = k
            });
        });

    private void Loop()
    {
        const short DEAD = 16000;
        bool lastA = false, lastB = false;

        while (_running)
        {
            SDL.SDL_GameControllerUpdate();

            bool left  = SDL.SDL_GameControllerGetButton(_controller, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT)  == 1;
            bool right = SDL.SDL_GameControllerGetButton(_controller, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT) == 1;
            bool up    = SDL.SDL_GameControllerGetButton(_controller, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP)    == 1;
            bool down  = SDL.SDL_GameControllerGetButton(_controller, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN) == 1;

            short lx = SDL.SDL_GameControllerGetAxis(_controller, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX);
            short ly = SDL.SDL_GameControllerGetAxis(_controller, SDL.SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY);

            if (left  || lx < -DEAD) SendKey(Key.Left);
            if (right || lx >  DEAD) SendKey(Key.Right);
            if (up    || ly < -DEAD) SendKey(Key.Up);
            if (down  || ly >  DEAD) SendKey(Key.Down);

            bool a = SDL.SDL_GameControllerGetButton(_controller, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) == 1;
            bool b = SDL.SDL_GameControllerGetButton(_controller, SDL.SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B) == 1;

            if (!lastA && a) SendKey(Key.Enter);
            if (!lastB && b) SendKey(Key.Escape);

            if (b && !lastB) _bPressedAt = DateTime.UtcNow;
            if (!b && lastB) _bPressedAt = DateTime.MinValue;

            if (b && _bPressedAt != DateTime.MinValue && (DateTime.UtcNow - _bPressedAt).TotalMilliseconds > 1500)
            {
                _bPressedAt = DateTime.MinValue;
                QuitRequested?.Invoke();
            }

            lastA = a; lastB = b;
            Thread.Sleep(120);
        }
    }

    public void Stop()
    {
        _running = false;
        _thread?.Join(500);
        if (_controller != IntPtr.Zero)
        {
            SDL.SDL_GameControllerClose(_controller);
            _controller = IntPtr.Zero;
        }
        SDL.SDL_QuitSubSystem(SDL.SDL_INIT_GAMECONTROLLER);
        SDL.SDL_Quit();
    }

    public void Dispose() => Stop();
}
