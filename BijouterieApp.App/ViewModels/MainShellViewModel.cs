using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BijouterieApp.App.ViewModels;

public partial class MenuItem : ObservableObject
{
    public string Libelle { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
}

public partial class MainShellViewModel : ObservableObject
{
    private readonly SessionManager _sessionManager;

    [ObservableProperty]
    private ObservableObject _currentView;

    [ObservableProperty]
    private string _messageBienvenue = string.Empty;

    [ObservableProperty]
    private ObservableCollection<MenuItem> _itemsMenu = new();

    [ObservableProperty]
    private int _indexSelection = -1;

    public MainShellViewModel(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;

        if (sessionManager.UtilisateurCourant != null)
        {
            MessageBienvenue = $"Bienvenue, {sessionManager.UtilisateurCourant.Prenom} {sessionManager.UtilisateurCourant.Nom}";
        }

        ItemsMenu.Add(new MenuItem { Libelle = "Accueil", Tag = "Accueil" });
        ItemsMenu.Add(new MenuItem { Libelle = "Catégories", Tag = "Categories" });
        ItemsMenu.Add(new MenuItem { Libelle = "Bijoux", Tag = "Bijoux" });
        ItemsMenu.Add(new MenuItem { Libelle = "Clients", Tag = "Clients" });
        ItemsMenu.Add(new MenuItem { Libelle = "Stock", Tag = "Stock" });
        ItemsMenu.Add(new MenuItem { Libelle = "Ventes", Tag = "Ventes" });
        if (sessionManager.EstAdministrateur)
            ItemsMenu.Add(new MenuItem { Libelle = "Employés", Tag = "Employes" });

        _currentView = App.ServiceProvider.GetRequiredService<AccueilViewModel>();
    }

    partial void OnIndexSelectionChanged(int value)
    {
        if (value < 0 || value >= ItemsMenu.Count) return;
        var tag = ItemsMenu[value].Tag;

        CurrentView = tag switch
        {
            "Accueil" => App.ServiceProvider.GetRequiredService<AccueilViewModel>(),
            "Categories" => App.ServiceProvider.GetRequiredService<CategoriesViewModel>(),
            "Bijoux" => App.ServiceProvider.GetRequiredService<BijouxViewModel>(),
            "Clients" => App.ServiceProvider.GetRequiredService<ClientsViewModel>(),
            "Stock" => App.ServiceProvider.GetRequiredService<StockViewModel>(),
            "Ventes" => App.ServiceProvider.GetRequiredService<VentesViewModel>(),
            "Employes" => App.ServiceProvider.GetRequiredService<EmployesViewModel>(),
            _ => App.ServiceProvider.GetRequiredService<AccueilViewModel>()
        };
    }

    [RelayCommand]
    private void SeDeconnecter()
    {
        _sessionManager.Deconnecter();
    }
}