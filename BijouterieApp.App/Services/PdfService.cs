using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BijouterieApp.Core.Entities;

namespace BijouterieApp.App.Services;

public class PdfService
{
    private readonly VenteService _venteService;
    private readonly BijouService _bijouService;

    public PdfService(VenteService venteService, BijouService bijouService)
    {
        _venteService = venteService;
        _bijouService = bijouService;
    }

    public async Task GenererFactureAsync(int venteId, string? dossierSortie = null)
    {
        var vente = await _venteService.GetByIdAsync(venteId)
            ?? throw new InvalidOperationException("Vente introuvable.");

        var dossier = dossierSortie ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BijouterieApp_Factures");
        Directory.CreateDirectory(dossier);

        var cheminFichier = Path.Combine(dossier, $"Facture_{vente.Id:D4}.pdf");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("BIJOUTERIE").FontSize(20).Bold().FontColor("#1a1a1a");
                            col.Item().Text("Gestion de Bijouterie").FontSize(10).FontColor("#666666");
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"FACTURE N° {vente.Id:D4}").FontSize(14).Bold();
                            col.Item().Text($"Date : {vente.Date:dd/MM/yyyy HH:mm}").FontSize(10);
                        });
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().PaddingBottom(15).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Client").Bold().FontSize(11);
                                c.Item().Text($"{vente.Client.Prenom} {vente.Client.Nom}");
                                if (!string.IsNullOrEmpty(vente.Client.Telephone))
                                    c.Item().Text($"Tél : {vente.Client.Telephone}");
                                if (!string.IsNullOrEmpty(vente.Client.Email))
                                    c.Item().Text($"Email : {vente.Client.Email}");
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text("Vendeur").Bold().FontSize(11);
                                c.Item().Text($"{vente.Employe.Prenom} {vente.Employe.Nom}");
                                if (!string.IsNullOrEmpty(vente.Employe.Fonction))
                                    c.Item().Text(vente.Employe.Fonction);
                            });
                        });

                        col.Item().PaddingBottom(5).LineHorizontal(1).LineColor("#cccccc");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(110);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Text("Bijou").Bold().FontSize(9);
                                header.Cell().AlignRight().Text("Prix unit. (Ar)").Bold().FontSize(9);
                                header.Cell().AlignRight().Text("Qté").Bold().FontSize(9);
                                header.Cell().AlignRight().Text("Sous-total (Ar)").Bold().FontSize(9);
                            });
                            foreach (var ligne in vente.LigneVentes)
                            {
                                table.Cell().Text(ligne.Bijou.Nom).FontSize(9);
                                table.Cell().AlignRight().Text($"{ligne.PrixUnitaire:N0}").FontSize(9);
                                table.Cell().AlignRight().Text($"{ligne.Quantite}").FontSize(9);
                                table.Cell().AlignRight().Text($"{ligne.Total:N0}").FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor("#cccccc");

                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem();
                            row.ConstantItem(200).Column(c =>
                            {
                                c.Item().Row(r =>
                                {
                                    r.RelativeItem().Text("Sous-total :").FontSize(10);
                                    r.RelativeItem().AlignRight().Text($"{vente.LigneVentes.Sum(l => l.Total):N0} Ar").FontSize(10);
                                });
                                if (vente.Remise.HasValue && vente.Remise.Value > 0)
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text("Remise :").FontSize(10);
                                        r.RelativeItem().AlignRight().Text($"- {vente.Remise.Value:N0} Ar").FontSize(10).FontColor("#cc0000");
                                    });
                                }
                                c.Item().PaddingTop(4).LineHorizontal(1).LineColor("#cccccc");
                                c.Item().PaddingTop(4).Row(r =>
                                {
                                    r.RelativeItem().Text("TOTAL :").Bold().FontSize(13);
                                    r.RelativeItem().AlignRight().Text($"{vente.Total:N0} Ar").Bold().FontSize(13);
                                });
                            });
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Merci pour votre achat !").FontSize(9).FontColor("#999999");
                });
            });
        });

        document.GeneratePdf(cheminFichier);
        OuvrirFichier(cheminFichier);
    }

    public async Task GenererListeVentesAsync(DateTime debut, DateTime fin, string? dossierSortie = null)
    {
        var ventes = await _venteService.GetByPeriodeAsync(debut, fin);

        var dossier = dossierSortie ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BijouterieApp_Rapports");
        Directory.CreateDirectory(dossier);

        var cheminFichier = Path.Combine(dossier,
            $"Ventes_{debut:yyyyMMdd}_{fin:yyyyMMdd}.pdf");

        var totalGeneral = ventes.Sum(v => v.Total);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("BIJOUTERIE").FontSize(18).Bold();
                            col.Item().Text("Liste des ventes").FontSize(12);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Période : {debut:dd/MM/yyyy} — {fin:dd/MM/yyyy}").FontSize(10);
                            col.Item().Text($"Généré le {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor("#999999");
                        });
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().PaddingBottom(5).Text($"Nombre de ventes : {ventes.Count}  |  Total général : {totalGeneral:N0} Ar").Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(60);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Text("N°").Bold();
                                header.Cell().Text("Date").Bold();
                                header.Cell().Text("Client").Bold();
                                header.Cell().Text("Employé").Bold();
                                header.Cell().AlignRight().Text("Total (Ar)").Bold();
                                header.Cell().AlignRight().Text("Remise (Ar)").Bold();
                                header.Cell().AlignRight().Text("Lignes").Bold();
                            });
                            foreach (var v in ventes)
                            {
                                table.Cell().Text($"{v.Id:D4}");
                                table.Cell().Text($"{v.Date:dd/MM/yyyy HH:mm}");
                                table.Cell().Text($"{v.Client.Prenom} {v.Client.Nom}");
                                table.Cell().Text($"{v.Employe.Prenom} {v.Employe.Nom}");
                                table.Cell().AlignRight().Text($"{v.Total:N0}");
                                table.Cell().AlignRight().Text(v.Remise.HasValue ? $"{v.Remise.Value:N0}" : "-");
                                table.Cell().AlignRight().Text($"{v.LigneVentes.Count}");
                            }
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Rapport de ventes — Bijouterie").FontSize(8).FontColor("#999999");
                });
            });
        });

        document.GeneratePdf(cheminFichier);
        OuvrirFichier(cheminFichier);
    }

    public async Task GenererListeBijouxAsync(string? dossierSortie = null)
    {
        var bijoux = await _bijouService.GetAllAsync();

        var dossier = dossierSortie ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BijouterieApp_Rapports");
        Directory.CreateDirectory(dossier);

        var cheminFichier = Path.Combine(dossier,
            $"Bijoux_{DateTime.Now:yyyyMMdd}.pdf");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("BIJOUTERIE").FontSize(18).Bold();
                            col.Item().Text("Catalogue des bijoux").FontSize(12);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Total : {bijoux.Count} bijoux").FontSize(10);
                            col.Item().Text($"Généré le {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor("#999999");
                        });
                    });
                });

                page.Content().Element(content =>
                {
                    content.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(70);
                        });
                        table.Header(header =>
                        {
                            header.Cell().Text("Réf.").Bold();
                            header.Cell().Text("Nom").Bold();
                            header.Cell().Text("Catégorie").Bold();
                            header.Cell().Text("Matière").Bold();
                            header.Cell().AlignRight().Text("Poids (g)").Bold();
                            header.Cell().AlignRight().Text("Prix (Ar)").Bold();
                            header.Cell().AlignRight().Text("Stock").Bold();
                        });
                        foreach (var b in bijoux)
                        {
                            table.Cell().Text(b.Reference);
                            table.Cell().Text(b.Nom);
                            table.Cell().Text(b.Categorie.Nom);
                            table.Cell().Text(b.Matiere ?? "-");
                            table.Cell().AlignRight().Text(b.PoidsGrammes.HasValue ? $"{b.PoidsGrammes.Value:F2}" : "-");
                            table.Cell().AlignRight().Text($"{b.Prix:N0}");
                            table.Cell().AlignRight().Text($"{b.QuantiteStock}");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Catalogue — Bijouterie").FontSize(8).FontColor("#999999");
                });
            });
        });

        document.GeneratePdf(cheminFichier);
        OuvrirFichier(cheminFichier);
    }

    private static void OuvrirFichier(string chemin)
    {
        try
        {
            if (OperatingSystem.IsLinux())
            {
                var psi = new ProcessStartInfo("xdg-open", chemin)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
            }
            else if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo(chemin) { UseShellExecute = true });
            }
            else if (OperatingSystem.IsMacOS())
            {
                var psi = new ProcessStartInfo("open", chemin)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur ouverture PDF : {ex.Message}");
        }
    }
}
