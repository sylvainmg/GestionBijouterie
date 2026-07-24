using CommunityToolkit.Mvvm.ComponentModel;

namespace BijouterieApp.App.ViewModels;

public partial class EmployesViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titre = "Gestion des Employés";
}