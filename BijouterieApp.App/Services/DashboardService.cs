using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class DashboardService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DashboardService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<int> NombreBijouxAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Bijoux.CountAsync();
    }

    public async Task<int> NombreClientsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Clients.CountAsync();
    }

    public async Task<int> NombreVentesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Ventes.CountAsync();
    }

    public async Task<decimal> ChiffreAffairesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Ventes.SumAsync(v => v.Total);
    }

    public async Task<decimal> ChiffreAffairesMoisAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var debut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        return await context.Ventes
            .Where(v => v.Date >= debut)
            .SumAsync(v => v.Total);
    }

    public async Task<int> NombreEmployesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Employes.CountAsync();
    }

    public async Task<int> BijouxStockFaibleAsync(int seuil = 5)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Bijoux.CountAsync(b => b.QuantiteStock <= seuil);
    }
}
