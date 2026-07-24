using CommunityToolkit.Mvvm.ComponentModel;

namespace BijouterieApp.App.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titre = "Gestion des Catégories";
}