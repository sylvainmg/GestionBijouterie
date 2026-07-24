namespace BijouterieApp.Core.Entities;

public class LigneVente
{
    public int Id { get; set; }
    public int VenteId { get; set; }
    public Vente Vente { get; set; } = null!;
    public int BijouId { get; set; }
    public Bijou Bijou { get; set; } = null!;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal Total => Quantite * PrixUnitaire;
}
