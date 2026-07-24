using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.App.ViewModels;

public partial class BijouxViewModel : ObservableObject
{
    private readonly BijouService _bijouService;
    private readonly CategorieService _categorieService;
    private readonly PdfService _pdfService;

    public BijouxViewModel(BijouService bijouService, CategorieService categorieService, PdfService pdfService)
    {
        _bijouService = bijouService;
        _categorieService = categorieService;
        _pdfService = pdfService;
    }

    [ObservableProperty]
    private ObservableCollection<Bijou> _bijoux = new();

    [ObservableProperty]
    private ObservableCollection<Bijou> _bijouxFiltres = new();

    [ObservableProperty]
    private ObservableCollection<Categorie> _categories = new();

    [ObservableProperty]
    private string _recherche = string.Empty;

    [ObservableProperty]
    private bool _estEnEdition;

    [ObservableProperty]
    private Bijou? _bijouSelectionne;

    [ObservableProperty]
    private string _referenceSaisie = string.Empty;

    [ObservableProperty]
    private string _nomSaisi = string.Empty;

    [ObservableProperty]
    private Categorie? _categorieSelectionnee;

    [ObservableProperty]
    private string? _matiereSaisie;

    [ObservableProperty]
    private string? _poidsSaisi;

    [ObservableProperty]
    private string _prixSaisi = string.Empty;

    [ObservableProperty]
    private string _quantiteStockSaisie = string.Empty;

    [ObservableProperty]
    private string? _descriptionSaisie;

    [ObservableProperty]
    private string? _photoPathSaisi;

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

    partial void OnBijouSelectionneChanged(Bijou? value)
    {
        if (value != null && !EstEnEdition)
        {
            EditionId = value.Id;
            ReferenceSaisie = value.Reference;
            NomSaisi = value.Nom;
            CategorieSelectionnee = Categories.FirstOrDefault(c => c.Id == value.CategorieId);
            MatiereSaisie = value.Matiere;
            PoidsSaisi = value.PoidsGrammes?.ToString("F2");
            PrixSaisi = value.Prix.ToString("F0");
            QuantiteStockSaisie = value.QuantiteStock.ToString();
            DescriptionSaisie = value.Description;
            PhotoPathSaisi = value.PhotoPath;
            _estAjout = false;
        }
    }

