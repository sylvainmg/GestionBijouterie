using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Core.Entities;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class BijouService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BijouService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Bijou>> GetAllAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Bijoux
            .Include(b => b.Categorie)
            .OrderBy(b => b.Nom)
            .ToListAsync();
    }

    public async Task<Bijou?> GetByIdAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Bijoux
            .Include(b => b.Categorie)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bijou> CreateAsync(
        string reference, string nom, int categorieId,
        string? matiere, decimal? poidsGrammes, decimal prix,
        int quantiteStock, string? description, string? photoPath)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var bijou = new Bijou
        {
            Reference = reference,
            Nom = nom,
            CategorieId = categorieId,
            Matiere = matiere,
            PoidsGrammes = poidsGrammes,
            Prix = prix,
            QuantiteStock = quantiteStock,
            Description = description,
            PhotoPath = photoPath
        };
        context.Bijoux.Add(bijou);
        await context.SaveChangesAsync();
        return bijou;
    }

    public async Task UpdateAsync(
        int id, string reference, string nom, int categorieId,
        string? matiere, decimal? poidsGrammes, decimal prix,
        int quantiteStock, string? description, string? photoPath)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var bijou = await context.Bijoux.FindAsync(id);
        if (bijou == null)
            throw new InvalidOperationException("Bijou introuvable.");
        bijou.Reference = reference;
        bijou.Nom = nom;
        bijou.CategorieId = categorieId;
        bijou.Matiere = matiere;
        bijou.PoidsGrammes = poidsGrammes;
        bijou.Prix = prix;
        bijou.QuantiteStock = quantiteStock;
        bijou.Description = description;
        bijou.PhotoPath = photoPath;
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var bijou = await context.Bijoux
            .Include(b => b.LigneVentes)
            .Include(b => b.MouvementsStock)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (bijou == null) return false;
        if (bijou.LigneVentes.Count > 0 || bijou.MouvementsStock.Count > 0) return false;
        context.Bijoux.Remove(bijou);
        await context.SaveChangesAsync();
        return true;
    }
}
