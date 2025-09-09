using CommunityToolkit.Mvvm.Input;

namespace HtpcVibes;

public sealed class TileVm
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Exec { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;

    public IRelayCommand LaunchCommand => new RelayCommand(() => Launcher.Run(Exec));
}
