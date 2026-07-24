namespace BijouterieApp.Core.Entities;

public class Vente
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int EmployeId { get; set; }
    public Employe Employe { get; set; } = null!;
    public decimal Total { get; set; }
    public decimal? Remise { get; set; }

    public ICollection<LigneVente> LigneVentes { get; set; } = new List<LigneVente>();
}
