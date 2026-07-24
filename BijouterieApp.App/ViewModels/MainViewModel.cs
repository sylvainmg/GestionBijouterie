using CommunityToolkit.Mvvm.ComponentModel;
using BijouterieApp.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BijouterieApp.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SessionManager _sessionManager;
    private readonly LoginViewModel _loginViewModel;

    [ObservableProperty]
    private ObservableObject _currentView;

    public MainViewModel(SessionManager sessionManager, LoginViewModel loginViewModel)
    {
        _sessionManager = sessionManager;
        _loginViewModel = loginViewModel;
        _currentView = loginViewModel;

        _sessionManager.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SessionManager.UtilisateurCourant))
            {
                if (_sessionManager.EstConnecte)
                {
                    var shellVm = App.ServiceProvider.GetRequiredService<MainShellViewModel>();
                    CurrentView = shellVm;
                }
                else
                {
                    CurrentView = _loginViewModel;
                }
            }
        };
    }
}
