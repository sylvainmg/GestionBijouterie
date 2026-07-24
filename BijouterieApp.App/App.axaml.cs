using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BijouterieApp.Data;
using BijouterieApp.App.Services;
using BijouterieApp.App.ViewModels;
using BijouterieApp.App.Views;

namespace BijouterieApp.App;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigurerServices(services);
        ServiceProvider = services.BuildServiceProvider();

        InitialiserBaseDonnees().GetAwaiter().GetResult();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigurerServices(IServiceCollection services)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BijouterieApp", "bijouterie.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        services.AddDbContext<BijouterieDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddSingleton<SessionManager>();
        services.AddTransient<AuthentificationService>();
        services.AddTransient<CategorieService>();
        services.AddTransient<BijouService>();
        services.AddTransient<ClientService>();
        services.AddTransient<StockService>();
        services.AddTransient<VenteService>();
        services.AddTransient<EmployeService>();
        services.AddTransient<PdfService>();
        services.AddTransient<DashboardService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<AccueilViewModel>();
        services.AddTransient<CategoriesViewModel>();
        services.AddTransient<BijouxViewModel>();
        services.AddTransient<ClientsViewModel>();
        services.AddTransient<StockViewModel>();
        services.AddTransient<VentesViewModel>();
        services.AddTransient<EmployesViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainShellViewModel>();

        services.AddTransient<LoginView>();
        services.AddTransient<AccueilView>();
        services.AddTransient<CategoriesView>();
        services.AddTransient<BijouxView>();
        services.AddTransient<ClientsView>();
        services.AddTransient<StockView>();
        services.AddTransient<VentesView>();
        services.AddTransient<EmployesView>();
        services.AddTransient<MainShellView>();
        services.AddTransient<MainWindow>();
    }

    private static async Task InitialiserBaseDonnees()
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BijouterieDbContext>();
        await context.Database.MigrateAsync();
        await SeedData.InitializeAsync(context);
    }
}