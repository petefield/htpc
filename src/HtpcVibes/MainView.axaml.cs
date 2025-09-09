using Avalonia.Controls;

namespace HtpcVibes;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new TileStore("apps.json");
    }
}
