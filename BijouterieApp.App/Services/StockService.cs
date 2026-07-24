using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Core.Entities;
using BijouterieApp.Core.Enums;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class StockService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public StockService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Bijou>> GetStockAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Bijoux
            .Include(b => b.Categorie)
            .OrderBy(b => b.Nom)
            .ToListAsync();
    }

    public async Task<List<MouvementStock>> GetMouvementsAsync(int? bijouId = null)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var query = context.MouvementsStock
            .Include(m => m.Bijou)
            .Include(m => m.Employe)
            .AsQueryable();
        if (bijouId.HasValue)
            query = query.Where(m => m.BijouId == bijouId.Value);
        return await query.OrderByDescending(m => m.Date).ToListAsync();
    }

    public async Task EntrerStockAsync(int bijouId, int quantite, int? employeId, string? commentaire)
    {
        if (quantite <= 0)
            throw new InvalidOperationException("La quantité doit être supérieure à zéro.");

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var bijou = await context.Bijoux.FindAsync(bijouId);
        if (bijou == null)
            throw new InvalidOperationException("Bijou introuvable.");

        bijou.QuantiteStock += quantite;
        context.MouvementsStock.Add(new MouvementStock
        {
            BijouId = bijouId,
            Type = TypeMouvementStock.Entree,
            Quantite = quantite,
            Date = DateTime.Now,
            EmployeId = employeId,
            Commentaire = commentaire
        });
        await context.SaveChangesAsync();
    }

    public async Task<bool> SortirStockAsync(int bijouId, int quantite, int? employeId, string? commentaire)
    {
        if (quantite <= 0)
            throw new InvalidOperationException("La quantité doit être supérieure à zéro.");

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var bijou = await context.Bijoux.FindAsync(bijouId);
        if (bijou == null) return false;
        if (bijou.QuantiteStock < quantite) return false;

        bijou.QuantiteStock -= quantite;
        context.MouvementsStock.Add(new MouvementStock
        {
            BijouId = bijouId,
            Type = TypeMouvementStock.Sortie,
            Quantite = quantite,
            Date = DateTime.Now,
            EmployeId = employeId,
            Commentaire = commentaire
        });
        await context.SaveChangesAsync();
        return true;
    }
}
