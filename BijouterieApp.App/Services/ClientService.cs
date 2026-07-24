using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Core.Entities;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class ClientService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ClientService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Client>> GetAllAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Clients.OrderBy(c => c.Nom).ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Clients.FindAsync(id);
    }

    public async Task<Client> CreateAsync(
        string nom, string prenom, string? telephone,
        string? adresse, string? email)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var numeroClient = await GenererNumeroClientAsync(context);
        var client = new Client
        {
            NumeroClient = numeroClient,
            Nom = nom,
            Prenom = prenom,
            Telephone = telephone,
            Adresse = adresse,
            Email = email
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();
        return client;
    }

    public async Task UpdateAsync(
        int id, string nom, string prenom, string? telephone,
        string? adresse, string? email)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var client = await context.Clients.FindAsync(id);
        if (client == null)
            throw new InvalidOperationException("Client introuvable.");
        client.Nom = nom;
        client.Prenom = prenom;
        client.Telephone = telephone;
        client.Adresse = adresse;
        client.Email = email;
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var client = await context.Clients
            .Include(c => c.Ventes)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null) return false;
        if (client.Ventes.Count > 0) return false;
        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return true;
    }

    private static async Task<string> GenererNumeroClientAsync(BijouterieDbContext context)
    {
        var dernierNumero = await context.Clients
            .Select(c => c.NumeroClient)
            .OrderByDescending(n => n)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(dernierNumero))
            return "CLI001";

        var match = Regex.Match(dernierNumero, @"CLI(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var num))
            return $"CLI{(num + 1):D3}";

        return $"CLI{context.Clients.Count() + 1:D3}";
    }
}
