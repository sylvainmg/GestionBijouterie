# Gestion Bijouterie

Application desktop de gestion de bijouterie.

## Stack technique

- **Framework UI**: Avalonia UI 12.1.0 + FluentAvalonia 3.0.2 (thème Windows 11)
- **Runtime**: .NET 10, architecture MVVM
- **MVVM Toolkit**: CommunityToolkit.Mvvm 8.4.2
- **Base de données**: SQLite + EF Core 10.0.10 (migrations)
- **PDF**: QuestPDF 2026.7.1 (Community License)
- **Hash mots de passe**: BCrypt.Net-Next 4.2.0
- **DI**: Microsoft.Extensions.DependencyInjection 10.0.10
- **Tests**: xUnit 2.9.3 + Microsoft.NET.Test.Sdk 18.8.1
- **Devise**: Ariary malgache (MGA), format `X XXX Ar`

## Conventions de code

- **Nommage**: PascalCase pour classes/méthodes/propriétés, _camelCase pour champs privés, camelCase pour variables locales
- **Structure dossiers**: Couches Core (entités/logique métier) / Data (DbContext/repos) / App (Views/ViewModels/Services)
- **MVVM strict**: Pas de logique métier dans le code-behind. Views = XAML uniquement (sauf code minimal dans App.axaml.cs pour l'enregistrement des services)
- **Langue**: Français (UI, commentaires, messages)
- **Fichiers XAML**: Un fichier par View, suffixé `.axaml`

## PROGRESS

### Étape 1 — Initialisation (terminée le 24/07/2026)
- [x] Initialiser le dépôt git
- [x] Créer la solution avec 4 projets (Core, Data, App, Tests)
- [x] Ajouter les références entre projets
- [x] Installer tous les packages NuGet
- [x] Créer les dossiers (Assets/Photos, Views, ViewModels, Services)
- [x] Créer AGENTS.md et .gitignore
- [x] Vérifier que le projet compile (`dotnet build`)

### Étape 2 — Modèles de données + DbContext + migration initiale + seed (terminée le 24/07/2026)
- [x] Créer les entités (Employe, Categorie, Bijou, Client, Vente, LigneVente, MouvementStock)
- [x] Créer les énumérations (RoleEmploye, TypeMouvementStock)
- [x] Créer le DbContext avec configurations Fluent API
- [x] Créer la migration initiale
- [x] Créer les données de seed (admin, catégories, bijoux, client)
- [x] Vérifier que le projet compile

### Étape 3 — Authentification (terminée le 24/07/2026)
- [x] Créer AuthentificationService (login avec BCrypt)
- [x] Créer SessionManager (session utilisateur, vérification rôle)
- [x] Créer LoginView/LoginViewModel (écran de connexion FluentAvalonia)
- [x] Redirection vers MainShellView après connexion réussie

### Étape 4 — Shell principal (terminée le 24/07/2026)
- [x] Créer MainShellView/MainShellViewModel (DockPanel + ListBox)
- [x] Afficher les menus par rôle (Employés visible admin seulement)
- [x] Déconnexion depuis le shell

### Étape 5 — Module Catégories (CRUD) (terminée le 24/07/2026)
- [x] Créer CategorieService (CRUD : GetAll, GetById, Create, Update, Delete avec vérification des bijoux liés)
- [x] Implémenter CategoriesViewModel (liste filtrée, ajout, modification, suppression avec CommunityToolkit.Mvvm)
- [x] Mettre à jour CategoriesView.axaml (DataGrid + formulaire latéral avec saisie Nom/Description + boutons)
- [x] Ajouter DataGrid NuGet + styles Fluent, enregistrer CategorieService en DI

### Étape 6 — Module Bijoux (CRUD + photo + recherche) (terminée le 24/07/2026)
- [x] Créer BijouService (CRUD : GetAll, GetById, Create, Update, Delete avec vérification ventes/mouvements liés)
- [x] Implémenter BijouxViewModel (liste filtrée multi-champs, ajout, modification, suppression, upload photo via Avalonia StorageProvider)
- [x] Créer BijouxView.axaml (DataGrid 7 colonnes + formulaire latéral scrollable + ComboBox catégorie + sélection/upload photo)
- [x] Enregistrer BijouService en DI, charger catégories pour ComboBox

### Étape 7 — Module Clients (CRUD + recherche) (terminée le 24/07/2026)
- [x] Créer ClientService (CRUD : GetAll, GetById, Create, Update, Delete avec vérification ventes liées + numéro auto CLI001)
- [x] Implémenter ClientsViewModel (liste filtrée multi-champs, ajout, modification, suppression avec CommunityToolkit.Mvvm)
- [x] Mettre à jour ClientsView.axaml (DataGrid 6 colonnes + formulaire latéral avec Nom/Prénom/Téléphone/Adresse/Email)
- [x] Enregistrer ClientService en DI

### Étape 8 — Module Stock (terminée le 24/07/2026)
- [x] Créer StockService (GetAll, GetMouvements, EntrerStock, SortirStock)
- [x] Implémenter StockViewModel (vue stock filtrée, formulaire entrée stock, historique mouvements, filtre alertes seuil configurable)
- [x] Mettre à jour StockView.axaml (DataGrid stock + DataGrid mouvements + formulaire latéral + checkbox alertes + NumericUpDown seuil)
- [x] Enregistrer StockService en DI

### Étape 9 — Module Ventes (terminée le 24/07/2026)
- [x] Créer VenteService (GetAll, GetById, GetByPeriode, CreerVente avec vérification stock + décrément)
- [x] Implémenter VentesViewModel (liste filtrée, sélection client, ajout lignes, calcul total avec remise, validation stock)
- [x] Mettre à jour VentesView.axaml (DataGrid ventes + panneau création vente avec DataGrid lignes + NumericUpDown remise)
- [x] Enregistrer VenteService en DI

### Étape 10 — Génération PDF (terminée le 24/07/2026)
- [x] Créer PdfService (facture de vente, liste ventes par période, catalogue bijoux) avec QuestPDF Fluent API
- [x] Ajouter boutons "Facture PDF" et "Rapport du mois" dans VentesView
- [x] Ajouter bouton "Catalogue PDF" dans BijouxView
- [x] Ajouter commandes GenererFacture, GenererListeVentesPeriode, GenererListeBijoux dans les ViewModels
- [x] Enregistrer PdfService en DI

### Étape 11 — Module Employés CRUD (terminée le 24/07/2026)
- [x] Créer EmployeService (CRUD : GetAll, GetById, Create, Update, Delete avec matricule auto ADM/CAI, hash BCrypt, login unique)
- [x] Implémenter EmployesViewModel (liste filtrée, formulaire avec rôle ComboBox, validation login/mot de passe)
- [x] Mettre à jour EmployesView.axaml (DataGrid 6 colonnes + formulaire latéral avec tous les champs + ComboBox rôle)
- [x] Enregistrer EmployeService en DI

### Étape 12 — Tableau de bord (terminée le 24/07/2026)
- [x] Créer DashboardService (nombre bijoux/clients/ventes/employés, CA total, CA mois, stock faible)
- [x] Implémenter AccueilViewModel avec chargement statistiques
- [x] Mettre à jour AccueilView.axaml (cards WrapPanel avec statistiques + chiffre d'affaires MGA + alerte stock faible)
- [x] Enregistrer DashboardService en DI

### Étape 13 — Tests unitaires + polish UI (terminée le 24/07/2026)
- [x] Tests LigneVenteTests (5 tests : calcul total, quantité zero, grandes quantités, prix décimaux)
- [x] Tests BijouStockTests (6 tests : stock init, augmentation, diminution, détection seuil)
- [x] Tests VenteCalculsTests (4 tests : total sans remise, avec remise, vide, formatage MGA)
- [x] Tests ClientNumeroTests (4 théories : génération numéro CLI)
- [x] Vérifier `dotnet test` (19/19 tests passent)
- [x] Vérifier `dotnet build` (0 erreurs)

### Terminé — Projet complet

## NOTES

- La solution utilise .NET 10 (SDK 10.0.110)
- Pas de blobs binaires pour les photos : chemin relatif vers `/Assets/Photos/` stocké en base
- Projets nommés BijouterieApp.{Core,Data,App,Tests}
- FluentAvaloniaUI v3.0.2 est compatible avec Avalonia 12.1.0
- QuestPDF 2026.7.1 — licence Community gratuite pour revenu < $1M
- **Navigation**: Le shell utilise `DockPanel` + `ListBox` au lieu de `FANavigationView` (FluentAvalonia 3.0.2). La classe `FANavigationView` existe mais est instable en résolution XAML (propriétés non reconnues). À réévaluer dans une version ultérieure de FluentAvalonia ou si un contournement XAML est trouvé.
- **Upload photo**: Utiliser `TopLevel.GetTopLevel(mainWindow)?.StorageProvider.OpenFilePickerAsync()` depuis le ViewModel (via `Application.Current.ApplicationLifetime`). Ne pas utiliser `Microsoft.Win32.OpenFileDialog` (inexistant en Avalonia).
- **NumericUpDown**: Avalonia 12 ne supporte pas la propriété `Header` sur `NumericUpDown`. Utiliser un `TextBlock` séparé comme label.
- **QuestPDF**: L'ordre des appels chaînés est important — `PaddingBottom(5).LineHorizontal(1).LineColor(...)` (padding sur le container avant le trait, pas après).
- **Tests unitaires**: 19 tests couvrant le calcul de LigneVente.Total, la gestion de stock (augmentation/diminution/seuil), les calculs de vente avec remise, et la génération de numéros client.
