using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace BijouterieApp.App.ViewModels;

public partial class AccueilViewModel : ObservableObject
{
    [ObservableProperty]
    private string _messageBienvenue = string.Empty;

    [ObservableProperty]
    private string _dateHeure = string.Empty;

    public AccueilViewModel()
    {
        MessageBienvenue = "Bienvenue dans l'application de gestion de bijouterie";
        DateHeure = DateTime.Now.ToString("dddd dd MMMM yyyy HH:mm");
    }
}