using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace HtpcVibes;

public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
        {
            var win = new Window
            {
                SystemDecorations = SystemDecorations.None,
                WindowState = WindowState.FullScreen,
                CanResize = false,
                Topmost = true,
                Content = new MainView()
            };

            win.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.None);

            var loop = new SdlGamepadLoop(win);
            loop.QuitRequested += () => d.Shutdown();
            loop.Start();

            var webHost = WebRemote.Start(win);
            d.ShutdownRequested += (_, __) =>
            {
                loop.Stop();
                webHost.Dispose();
            };

            d.MainWindow = win;
        }
        base.OnFrameworkInitializationCompleted();
    }
}
