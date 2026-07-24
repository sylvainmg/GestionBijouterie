using BijouterieApp.Core.Enums;

namespace BijouterieApp.Core.Entities;

public class MouvementStock
{
    public int Id { get; set; }
    public int BijouId { get; set; }
    public Bijou Bijou { get; set; } = null!;
    public TypeMouvementStock Type { get; set; }
    public int Quantite { get; set; }
    public DateTime Date { get; set; }
    public int? EmployeId { get; set; }
    public Employe? Employe { get; set; }
    public string? Commentaire { get; set; }
}
