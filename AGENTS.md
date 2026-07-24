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

### À venir
- [ ] Étape 2 — Modèles de données + DbContext + migration initiale + seed
- [ ] Étape 3 — Authentification (login/logout, hash, session, écran login FluentAvalonia)
- [x] Étape 4 — Shell principal (DockPanel/ListBox, menus par rôle, déconnexion)
- [ ] Étape 5 — Module Catégories (CRUD)
- [ ] Étape 6 — Module Bijoux (CRUD + photo + recherche)
- [ ] Étape 7 — Module Clients (CRUD + recherche)
- [ ] Étape 8 — Module Stock (vue stock, entrée de stock, alertes seuil faible)
- [ ] Étape 9 — Module Ventes (sélection client/bijoux, calcul, validation, décrément stock)
- [ ] Étape 10 — Génération PDF (facture, liste ventes, liste bijoux) + impression
- [ ] Étape 11 — Module Employés (CRUD, admin uniquement)
- [ ] Étape 12 — Tableau de bord (statistiques)
- [ ] Étape 13 — Tests unitaires + polish UI + README final

## NOTES

- La solution utilise .NET 10 (SDK 10.0.110)
- Pas de blobs binaires pour les photos : chemin relatif vers `/Assets/Photos/` stocké en base
- Projets nommés BijouterieApp.{Core,Data,App,Tests}
- FluentAvaloniaUI v3.0.2 est compatible avec Avalonia 12.1.0
- QuestPDF 2026.7.1 — licence Community gratuite pour revenu < $1M
- **Navigation**: Le shell utilise `DockPanel` + `ListBox` au lieu de `FANavigationView` (FluentAvalonia 3.0.2). La classe `FANavigationView` existe mais est instable en résolution XAML (propriétés non reconnues). À réévaluer dans une version ultérieure de FluentAvalonia ou si un contournement XAML est trouvé.
