using BijouterieApp.Core.Enums;

namespace BijouterieApp.Core.Entities;

public class Employe
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Adresse { get; set; }
    public string? Fonction { get; set; }
    public string Login { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public RoleEmploye Role { get; set; }

    public ICollection<Vente> Ventes { get; set; } = new List<Vente>();
    public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();
}
