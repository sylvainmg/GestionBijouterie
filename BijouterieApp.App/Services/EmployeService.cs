using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BijouterieApp.Core.Entities;
using BijouterieApp.Core.Enums;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class EmployeService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EmployeService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Employe>> GetAllAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Employes.OrderBy(e => e.Nom).ToListAsync();
    }

    public async Task<Employe?> GetByIdAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        return await context.Employes.FindAsync(id);
    }

    public async Task<Employe> CreateAsync(
        string nom, string prenom, string? telephone,
        string? adresse, string? fonction, string login,
        string motDePasse, RoleEmploye role)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();

        if (await context.Employes.AnyAsync(e => e.Login == login))
            throw new InvalidOperationException("Ce login existe déjà.");

        var matricule = await GenererMatriculeAsync(context, role);
        var employe = new Employe
        {
            Matricule = matricule,
            Nom = nom,
            Prenom = prenom,
            Telephone = telephone,
            Adresse = adresse,
            Fonction = fonction,
            Login = login,
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(motDePasse),
            Role = role
        };
        context.Employes.Add(employe);
        await context.SaveChangesAsync();
        return employe;
    }

    public async Task UpdateAsync(
        int id, string nom, string prenom, string? telephone,
        string? adresse, string? fonction, string login,
        string? motDePasse, RoleEmploye role)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var employe = await context.Employes.FindAsync(id);
        if (employe == null)
            throw new InvalidOperationException("Employé introuvable.");

        if (await context.Employes.AnyAsync(e => e.Login == login && e.Id != id))
            throw new InvalidOperationException("Ce login est déjà utilisé par un autre employé.");

        employe.Nom = nom;
        employe.Prenom = prenom;
        employe.Telephone = telephone;
        employe.Adresse = adresse;
        employe.Fonction = fonction;
        employe.Login = login;
        employe.Role = role;
        if (!string.IsNullOrWhiteSpace(motDePasse))
            employe.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(motDePasse);
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        var employe = await context.Employes
            .Include(e => e.Ventes)
            .Include(e => e.MouvementsStock)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (employe == null) return false;
        if (employe.Ventes.Count > 0 || employe.MouvementsStock.Count > 0) return false;
        context.Employes.Remove(employe);
        await context.SaveChangesAsync();
        return true;
    }

    private static async Task<string> GenererMatriculeAsync(BijouterieDbContext context, RoleEmploye role)
    {
        var prefixe = role == RoleEmploye.Administrateur ? "ADM" : "CAI";
        var dernierMatricule = await context.Employes
            .Where(e => e.Role == role)
            .Select(e => e.Matricule)
            .OrderByDescending(m => m)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(dernierMatricule))
            return $"{prefixe}001";

        var match = Regex.Match(dernierMatricule, @"[A-Z]+(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var num))
            return $"{prefixe}{(num + 1):D3}";

        return $"{prefixe}{context.Employes.Count() + 1:D3}";
    }
}