    [RelayCommand]
    private async Task ChargerBijoux()
    {
        EstChargement = true;
        MessageErreur = null;
        try
        {
            Categories = new ObservableCollection<Categorie>(await _categorieService.GetAllAsync());
            var listes = await _bijouService.GetAllAsync();
            Bijoux = new ObservableCollection<Bijou>(listes);
            Filtrer();
        }
        catch
        {
            MessageErreur = "Erreur lors du chargement des bijoux.";
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
        ReferenceSaisie = string.Empty;
        NomSaisi = string.Empty;
        CategorieSelectionnee = null;
        MatiereSaisie = null;
        PoidsSaisi = null;
        PrixSaisi = string.Empty;
        QuantiteStockSaisie = "0";
        DescriptionSaisie = null;
        PhotoPathSaisi = null;
        _estAjout = true;
        EstEnEdition = true;
    }

    [RelayCommand]
    private void Modifier()
    {
        if (BijouSelectionne == null) return;
        _estAjout = false;
        EstEnEdition = true;
    }

    [RelayCommand]
    private async Task Supprimer()
    {
        if (BijouSelectionne == null) return;
        MessageErreur = null;
        var resultat = await _bijouService.DeleteAsync(BijouSelectionne.Id);
        if (!resultat)
        {
            MessageErreur = "Impossible de supprimer : le bijou est lié à des ventes ou mouvements de stock.";
            return;
        }
        await ChargerBijoux();
        Annuler();
    }

    [RelayCommand]
    private async Task Enregistrer()
    {
        MessageErreur = null;

        if (string.IsNullOrWhiteSpace(ReferenceSaisie))
        {
            MessageErreur = "La référence est obligatoire.";
            return;
        }
        if (string.IsNullOrWhiteSpace(NomSaisi))
        {
            MessageErreur = "Le nom est obligatoire.";
            return;
        }
        if (CategorieSelectionnee == null)
        {
            MessageErreur = "La catégorie est obligatoire.";
            return;
        }
        if (!decimal.TryParse(PrixSaisi, out var prix) || prix < 0)
        {
            MessageErreur = "Le prix doit être un nombre positif.";
            return;
        }
        if (!int.TryParse(QuantiteStockSaisie, out var quantite) || quantite < 0)
        {
            MessageErreur = "La quantité en stock doit être un entier positif.";
            return;
        }
        decimal? poids = null;
        if (!string.IsNullOrWhiteSpace(PoidsSaisi))
        {
            if (!decimal.TryParse(PoidsSaisi, out var p) || p < 0)
            {
                MessageErreur = "Le poids doit être un nombre positif.";
                return;
            }
            poids = p;
        }

        try
        {
            if (_estAjout)
            {
                await _bijouService.CreateAsync(
                    ReferenceSaisie.Trim(), NomSaisi.Trim(), CategorieSelectionnee.Id,
                    MatiereSaisie?.Trim(), poids, prix,
                    quantite, DescriptionSaisie?.Trim(), PhotoPathSaisi);
            }
            else if (EditionId.HasValue)
            {
                await _bijouService.UpdateAsync(
                    EditionId.Value, ReferenceSaisie.Trim(), NomSaisi.Trim(), CategorieSelectionnee.Id,
                    MatiereSaisie?.Trim(), poids, prix,
                    quantite, DescriptionSaisie?.Trim(), PhotoPathSaisi);
            }
            Annuler();
            await ChargerBijoux();
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
        ReferenceSaisie = string.Empty;
        NomSaisi = string.Empty;
        CategorieSelectionnee = null;
        MatiereSaisie = null;
        PoidsSaisi = null;
        PrixSaisi = string.Empty;
        QuantiteStockSaisie = "0";
        DescriptionSaisie = null;
        PhotoPathSaisi = null;
        _estAjout = false;
        MessageErreur = null;
    }

    [RelayCommand]
    private async Task ChoisirPhoto()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choisir une photo du bijou",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Images") { Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.webp" } }
            }
        });

        if (files.Count > 0)
        {
            var sourcePath = files[0].Path.LocalPath;
            var photosDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Photos");
            if (!Directory.Exists(photosDir))
                Directory.CreateDirectory(photosDir);

            var ext = Path.GetExtension(sourcePath);
            var fileName = $"bijou_{DateTime.Now:yyyyMMddHHmmss}{ext}";
            var destPath = Path.Combine(photosDir, fileName);
            File.Copy(sourcePath, destPath, true);

            PhotoPathSaisi = Path.Combine("Assets", "Photos", fileName);
        }
    }

    private void Filtrer()
    {
        if (string.IsNullOrWhiteSpace(Recherche))
        {
            BijouxFiltres = new ObservableCollection<Bijou>(Bijoux);
        }
        else
        {
            var filtre = Recherche.ToLower();
            BijouxFiltres = new ObservableCollection<Bijou>(
                Bijoux.Where(b =>
                    b.Nom.ToLower().Contains(filtre) ||
                    b.Reference.ToLower().Contains(filtre) ||
                    (b.Matiere != null && b.Matiere.ToLower().Contains(filtre)) ||
                    (b.Description != null && b.Description.ToLower().Contains(filtre)) ||
                    b.Categorie.Nom.ToLower().Contains(filtre)));
        }
    }

    [RelayCommand]
    private async Task GenererListeBijoux()
    {
        MessageErreur = null;
        try
        {
            await _pdfService.GenererListeBijouxAsync();
        }
        catch (Exception ex)
        {
            MessageErreur = $"Erreur PDF : {ex.Message}";
        }
    }
}
