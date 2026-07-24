namespace BijouterieApp.Core.Entities;

public class Categorie
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Bijou> Bijoux { get; set; } = new List<Bijou>();
}
