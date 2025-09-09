using System.Collections.ObjectModel;
using System.Text.Json;
using Avalonia.Threading;
using System.IO;

namespace HtpcVibes;

public sealed class TileStore : System.IDisposable
{
    private readonly string _path;
    private readonly FileSystemWatcher _fw;
    private System.Timers.Timer? _debounce;

    public ObservableCollection<TileVm> Tiles { get; } = new();

    public TileStore(string path)
    {
        _path = path;
        Load();

        _fw = new FileSystemWatcher(Path.GetDirectoryName(_path)!, Path.GetFileName(_path))
        { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size };
        _fw.Changed += (_, __) => DebouncedReload();
        _fw.EnableRaisingEvents = true;
    }

    private void Load()
    {
        try
        {
            var json = File.ReadAllText(_path);
            var list = JsonSerializer.Deserialize<List<TileVm>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            Dispatcher.UIThread.Post(() =>
            {
                Tiles.Clear();
                foreach (var t in list) Tiles.Add(t);
            });
        }
        catch { }
    }

    private void DebouncedReload()
    {
        _debounce?.Stop();
        _debounce ??= new System.Timers.Timer(250) { AutoReset = false };
        _debounce.Elapsed += (_, __) => Load();
        _debounce.Start();
    }

    public void Dispose()
    {
        _fw.Dispose();
        _debounce?.Dispose();
    }
}
