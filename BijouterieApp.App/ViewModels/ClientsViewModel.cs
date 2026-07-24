using CommunityToolkit.Mvvm.ComponentModel;

namespace BijouterieApp.App.ViewModels;

public partial class ClientsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titre = "Gestion des Clients";
}