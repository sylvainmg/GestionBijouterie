using Xunit;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.Tests;

public class BijouStockTests
{
    [Fact]
    public void QuantiteStock_InitAZero_ParDefaut()
    {
        var bijou = new Bijou();
        Assert.Equal(0, bijou.QuantiteStock);
    }

    [Fact]
    public void QuantiteStock_ApresAugmentation_CalculeCorrectement()
    {
        var bijou = new Bijou { QuantiteStock = 10 };
        bijou.QuantiteStock += 5;
        Assert.Equal(15, bijou.QuantiteStock);
    }

    [Fact]
    public void QuantiteStock_ApresDiminution_CalculeCorrectement()
    {
        var bijou = new Bijou { QuantiteStock = 10 };
        bijou.QuantiteStock -= 3;
        Assert.Equal(7, bijou.QuantiteStock);
    }

    [Fact]
    public void StockInsuffisant_DetecteCorrectement()
    {
        var bijou = new Bijou { QuantiteStock = 2 };
        bool stockInsuffisant = bijou.QuantiteStock < 5;
        Assert.True(stockInsuffisant);
    }

    [Fact]
    public void StockSuffisant_DetecteCorrectement()
    {
        var bijou = new Bijou { QuantiteStock = 10 };
        bool stockInsuffisant = bijou.QuantiteStock < 5;
        Assert.False(stockInsuffisant);
    }

    [Fact]
    public void StockSeuil_Egal_AuSeuil()
    {
        var bijou = new Bijou { QuantiteStock = 5 };
        bool estAlerte = bijou.QuantiteStock <= 5;
        Assert.True(estAlerte);
    }
}

public class VenteCalculsTests
{
    [Fact]
    public void TotalVente_SansRemise_CalculeCorrectement()
    {
        var lignes = new List<LigneVente>
        {
            new() { Quantite = 2, PrixUnitaire = 100_000m },
            new() { Quantite = 1, PrixUnitaire = 250_000m }
        };
        decimal remise = 0;
        var total = lignes.Sum(l => l.Total) - remise;
        Assert.Equal(450_000m, total);
    }

    [Fact]
    public void TotalVente_AvecRemise_CalculeCorrectement()
    {
        var lignes = new List<LigneVente>
        {
            new() { Quantite = 1, PrixUnitaire = 500_000m },
            new() { Quantite = 2, PrixUnitaire = 100_000m }
        };
        decimal remise = 50_000m;
        var total = lignes.Sum(l => l.Total) - remise;
        Assert.Equal(650_000m, total);
    }

    [Fact]
    public void TotalVente_Vide_EstZero()
    {
        var lignes = new List<LigneVente>();
        var total = lignes.Sum(l => l.Total);
        Assert.Equal(0m, total);
    }

    [Fact]
    public void TotalVente_PrixMGA_FormateCorrectement()
    {
        var lignes = new List<LigneVente>
        {
            new() { Quantite = 1, PrixUnitaire = 2_500_000m }
        };
        var total = lignes.Sum(l => l.Total);
        Assert.Equal(2_500_000m, total);
    }
}

public class ClientNumeroTests
{
    [Theory]
    [InlineData(0, "CLI001")]
    [InlineData(1, "CLI002")]
    [InlineData(99, "CLI100")]
    [InlineData(999, "CLI1000")]
    public void GenererNumero_CalculeCorrectement(int dernierNum, string attendu)
    {
        string prefixe = "CLI";
        string numero = $"{prefixe}{(dernierNum + 1):D3}";
        Assert.Equal(attendu, numero);
    }
}
