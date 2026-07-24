using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class BijouxView : UserControl
{
    public BijouxView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is BijouxViewModel vm)
        {
            vm.ChargerBijouxCommand.Execute(null);
        }
    }
}