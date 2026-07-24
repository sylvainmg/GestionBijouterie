using CommunityToolkit.Mvvm.ComponentModel;

namespace BijouterieApp.App.ViewModels;

public partial class StockViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titre = "Gestion du Stock";
}