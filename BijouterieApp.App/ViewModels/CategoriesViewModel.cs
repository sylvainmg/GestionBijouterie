using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.App.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly CategorieService _categorieService;

    public CategoriesViewModel(CategorieService categorieService)
    {
        _categorieService = categorieService;
    }

    [ObservableProperty]
    private ObservableCollection<Categorie> _categories = new();

    [ObservableProperty]
    private ObservableCollection<Categorie> _categoriesFiltrees = new();

    [ObservableProperty]
    private string _recherche = string.Empty;

    [ObservableProperty]
    private bool _estEnEdition;

    [ObservableProperty]
    private Categorie? _categorieSelectionnee;

    [ObservableProperty]
    private string _nomSaisi = string.Empty;

    [ObservableProperty]
    private string? _descriptionSaisie;

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

    partial void OnCategorieSelectionneeChanged(Categorie? value)
    {
        if (value != null && !EstEnEdition)
        {
            EditionId = value.Id;
            NomSaisi = value.Nom;
            DescriptionSaisie = value.Description;
            _estAjout = false;
        }
    }

    [RelayCommand]
    private async Task ChargerCategories()
    {
        EstChargement = true;
        MessageErreur = null;
        try
        {
            var listes = await _categorieService.GetAllAsync();
            Categories = new ObservableCollection<Categorie>(listes);
            Filtrer();
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement des catégories.";
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
        DescriptionSaisie = null;
        _estAjout = true;
        EstEnEdition = true;
    }

    [RelayCommand]
    private void Modifier()
    {
        if (CategorieSelectionnee == null) return;
        _estAjout = false;
        EstEnEdition = true;
    }

    [RelayCommand]
    private async Task Supprimer()
    {
        if (CategorieSelectionnee == null) return;
        MessageErreur = null;
        var resultat = await _categorieService.DeleteAsync(CategorieSelectionnee.Id);
        if (!resultat)
        {
            MessageErreur = "Impossible de supprimer : des bijoux sont liés à cette catégorie.";
            return;
        }
        await ChargerCategories();
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

        try
        {
            if (_estAjout)
            {
                await _categorieService.CreateAsync(NomSaisi.Trim(), DescriptionSaisie?.Trim());
            }
            else if (EditionId.HasValue)
            {
                await _categorieService.UpdateAsync(EditionId.Value, NomSaisi.Trim(), DescriptionSaisie?.Trim());
            }
            Annuler();
            await ChargerCategories();
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
        DescriptionSaisie = null;
        _estAjout = false;
        MessageErreur = null;
    }

    private void Filtrer()
    {
        if (string.IsNullOrWhiteSpace(Recherche))
        {
            CategoriesFiltrees = new ObservableCollection<Categorie>(Categories);
        }
        else
        {
            var filtre = Recherche.ToLower();
            CategoriesFiltrees = new ObservableCollection<Categorie>(
                Categories.Where(c => c.Nom.ToLower().Contains(filtre)));
        }
    }
}