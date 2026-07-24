using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class ClientsView : UserControl
{
    public ClientsView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ClientsViewModel vm)
        {
            vm.ChargerClientsCommand.Execute(null);
        }
    }
}
