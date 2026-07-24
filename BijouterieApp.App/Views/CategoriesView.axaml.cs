using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class CategoriesView : UserControl
{
    public CategoriesView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is CategoriesViewModel vm)
        {
            vm.ChargerCategoriesCommand.Execute(null);
        }
    }
}