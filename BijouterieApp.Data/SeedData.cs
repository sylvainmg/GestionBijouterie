using Microsoft.EntityFrameworkCore;
using BijouterieApp.Core.Entities;
using BijouterieApp.Core.Enums;

namespace BijouterieApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(BijouterieDbContext context)
    {
        if (await context.Employes.AnyAsync())
            return;

        var admin = new Employe
        {
            Matricule = "ADM001",
            Nom = "Admin",
            Prenom = "Super",
            Login = "admin",
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = RoleEmploye.Administrateur,
            Fonction = "Administrateur système"
        };
        context.Employes.Add(admin);

        var categories = new List<Categorie>
        {
            new() { Nom = "Or", Description = "Bijoux en or" },
            new() { Nom = "Argent", Description = "Bijoux en argent" },
            new() { Nom = "Diamant", Description = "Bijoux avec diamants" },
            new() { Nom = "Bracelet", Description = "Bracelets" },
            new() { Nom = "Collier", Description = "Colliers" },
            new() { Nom = "Bague", Description = "Bagues" },
            new() { Nom = "Boucle d'oreille", Description = "Boucles d'oreilles" }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var bijoux = new List<Bijou>
        {
            new()
            {
                Reference = "OR-001", Nom = "Bague Or Jaune", CategorieId = categories[0].Id,
                Matiere = "Or 18K", PoidsGrammes = 5.2m, Prix = 850_000m, QuantiteStock = 10,
                Description = "Bague en or jaune 18 carats"
            },
            new()
            {
                Reference = "AR-001", Nom = "Collier Argent", CategorieId = categories[1].Id,
                Matiere = "Argent 925", PoidsGrammes = 12.5m, Prix = 120_000m, QuantiteStock = 15,
                Description = "Collier en argent massif 925"
            },
            new()
            {
                Reference = "DM-001", Nom = "Bague Diamant", CategorieId = categories[2].Id,
                Matiere = "Or blanc + diamant", PoidsGrammes = 3.8m, Prix = 2_500_000m, QuantiteStock = 3,
                Description = "Bague solitaire diamant taille brillant"
            },
            new()
            {
                Reference = "BR-001", Nom = "Bracelet Or", CategorieId = categories[3].Id,
                Matiere = "Or 18K", PoidsGrammes = 8.0m, Prix = 950_000m, QuantiteStock = 7,
                Description = "Bracelet gourmette or jaune"
            },
            new()
            {
                Reference = "CO-001", Nom = "Collier Perles", CategorieId = categories[4].Id,
                Matiere = "Perles de culture", PoidsGrammes = 15.0m, Prix = 450_000m, QuantiteStock = 5,
                Description = "Collier de perles de culture"
            }
        };
        context.Bijoux.AddRange(bijoux);
        await context.SaveChangesAsync();

        var client = new Client
        {
            NumeroClient = "CLI001",
            Nom = "Rakoto",
            Prenom = "Jean",
            Telephone = "+261 34 12 345 67",
            Adresse = "Lot IVT 123, Antananarivo",
            Email = "jean.rakoto@email.com"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();
    }
}
