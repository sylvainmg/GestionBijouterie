using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using BijouterieApp.Core.Entities;
using BijouterieApp.Core.Enums;

namespace BijouterieApp.App.ViewModels;

public partial class EmployesViewModel : ObservableObject
{
    private readonly EmployeService _employeService;

    public EmployesViewModel(EmployeService employeService)
    {
        _employeService = employeService;
    }

    [ObservableProperty]
    private ObservableCollection<Employe> _employes = new();

    [ObservableProperty]
    private ObservableCollection<Employe> _employesFiltres = new();

    [ObservableProperty]
    private string _recherche = string.Empty;

    [ObservableProperty]
    private bool _estEnEdition;

    [ObservableProperty]
    private Employe? _employeSelectionne;

    [ObservableProperty]
    private string _nomSaisi = string.Empty;

    [ObservableProperty]
    private string _prenomSaisi = string.Empty;

    [ObservableProperty]
    private string? _telephoneSaisi;

    [ObservableProperty]
    private string? _adresseSaisie;

    [ObservableProperty]
    private string? _fonctionSaisie;

    [ObservableProperty]
    private string _loginSaisi = string.Empty;

    [ObservableProperty]
    private string _motDePasseSaisi = string.Empty;

    [ObservableProperty]
    private RoleEmploye _roleSelectionne = RoleEmploye.Caissier;

    [ObservableProperty]
    private bool _estChargement;

    [ObservableProperty]
    private string? _messageErreur;

    [ObservableProperty]
    private int? _editionId;

    private bool _estAjout;

    partial void OnRechercheChanged(string value) => Filtrer();

    partial void OnEmployeSelectionneChanged(Employe? value)
    {
        if (value != null && !EstEnEdition)
        {
            EditionId = value.Id;
            NomSaisi = value.Nom;
            PrenomSaisi = value.Prenom;
            TelephoneSaisi = value.Telephone;
            AdresseSaisie = value.Adresse;
            FonctionSaisie = value.Fonction;
            LoginSaisi = value.Login;
            MotDePasseSaisi = string.Empty;
            RoleSelectionne = value.Role;
            _estAjout = false;
        }
    }

    [RelayCommand]
    private async Task ChargerEmployes()
    {
        EstChargement = true;
        MessageErreur = null;
        try
        {
            var liste = await _employeService.GetAllAsync();
            Employes = new ObservableCollection<Employe>(liste);
            Filtrer();
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement des employés.";
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
        FonctionSaisie = null;
        LoginSaisi = string.Empty;
        MotDePasseSaisi = string.Empty;
        RoleSelectionne = RoleEmploye.Caissier;
        _estAjout = true;
        EstEnEdition = true;
    }

    [RelayCommand]
    private void Modifier()
    {
        if (EmployeSelectionne == null) return;
        _estAjout = false;
        EstEnEdition = true;
    }

    [RelayCommand]
    private async Task Supprimer()
    {
        if (EmployeSelectionne == null) return;
        MessageErreur = null;
        var resultat = await _employeService.DeleteAsync(EmployeSelectionne.Id);
        if (!resultat)
        {
            MessageErreur = "Impossible de supprimer : l'employé est lié à des ventes ou mouvements.";
            return;
        }
        await ChargerEmployes();
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
        if (string.IsNullOrWhiteSpace(LoginSaisi))
        {
            MessageErreur = "Le login est obligatoire.";
            return;
        }
        if (_estAjout && string.IsNullOrWhiteSpace(MotDePasseSaisi))
        {
            MessageErreur = "Le mot de passe est obligatoire à la création.";
            return;
        }

        try
        {
            if (_estAjout)
            {
                await _employeService.CreateAsync(
                    NomSaisi.Trim(), PrenomSaisi.Trim(), TelephoneSaisi?.Trim(),
                    AdresseSaisie?.Trim(), FonctionSaisie?.Trim(),
                    LoginSaisi.Trim(), MotDePasseSaisi, RoleSelectionne);
            }
            else if (EditionId.HasValue)
            {
                await _employeService.UpdateAsync(
                    EditionId.Value,
                    NomSaisi.Trim(), PrenomSaisi.Trim(), TelephoneSaisi?.Trim(),
                    AdresseSaisie?.Trim(), FonctionSaisie?.Trim(),
                    LoginSaisi.Trim(), MotDePasseSaisi, RoleSelectionne);
            }
            Annuler();
            await ChargerEmployes();
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
        EditionId = null;
        NomSaisi = string.Empty;
        PrenomSaisi = string.Empty;
        TelephoneSaisi = null;
        AdresseSaisie = null;
        FonctionSaisie = null;
        LoginSaisi = string.Empty;
        MotDePasseSaisi = string.Empty;
        RoleSelectionne = RoleEmploye.Caissier;
        _estAjout = false;
        MessageErreur = null;
    }

    private void Filtrer()
    {
        if (string.IsNullOrWhiteSpace(Recherche))
        {
            EmployesFiltres = new ObservableCollection<Employe>(Employes);
        }
        else
        {
            var filtre = Recherche.ToLower();
            EmployesFiltres = new ObservableCollection<Employe>(
                Employes.Where(e =>
                    e.Nom.ToLower().Contains(filtre) ||
                    e.Prenom.ToLower().Contains(filtre) ||
                    e.Matricule.ToLower().Contains(filtre) ||
                    e.Login.ToLower().Contains(filtre) ||
                    (e.Fonction != null && e.Fonction.ToLower().Contains(filtre))));
        }
    }
}
