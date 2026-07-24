using CommunityToolkit.Mvvm.ComponentModel;
using BijouterieApp.Core.Entities;
using BijouterieApp.Core.Enums;

namespace BijouterieApp.App.Services;

public partial class SessionManager : ObservableObject
{
    [ObservableProperty]
    private Employe? _utilisateurCourant;

    public bool EstConnecte => UtilisateurCourant != null;
    public bool EstAdministrateur => UtilisateurCourant?.Role == RoleEmploye.Administrateur;
    public bool EstCaissier => UtilisateurCourant?.Role == RoleEmploye.Caissier;

    public void Connecter(Employe employe)
    {
        UtilisateurCourant = employe;
    }

    public void Deconnecter()
    {
        UtilisateurCourant = null;
    }
}
