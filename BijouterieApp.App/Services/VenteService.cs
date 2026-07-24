using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Core.Entities;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class VenteService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public VenteService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Vente>> GetAllAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Employe)
            .Include(v => v.LigneVentes).ThenInclude(l => l.Bijou)
            .OrderByDescending(v => v.Date)
            .ToListAsync();
    }

    public async Task<Vente?> GetByIdAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Employe)
            .Include(v => v.LigneVentes).ThenInclude(l => l.Bijou)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Vente>> GetByPeriodeAsync(DateTime debut, DateTime fin)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Employe)
            .Include(v => v.LigneVentes).ThenInclude(l => l.Bijou)
            .Where(v => v.Date >= debut && v.Date <= fin)
            .OrderByDescending(v => v.Date)
            .ToListAsync();
    }

    public async Task<Vente> CreerVenteAsync(
        int clientId, int employeId,
        List<(int BijouId, int Quantite, decimal PrixUnitaire)> lignes,
        decimal? remise)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();

        foreach (var ligne in lignes)
        {
            var bijou = await context.Bijoux.FindAsync(ligne.BijouId);
            if (bijou == null)
                throw new InvalidOperationException($"Bijou ID {ligne.BijouId} introuvable.");
            if (bijou.QuantiteStock < ligne.Quantite)
                throw new InvalidOperationException(
                    $"Stock insuffisant pour « {bijou.Nom} » (disponible : {bijou.QuantiteStock}, demandé : {ligne.Quantite}).");
        }

        var total = lignes.Sum(l => l.Quantite * l.PrixUnitaire);
        if (remise.HasValue && remise.Value > 0)
            total -= remise.Value;

        var vente = new Vente
        {
            Date = DateTime.Now,
            ClientId = clientId,
            EmployeId = employeId,
            Total = total,
            Remise = remise
        };

        foreach (var ligne in lignes)
        {
            vente.LigneVentes.Add(new LigneVente
            {
                BijouId = ligne.BijouId,
                Quantite = ligne.Quantite,
                PrixUnitaire = ligne.PrixUnitaire
            });

            var bijou = await context.Bijoux.FindAsync(ligne.BijouId);
            if (bijou != null)
                bijou.QuantiteStock -= ligne.Quantite;
        }

        context.Ventes.Add(vente);
        await context.SaveChangesAsync();
        return await GetByIdAsync(vente.Id) ?? vente;
    }
}
