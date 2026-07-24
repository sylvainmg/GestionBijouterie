namespace BijouterieApp.Core.Entities;

public class Client
{
    public int Id { get; set; }
    public string NumeroClient { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public string? Email { get; set; }

    public ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}
