using Microsoft.EntityFrameworkCore;
using BijouterieApp.Core.Entities;
using BijouterieApp.Core.Enums;

namespace BijouterieApp.Data;

public class BijouterieDbContext : DbContext
{
    public BijouterieDbContext(DbContextOptions<BijouterieDbContext> options) : base(options) { }

    public DbSet<Employe> Employes => Set<Employe>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Bijou> Bijoux => Set<Bijou>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Vente> Ventes => Set<Vente>();
    public DbSet<LigneVente> LigneVentes => Set<LigneVente>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employe>(entity =>
        {
            entity.ToTable("Employes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Matricule).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nom).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Prenom).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Telephone).HasMaxLength(20);
            entity.Property(e => e.Adresse).HasMaxLength(255);
            entity.Property(e => e.Fonction).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(50).IsRequired();
            entity.Property(e => e.MotDePasseHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(e => e.Login).IsUnique();
        });

        modelBuilder.Entity<Categorie>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nom).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Bijou>(entity =>
        {
            entity.ToTable("Bijoux");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Reference).HasMaxLength(50).IsRequired();
            entity.Property(b => b.Nom).HasMaxLength(200).IsRequired();
            entity.Property(b => b.Matiere).HasMaxLength(100);
            entity.Property(b => b.PoidsGrammes).HasColumnType("decimal(10,2)");
            entity.Property(b => b.Prix).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(b => b.Description).HasMaxLength(500);
            entity.Property(b => b.PhotoPath).HasMaxLength(500);
            entity.HasIndex(b => b.Reference).IsUnique();
            entity.HasOne(b => b.Categorie)
                  .WithMany(c => c.Bijoux)
                  .HasForeignKey(b => b.CategorieId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.NumeroClient).HasMaxLength(50).IsRequired();
            entity.Property(c => c.Nom).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Prenom).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Telephone).HasMaxLength(20);
            entity.Property(c => c.Adresse).HasMaxLength(255);
            entity.Property(c => c.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Vente>(entity =>
        {
            entity.ToTable("Ventes");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Date).IsRequired();
            entity.Property(v => v.Total).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(v => v.Remise).HasColumnType("decimal(18,2)");
            entity.HasOne(v => v.Client)
                  .WithMany(c => c.Ventes)
                  .HasForeignKey(v => v.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(v => v.Employe)
                  .WithMany(e => e.Ventes)
                  .HasForeignKey(v => v.EmployeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LigneVente>(entity =>
        {
            entity.ToTable("LigneVentes");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.PrixUnitaire).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(l => l.Quantite).IsRequired();
            entity.HasOne(l => l.Vente)
                  .WithMany(v => v.LigneVentes)
                  .HasForeignKey(l => l.VenteId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(l => l.Bijou)
                  .WithMany(b => b.LigneVentes)
                  .HasForeignKey(l => l.BijouId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MouvementStock>(entity =>
        {
            entity.ToTable("MouvementsStock");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Type).HasConversion<string>().HasMaxLength(10);
            entity.Property(m => m.Date).IsRequired();
            entity.Property(m => m.Commentaire).HasMaxLength(500);
            entity.HasOne(m => m.Bijou)
                  .WithMany(b => b.MouvementsStock)
                  .HasForeignKey(m => m.BijouId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(m => m.Employe)
                  .WithMany(e => e.MouvementsStock)
                  .HasForeignKey(m => m.EmployeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
