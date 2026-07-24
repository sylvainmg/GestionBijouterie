using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BijouterieApp.Data;

public class BijouterieDbContextFactory : IDesignTimeDbContextFactory<BijouterieDbContext>
{
    public BijouterieDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BijouterieDbContext>();
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BijouterieApp", "bijouterie.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new BijouterieDbContext(optionsBuilder.Options);
    }
}
