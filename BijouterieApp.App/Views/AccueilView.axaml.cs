using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class AccueilView : UserControl
{
    public AccueilView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is AccueilViewModel vm)
        {
            vm.ChargerStatistiquesCommand.Execute(null);
        }
    }
}
