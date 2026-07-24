using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.App.ViewModels;

public partial class StockViewModel : ObservableObject
{
    private readonly StockService _stockService;
    private readonly SessionManager _sessionManager;

    public StockViewModel(StockService stockService, SessionManager sessionManager)
    {
        _stockService = stockService;
        _sessionManager = sessionManager;
    }

    [ObservableProperty]
    private ObservableCollection<Bijou> _bijouxStock = new();

    [ObservableProperty]
    private ObservableCollection<Bijou> _bijouxFiltres = new();

    [ObservableProperty]
    private ObservableCollection<MouvementStock> _mouvements = new();

    [ObservableProperty]
    private string _recherche = string.Empty;

    [ObservableProperty]
    private Bijou? _bijouSelectionne;

    [ObservableProperty]
    private int _quantiteEntree = 1;

    [ObservableProperty]
    private string? _commentaireEntree;

    [ObservableProperty]
    private bool _estChargement;

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private string? _messageSucces;

    [ObservableProperty]
    private int _seuilAlerte = 5;

    [ObservableProperty]
    private bool _afficherAlertes;

    partial void OnRechercheChanged(string value) => Filtrer();
    partial void OnAfficherAlertesChanged(bool value) => Filtrer();

    [RelayCommand]
    private async Task ChargerStock()
    {
        EstChargement = true;
        MessageErreur = null;
        try
        {
            var liste = await _stockService.GetStockAsync();
            BijouxStock = new ObservableCollection<Bijou>(liste);
            Filtrer();
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement du stock.";
        }
        finally
        {
            EstChargement = false;
        }
    }

    [RelayCommand]
    private async Task ChargerMouvements()
    {
        if (BijouSelectionne == null) return;
        try
        {
            var liste = await _stockService.GetMouvementsAsync(BijouSelectionne.Id);
            Mouvements = new ObservableCollection<MouvementStock>(liste);
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement des mouvements.";
        }
    }

    [RelayCommand]
    private async Task EntrerStock()
    {
        MessageErreur = null;
        MessageSucces = null;
        if (BijouSelectionne == null)
        {
            MessageErreur = "Sélectionnez un bijou.";
            return;
        }
        if (QuantiteEntree <= 0)
        {
            MessageErreur = "La quantité doit être supérieure à zéro.";
            return;
        }

        try
        {
            await _stockService.EntrerStockAsync(
                BijouSelectionne.Id, QuantiteEntree,
                _sessionManager.UtilisateurCourant?.Id, CommentaireEntree?.Trim());
            MessageSucces = $"{QuantiteEntree} unité(s) ajoutée(s) au stock de « {BijouSelectionne.Nom} ».";
            QuantiteEntree = 1;
            CommentaireEntree = null;
            await ChargerStock();
            await ChargerMouvements();
        }
        catch (Exception ex)
        {
            MessageErreur = ex.Message;
        }
    }

    private void Filtrer()
    {
        IEnumerable<Bijou> source = BijouxStock;

        if (AfficherAlertes)
            source = source.Where(b => b.QuantiteStock <= SeuilAlerte);

        if (!string.IsNullOrWhiteSpace(Recherche))
        {
            var filtre = Recherche.ToLower();
            source = source.Where(b =>
                b.Nom.ToLower().Contains(filtre) ||
                b.Reference.ToLower().Contains(filtre) ||
                b.Categorie.Nom.ToLower().Contains(filtre));
        }

        BijouxFiltres = new ObservableCollection<Bijou>(source);
    }
}
