using CommunityToolkit.Mvvm.ComponentModel;

namespace BijouterieApp.App.ViewModels;

public partial class VentesViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titre = "Gestion des Ventes";
}