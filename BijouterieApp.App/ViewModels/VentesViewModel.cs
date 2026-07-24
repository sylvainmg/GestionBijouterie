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

public partial class LigneVenteEdit : ObservableObject
{
    [ObservableProperty]
    private Bijou? _bijou;

    [ObservableProperty]
    private int _quantite = 1;

    public decimal PrixUnitaire => Bijou?.Prix ?? 0;
    public decimal SousTotal => Quantite * PrixUnitaire;
    public int StockDisponible => Bijou?.QuantiteStock ?? 0;
}

public partial class VentesViewModel : ObservableObject
{
    private readonly VenteService _venteService;
    private readonly ClientService _clientService;
    private readonly BijouService _bijouService;
    private readonly SessionManager _sessionManager;
    private readonly PdfService _pdfService;

    public VentesViewModel(
        VenteService venteService,
        ClientService clientService,
        BijouService bijouService,
        SessionManager sessionManager,
        PdfService pdfService)
    {
        _venteService = venteService;
        _clientService = clientService;
        _bijouService = bijouService;
        _sessionManager = sessionManager;
        _pdfService = pdfService;
    }

    [ObservableProperty]
    private ObservableCollection<Vente> _ventes = new();

    [ObservableProperty]
    private ObservableCollection<Client> _clients = new();

    [ObservableProperty]
    private ObservableCollection<Bijou> _bijouxDisponibles = new();

    [ObservableProperty]
    private ObservableCollection<LigneVenteEdit> _lignesEnEdition = new();

    [ObservableProperty]
    private Client? _clientSelectionne;

    [ObservableProperty]
    private Bijou? _bijouSelectionne;

    [ObservableProperty]
    private Vente? _venteSelectionnee;

    [ObservableProperty]
    private bool _estEnEdition;

    [ObservableProperty]
    private bool _estChargement;

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private decimal _remiseSaisie;

    [ObservableProperty]
    private decimal _totalVente;

    [ObservableProperty]
    private string _recherche = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Vente> _ventesFiltrees = new();

    partial void OnRechercheChanged(string value) => Filtrer();

    [RelayCommand]
    private async Task ChargerVentes()
    {
        EstChargement = true;
        MessageErreur = null;
        try
        {
            Clients = new ObservableCollection<Client>(await _clientService.GetAllAsync());
            var bijoux = await _bijouService.GetAllAsync();
            BijouxDisponibles = new ObservableCollection<Bijou>(bijoux.Where(b => b.QuantiteStock > 0));
            var liste = await _venteService.GetAllAsync();
            Ventes = new ObservableCollection<Vente>(liste);
            Filtrer();
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement des ventes.";
        }
        finally
        {
            EstChargement = false;
        }
    }

    [RelayCommand]
    private void NouvelleVente()
    {
        ClientSelectionne = null;
        LignesEnEdition.Clear();
        RemiseSaisie = 0;
        TotalVente = 0;
        MessageErreur = null;
        EstEnEdition = true;
    }

    [RelayCommand]
    private void AjouterLigne()
    {
        if (BijouSelectionne == null) return;

        var existante = LignesEnEdition.FirstOrDefault(l => l.Bijou?.Id == BijouSelectionne.Id);
        if (existante != null)
        {
            existante.Quantite++;
        }
        else
        {
            LignesEnEdition.Add(new LigneVenteEdit
            {
                Bijou = BijouSelectionne,
                Quantite = 1
            });
        }
        BijouSelectionne = null;
        CalculerTotal();
    }

    [RelayCommand]
    private void RetirerLigne(LigneVenteEdit? ligne)
    {
        if (ligne == null) return;
        LignesEnEdition.Remove(ligne);
        CalculerTotal();
    }

    private void CalculerTotal()
    {
        var total = LignesEnEdition.Sum(l => l.SousTotal);
        if (RemiseSaisie > 0 && RemiseSaisie < total)
            total -= RemiseSaisie;
        TotalVente = total;
    }

    partial void OnRemiseSaisieChanged(decimal value) => CalculerTotal();

    [RelayCommand]
    private async Task ValiderVente()
    {
        MessageErreur = null;

        if (ClientSelectionne == null)
        {
            MessageErreur = "Sélectionnez un client.";
            return;
        }
        if (LignesEnEdition.Count == 0)
        {
            MessageErreur = "Ajoutez au moins une ligne.";
            return;
        }

        foreach (var ligne in LignesEnEdition)
        {
            if (ligne.Quantite <= 0)
            {
                MessageErreur = $"Quantité invalide pour « {ligne.Bijou?.Nom} ».";
                return;
            }
            if (ligne.Bijou != null && ligne.Quantite > ligne.Bijou.QuantiteStock)
            {
                MessageErreur = $"Stock insuffisant pour « {ligne.Bijou.Nom} » (disponible : {ligne.Bijou.QuantiteStock}).";
                return;
            }
        }

        try
        {
            var lignes = LignesEnEdition
                .Select(l => (l.Bijou!.Id, l.Quantite, l.PrixUnitaire))
                .ToList();

            await _venteService.CreerVenteAsync(
                ClientSelectionne.Id,
                _sessionManager.UtilisateurCourant!.Id,
                lignes,
                RemiseSaisie > 0 ? RemiseSaisie : null);

            EstEnEdition = false;
            await ChargerVentes();
        }
        catch (Exception ex)
        {
            MessageErreur = ex.Message;
        }
    }

    [RelayCommand]
    private void Annuler()
    {
        EstEnEdition = false;
        ClientSelectionne = null;
        LignesEnEdition.Clear();
        RemiseSaisie = 0;
        TotalVente = 0;
        MessageErreur = null;
    }

    private void Filtrer()
    {
        if (string.IsNullOrWhiteSpace(Recherche))
        {
            VentesFiltrees = new ObservableCollection<Vente>(Ventes);
        }
        else
        {
            var filtre = Recherche.ToLower();
            VentesFiltrees = new ObservableCollection<Vente>(
                Ventes.Where(v =>
                    v.Client.Nom.ToLower().Contains(filtre) ||
                    v.Client.Prenom.ToLower().Contains(filtre) ||
                    v.Employe.Nom.ToLower().Contains(filtre) ||
                    v.Id.ToString().Contains(filtre)));
        }
    }

    [RelayCommand]
    private async Task GenererFacture()
    {
        if (VenteSelectionnee == null) return;
        MessageErreur = null;
        try
        {
            await _pdfService.GenererFactureAsync(VenteSelectionnee.Id);
        }
        catch (Exception ex)
        {
            MessageErreur = $"Erreur PDF : {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task GenererListeVentesPeriode()
    {
        MessageErreur = null;
        try
        {
            var debut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fin = debut.AddMonths(1).AddDays(-1);
            await _pdfService.GenererListeVentesAsync(debut, fin);
        }
        catch (Exception ex)
        {
            MessageErreur = $"Erreur PDF : {ex.Message}";
        }
    }
}
