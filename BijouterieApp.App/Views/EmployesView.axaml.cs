using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class EmployesView : UserControl
{
    public EmployesView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is EmployesViewModel vm)
        {
            vm.ChargerEmployesCommand.Execute(null);
        }
    }
}
