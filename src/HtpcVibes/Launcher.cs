using System;
using System.Diagnostics;

namespace HtpcVibes;

public static class Launcher
{
    public static void Run(string exec)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(exec)) return;

            if (exec.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    UseShellExecute = false,
                    ArgumentList = { exec }
                });
                return;
            }

            if (exec.Contains(" ") || exec.Contains("\"") || exec.Contains("$") || exec.Contains("||") || exec.Contains("&&"))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "bash",
                    UseShellExecute = false,
                    ArgumentList = { "-lc", exec }
                });
            }
            else
            {
                Process.Start(new ProcessStartInfo { FileName = exec, UseShellExecute = true });
            }
        }
        catch
        {
            // TODO: toast
        }
    }
}
