using Xunit;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.Tests;

public class LigneVenteTests
{
    [Fact]
    public void Total_CalculeCorrectement()
    {
        var ligne = new LigneVente
        {
            Quantite = 3,
            PrixUnitaire = 150_000m
        };
        Assert.Equal(450_000m, ligne.Total);
    }

    [Fact]
    public void Total_QuantiteUn_ReturnePrixUnitaire()
    {
        var ligne = new LigneVente
        {
            Quantite = 1,
            PrixUnitaire = 850_000m
        };
        Assert.Equal(850_000m, ligne.Total);
    }

    [Fact]
    public void Total_QuantiteZero_EstZero()
    {
        var ligne = new LigneVente
        {
            Quantite = 0,
            PrixUnitaire = 100_000m
        };
        Assert.Equal(0m, ligne.Total);
    }

    [Fact]
    public void Total_GrandeQuantite_CalculeCorrectement()
    {
        var ligne = new LigneVente
        {
            Quantite = 100,
            PrixUnitaire = 2_500_000m
        };
        Assert.Equal(250_000_000m, ligne.Total);
    }

    [Fact]
    public void Total_PrixDecimal_CalculeCorrectement()
    {
        var ligne = new LigneVente
        {
            Quantite = 2,
            PrixUnitaire = 120_500.50m
        };
        Assert.Equal(241_001.00m, ligne.Total);
    }
}
