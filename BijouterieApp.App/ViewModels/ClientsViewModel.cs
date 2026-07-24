using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.App.ViewModels;

public partial class ClientsViewModel : ObservableObject
{
    private readonly ClientService _clientService;

    public ClientsViewModel(ClientService clientService)
    {
        _clientService = clientService;
    }

    [ObservableProperty]
    private ObservableCollection<Client> _clients = new();

    [ObservableProperty]
    private ObservableCollection<Client> _clientsFiltres = new();

    [ObservableProperty]
    private string _recherche = string.Empty;

    [ObservableProperty]
    private bool _estEnEdition;

    [ObservableProperty]
    private Client? _clientSelectionne;

    [ObservableProperty]
    private string _nomSaisi = string.Empty;

    [ObservableProperty]
    private string _prenomSaisi = string.Empty;

    [ObservableProperty]
    private string? _telephoneSaisi;

    [ObservableProperty]
    private string? _adresseSaisie;

    [ObservableProperty]
    private string? _emailSaisi;

    [ObservableProperty]
    private bool _estChargement;

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private int? _editionId;

    private bool _estAjout;

    partial void OnRechercheChanged(string value)
    {
        Filtrer();
    }

    partial void OnClientSelectionneChanged(Client? value)
    {
        if (value != null && !EstEnEdition)
        {
            EditionId = value.Id;
            NomSaisi = value.Nom;
            PrenomSaisi = value.Prenom;
            TelephoneSaisi = value.Telephone;
            AdresseSaisie = value.Adresse;
            EmailSaisi = value.Email;
            _estAjout = false;
        }
    }

    [RelayCommand]
    private async Task ChargerClients()
    {
        EstChargement = true;
        MessageErreur = null;
        try
        {
            var liste = await _clientService.GetAllAsync();
            Clients = new ObservableCollection<Client>(liste);
            Filtrer();
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement des clients.";
        }
        finally
        {
            EstChargement = false;
        }
    }

    [RelayCommand]
    private void Nouveau()
    {
        EditionId = null;
        NomSaisi = string.Empty;
        PrenomSaisi = string.Empty;
        TelephoneSaisi = null;
        AdresseSaisie = null;
        EmailSaisi = null;
        _estAjout = true;
        EstEnEdition = true;
    }

    [RelayCommand]
    private void Modifier()
    {
        if (ClientSelectionne == null) return;
        _estAjout = false;
        EstEnEdition = true;
    }

    [RelayCommand]
    private async Task Supprimer()
    {
        if (ClientSelectionne == null) return;
        MessageErreur = null;
        var resultat = await _clientService.DeleteAsync(ClientSelectionne.Id);
        if (!resultat)
        {
            MessageErreur = "Impossible de supprimer : le client est lié à des ventes.";
            return;
        }
        await ChargerClients();
        Annuler();
    }

    [RelayCommand]
    private async Task Enregistrer()
    {
        MessageErreur = null;

        if (string.IsNullOrWhiteSpace(NomSaisi))
        {
            MessageErreur = "Le nom est obligatoire.";
            return;
        }
        if (string.IsNullOrWhiteSpace(PrenomSaisi))
        {
            MessageErreur = "Le prénom est obligatoire.";
            return;
        }
        if (!string.IsNullOrWhiteSpace(EmailSaisi) && !EmailSaisi.Contains("@"))
        {
            MessageErreur = "L'adresse email n'est pas valide.";
            return;
        }

        try
        {
            if (_estAjout)
            {
                await _clientService.CreateAsync(
                    NomSaisi.Trim(), PrenomSaisi.Trim(),
                    TelephoneSaisi?.Trim(), AdresseSaisie?.Trim(), EmailSaisi?.Trim());
            }
            else if (EditionId.HasValue)
            {
                await _clientService.UpdateAsync(
                    EditionId.Value,
                    NomSaisi.Trim(), PrenomSaisi.Trim(),
                    TelephoneSaisi?.Trim(), AdresseSaisie?.Trim(), EmailSaisi?.Trim());
            }
            Annuler();
            await ChargerClients();
        }
        catch
        {
            MessageErreur = "Erreur lors de l'enregistrement.";
        }
    }

    [RelayCommand]
    private void Annuler()
    {
        EstEnEdition = false;
        EditionId = null;
        NomSaisi = string.Empty;
        PrenomSaisi = string.Empty;
        TelephoneSaisi = null;
        AdresseSaisie = null;
        EmailSaisi = null;
        _estAjout = false;
        MessageErreur = null;
    }

    private void Filtrer()
    {
        if (string.IsNullOrWhiteSpace(Recherche))
        {
            ClientsFiltres = new ObservableCollection<Client>(Clients);
        }
        else
        {
            var filtre = Recherche.ToLower();
            ClientsFiltres = new ObservableCollection<Client>(
                Clients.Where(c =>
                    c.Nom.ToLower().Contains(filtre) ||
                    c.Prenom.ToLower().Contains(filtre) ||
                    c.NumeroClient.ToLower().Contains(filtre) ||
                    (c.Telephone != null && c.Telephone.ToLower().Contains(filtre)) ||
                    (c.Email != null && c.Email.ToLower().Contains(filtre)) ||
                    (c.Adresse != null && c.Adresse.ToLower().Contains(filtre))));
        }
    }
}
