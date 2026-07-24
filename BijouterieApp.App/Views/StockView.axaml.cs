using Avalonia.Controls;
using BijouterieApp.App.ViewModels;

namespace BijouterieApp.App.Views;

public partial class StockView : UserControl
{
    public StockView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is StockViewModel vm)
        {
            vm.ChargerStockCommand.Execute(null);
        }
    }
}
