namespace BijouterieApp.Core.Entities;

public class Bijou
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public int CategorieId { get; set; }
    public Categorie Categorie { get; set; } = null!;
    public string? Matiere { get; set; }
    public decimal? PoidsGrammes { get; set; }
    public decimal Prix { get; set; }
    public int QuantiteStock { get; set; }
    public string? Description { get; set; }
    public string? PhotoPath { get; set; }

    public ICollection<LigneVente> LigneVentes { get; set; } = new List<LigneVente>();
    public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();
}
