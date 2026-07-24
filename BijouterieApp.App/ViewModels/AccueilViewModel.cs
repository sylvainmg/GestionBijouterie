using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.App.ViewModels;

public partial class AccueilViewModel : ObservableObject
{
    private readonly DashboardService _dashboardService;

    public AccueilViewModel(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
        MessageBienvenue = "Bienvenue dans l'application de gestion de bijouterie";
        DateHeure = DateTime.Now.ToString("dddd dd MMMM yyyy HH:mm");
    }

    [ObservableProperty]
    private string _messageBienvenue = string.Empty;

    [ObservableProperty]
    private string _dateHeure = string.Empty;

    [ObservableProperty]
    private int _nombreBijoux;

    [ObservableProperty]
    private int _nombreClients;

    [ObservableProperty]
    private int _nombreVentes;

    [ObservableProperty]
    private int _nombreEmployes;

    [ObservableProperty]
    private decimal _chiffreAffaires;

    [ObservableProperty]
    private decimal _chiffreAffairesMois;

    [ObservableProperty]
    private int _bijouxStockFaible;

    [ObservableProperty]
    private bool _estChargement;

    [RelayCommand]
    private async Task ChargerStatistiques()
    {
        EstChargement = true;
        try
        {
            NombreBijoux = await _dashboardService.NombreBijouxAsync();
            NombreClients = await _dashboardService.NombreClientsAsync();
            NombreVentes = await _dashboardService.NombreVentesAsync();
            NombreEmployes = await _dashboardService.NombreEmployesAsync();
            ChiffreAffaires = await _dashboardService.ChiffreAffairesAsync();
            ChiffreAffairesMois = await _dashboardService.ChiffreAffairesMoisAsync();
            BijouxStockFaible = await _dashboardService.BijouxStockFaibleAsync();
        }
        finally
        {
            EstChargement = false;
        }
    }
}
