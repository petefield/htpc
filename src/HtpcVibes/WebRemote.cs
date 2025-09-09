using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace HtpcVibes;

public static class WebRemote
{
    private static void SendKey(Window w, Key k) =>
        Dispatcher.UIThread.Post(() => w.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = k }));

    public static IHost Start(Window win, string bind = "http://127.0.0.1:7777", string? pin = null)
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(web =>
            {
                web.UseUrls(bind);
                web.Configure(app =>
                {
                    app.UseDefaultFiles();
                    app.UseStaticFiles();
                    app.MapPost("/key/{name}", (HttpContext ctx, string name) =>
                    {
                        if (!string.IsNullOrEmpty(pin) && ctx.Request.Query["pin"] != pin)
                            return Results.Unauthorized();

                        switch (name.ToLowerInvariant())
                        {
                            case "up": SendKey(win, Key.Up); break;
                            case "down": SendKey(win, Key.Down); break;
                            case "left": SendKey(win, Key.Left); break;
                            case "right": SendKey(win, Key.Right); break;
                            case "a": SendKey(win, Key.Enter); break;
                            case "b": SendKey(win, Key.Escape); break;
                        }
                        return Results.Ok();
                    });
                });
            });
        var host = builder.Build();
        host.Start();
        return host;
    }
}
