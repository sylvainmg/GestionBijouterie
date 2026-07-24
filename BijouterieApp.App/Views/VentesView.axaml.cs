using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class VentesView : UserControl
{
    public VentesView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is VentesViewModel vm)
        {
            vm.ChargerVentesCommand.Execute(null);
        }
    }
}
