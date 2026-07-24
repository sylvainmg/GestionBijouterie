using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Core.Entities;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class CategorieService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CategorieService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Categorie>> GetAllAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Categories.OrderBy(c => c.Nom).ToListAsync();
    }

    public async Task<Categorie?> GetByIdAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Categories.FindAsync(id);
    }

    public async Task<Categorie> CreateAsync(string nom, string? description)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var categorie = new Categorie { Nom = nom, Description = description };
        context.Categories.Add(categorie);
        await context.SaveChangesAsync();
        return categorie;
    }

    public async Task UpdateAsync(int id, string nom, string? description)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var categorie = await context.Categories.FindAsync(id);
        if (categorie == null)
            throw new InvalidOperationException("Catégorie introuvable.");
        categorie.Nom = nom;
        categorie.Description = description;
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var categorie = await context.Categories
            .Include(c => c.Bijoux)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (categorie == null) return false;
        if (categorie.Bijoux.Count > 0) return false;
        context.Categories.Remove(categorie);
        await context.SaveChangesAsync();
        return true;
    }
}