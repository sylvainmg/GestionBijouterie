using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BijouterieApp.App.Services;

namespace BijouterieApp.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthentificationService _authService;
    private readonly SessionManager _sessionManager;

    [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _motDePasse = string.Empty;

    [ObservableProperty]
    private string _messageErreur = string.Empty;

    [ObservableProperty]
    private bool _estCharge;

    public LoginViewModel(AuthentificationService authService, SessionManager sessionManager)
    {
        _authService = authService;
        _sessionManager = sessionManager;
    }

    [RelayCommand]
    private async Task SeConnecterAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            MessageErreur = "Veuillez saisir votre login et mot de passe.";
            return;
        }

        EstCharge = true;
        MessageErreur = string.Empty;

        try
        {
            var employe = await _authService.AuthentifierAsync(Login, MotDePasse);
            if (employe == null)
            {
                MessageErreur = "Login ou mot de passe incorrect.";
                EstCharge = false;
                return;
            }

            _sessionManager.Connecter(employe);
        }
        catch (Exception ex)
        {
            MessageErreur = $"Erreur de connexion : {ex.Message}";
        }
        finally
        {
            EstCharge = false;
        }
    }
}
