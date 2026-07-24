using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BijouterieApp.Data;

namespace BijouterieApp.App.Services;

public class AuthentificationService
{
    private readonly BijouterieDbContext _context;

    public AuthentificationService(BijouterieDbContext context)
    {
        _context = context;
    }

    public async Task<Core.Entities.Employe?> AuthentifierAsync(string login, string motDePasse)
    {
        var employe = await Task.Run(() =>
            _context.Employes.FirstOrDefault(e => e.Login == login));

        if (employe == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(motDePasse, employe.MotDePasseHash))
            return null;

        return employe;
    }
}
