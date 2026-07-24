using CommunityToolkit.Mvvm.ComponentModel;

namespace BijouterieApp.App.ViewModels;

public partial class BijouxViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titre = "Gestion des Bijoux";
}