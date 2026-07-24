# BijouterieApp — Application de Gestion de Bijouterie

Application desktop cross-platform (Windows, Linux, macOS) de gestion complète pour une bijouterie : catalogues, stock, ventes, clients, employés, et génération de documents PDF.

---

## Table des matieres

1. [Cahier des charges](#cahier-des-charges)
2. [Architecture technique](#architecture-technique)
3. [Structure du projet](#structure-du-projet)
4. [Modele de donnees](#modele-de-donnees)
5. [Fonctionnalites](#fonctionnalites)
6. [Comptes par defaut](#comptes-par-defaut)
7. [Lancer lapplication](#lancer-lapplication)
8. [Lancer les tests](#lancer-les-tests)
9. [Generer des PDF](#generer-des-pdf)
10. [Notes techniques](#notes-techniques)

---

## Cahier des charges

### Objectif

Fournir aux gérants et employés d'une bijouterie une application desktop permettant de gérer l'ensemble des opérations courantes :

- **Catalogue** : enregistrer les bijoux avec leur référence, catégorie, matière, poids, prix, photo et quantité en stock.
- **Stock** : suivre les entrées et sorties de stock avec historique, alertes visuelles lorsque le stock passe sous un seuil configurable.
- **Ventes** : créer des ventes avec sélection du client, ajout de plusieurs bijoux, vérification du stock disponible, calcul automatique du total avec remise possible, et décrément automatique du stock.
- **Clients** : gérer la base clients avec numéro automatique (CLI001, CLI002...).
- **Employés** : gérer les comptes employés avec authentification par login/mot de passe hashé (BCrypt), matricule automatique (ADM pour admins, CAI pour caissiers), et contrôle d'accès par rôle.
- **PDF** : générer des factures de vente, des rapports de ventes par période, et des catalogues de bijoux.
- **Tableau de bord** : vue synthétique avec statistiques (nombre de bijoux, clients, ventes, employés), chiffre d'affaires total et du mois en Ariary (MGA), et nombre de bijoux en stock faible.

### Public cible

- Gérant de bijouterie (rôle Administrateur) : accès à toutes les fonctionnalités dont la gestion des employés.
- Employé / Caissier (rôle Caissier) : accès aux ventes, stock, clients, bijoux, mais pas à la gestion des employés.

### Devise

Tous les prix et montants sont en **Ariary malgache (MGA)**, affichés au format `X XXX Ar` (ex : `2 500 000 Ar`).

---

## Architecture technique

### Stack

| Composant | Technologie | Version |
|---|---|---|
| Framework UI | Avalonia UI | 12.1.0 |
| Thème | FluentAvalonia (Windows 11) | 3.0.2 |
| Runtime | .NET | 10.0 |
| Pattern | MVVM | CommunityToolkit.Mvvm 8.4.2 |
| Base de données | SQLite + EF Core | 10.0.10 |
| PDF | QuestPDF (Community License) | 2026.7.1 |
| Sécurité | BCrypt.Net-Next | 4.2.0 |
| DI | Microsoft.Extensions.DependencyInjection | 10.0.10 |
| Tests | xUnit | 2.9.3 |

### Motifs de conception

- **MVVM strict** : aucune logique métier dans le code-behind des Views. Les ViewModels gèrent l'état, les commandes, et appellent les Services. Les Views sont 100 % XAML (hors code minimal de chargement dans `*.axaml.cs`).
- **Injection de dépendances** : tous les services et ViewModels sont enregistrés dans `App.axaml.cs` via `IServiceCollection` et résolus par le conteneur.
- **Services métier** : chaque module (Catégories, Bijoux, Clients, Stock, Ventes, Employés, PDF, Dashboard) possède son propre service gérant les opérations CRUD et les règles de validation.
- **Contexte scoped** : les services utilisent `IServiceScopeFactory` pour créer un scope par opération, garantissant un `DbContext` court-circuité.

### Style de code

- **Nommage** : `PascalCase` pour les classes, méthodes et propriétés publiques. `_camelCase` pour les champs privés. `camelCase` pour les variables locales.
- **Langue** : tout est en français (noms de classes, propriétés, messages d'erreur, commentaires, interface).
- **Fichiers XAML** : suffixe `.axaml`, un fichier par View.

---

## Structure du projet

```
GestionBijouterie/
├── BijouterieApp.slnx                    # Solution .NET
├── AGENTS.md                             # Suivi de progression
├── .gitignore
│
├── BijouterieApp.Core/                   # Couche metier / entites
│   ├── Entities/
│   │   ├── Employe.cs                    # Employe (Matricule, Login, Role, etc.)
│   │   ├── Categorie.cs                  # Categorie de bijoux
│   │   ├── Bijou.cs                      # Bijou (Reference, Prix, Stock, Photo, etc.)
│   │   ├── Client.cs                     # Client (NumeroClient auto)
│   │   ├── Vente.cs                      # Vente (Client, Employe, Total, Remise)
│   │   ├── LigneVente.cs                 # Ligne de vente (Bijou, Quantite, PrixUnitaire)
│   │   └── MouvementStock.cs             # Mouvement (Entree/Sortie, Quantite, Date)
│   └── Enums/
│       ├── RoleEmploye.cs                # Administrateur, Caissier
│       └── TypeMouvementStock.cs         # Entree, Sortie
│
├── BijouterieApp.Data/                   # Couche acces aux donnees
│   ├── BijouterieDbContext.cs            # DbContext EF Core + Fluent API
│   ├── BijouterieDbContextFactory.cs     # Factory pour les migrations
│   ├── SeedData.cs                       # Donnees initiales (admin, categories, bijoux)
│   └── Migrations/                       # Migrations EF Core
│
├── BijouterieApp.App/                    # Application desktop (UI + logique)
│   ├── App.axaml / App.axaml.cs          # Point d'entree + configuration DI
│   ├── MainWindow.axaml                  # Fenetre principale
│   ├── Program.cs                        # Bootstrap .NET
│   ├── Assets/Photos/                    # Dossier de stockage des photos
│   ├── Services/
│   │   ├── AuthentificationService.cs    # Verification login + BCrypt
│   │   ├── SessionManager.cs             # Session utilisateur connecte
│   │   ├── CategorieService.cs           # CRUD categories
│   │   ├── BijouService.cs               # CRUD bijoux
│   │   ├── ClientService.cs              # CRUD clients (numero auto CLIxxx)
│   │   ├── StockService.cs               # Entrees/sorties stock + historique
│   │   ├── VenteService.cs               # Creation ventes + verification stock
│   │   ├── EmployeService.cs             # CRUD employes (matricule auto, BCrypt)
│   │   ├── DashboardService.cs           # Statistiques tableau de bord
│   │   └── PdfService.cs                 # Generation PDF (QuestPDF)
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs             # Ecran de connexion
│   │   ├── MainViewModel.cs              # Conteneur principal (Login <-> Shell)
│   │   ├── MainShellViewModel.cs         # Navigation laterale + contenu
│   │   ├── AccueilViewModel.cs           # Tableau de bord
│   │   ├── CategoriesViewModel.cs        # Gestion categories
│   │   ├── BijouxViewModel.cs            # Gestion bijoux + photo
│   │   ├── ClientsViewModel.cs           # Gestion clients
│   │   ├── StockViewModel.cs             # Gestion stock
│   │   ├── VentesViewModel.cs            # Gestion ventes
│   │   └── EmployesViewModel.cs          # Gestion employes
│   └── Views/
│       ├── LoginView.axaml               # Ecran de connexion
│       ├── MainShellView.axaml           # Shell principal (menu + contenu)
│       ├── AccueilView.axaml             # Tableau de bord
│       ├── CategoriesView.axaml          # Module categories
│       ├── BijouxView.axaml              # Module bijoux
│       ├── ClientsView.axaml             # Module clients
│       ├── StockView.axaml               # Module stock
│       ├── VentesView.axaml              # Module ventes
│       └── EmployesView.axaml            # Module employes
│
└── BijouterieApp.Tests/                  # Tests unitaires
    ├── LigneVenteTests.cs                # 5 tests (calcul total, quantite zero, etc.)
    └── BijouStockTests.cs                # 14 tests (stock, ventes, numeros client)
```

---

## Modele de donnees

### Diagramme des relations

```
Categorie 1 ──── * Bijou
Bijou      1 ──── * LigneVente
Bijou      1 ──── * MouvementStock
Client     1 ──── * Vente
Employe    1 ──── * Vente
Employe    1 ──── * MouvementStock
Vente      1 ──── * LigneVente
```

### Entites

| Entite | Champs principaux | Regles |
|---|---|---|
| **Employe** | Matricule (auto ADM/CAI), Nom, Prenom, Login (unique), MotDePasseHash (BCrypt), Role | Suppression bloquee si ventes/mouvements lies |
| **Categorie** | Nom, Description | Suppression bloquee si bijoux lies |
| **Bijou** | Reference (unique), Nom, CategorieId (FK), Matiere, PoidsGrammes, Prix (MGA), QuantiteStock, PhotoPath | Suppression bloquee si ventes/mouvements lies |
| **Client** | NumeroClient (auto CLI001), Nom, Prenom, Telephone, Adresse, Email | Suppression bloquee si ventes liees |
| **Vente** | Date, ClientId (FK), EmployeId (FK), Total, Remise | Total = somme des lignes - remise |
| **LigneVente** | VenteId (FK), BijouId (FK), Quantite, PrixUnitaire | Total = Quantite x PrixUnitaire |
| **MouvementStock** | BijouId (FK), Type (Entree/Sortie), Quantite, Date, EmployeId (FK), Commentaire | Cree automatiquement lors des entrees/sorties |

---

## Fonctionnalites

### Authentification
- Ecran de connexion avec login et mot de passe.
- Mots de passe hashés avec BCrypt (jamais en clair en base).
- Gestion de session via `SessionManager` (utilisateur courant, role).
- Controle d'ecran : redirection automatique Login <-> Shell.

### Navigation
- Menu lateral (`ListBox`) avec items conditionnes par le role.
- L'item "Employes" n'apparait que pour les administrateurs.
- Bouton de deconnexion dans le menu.

### Module Categories (CRUD)
- Liste avec recherche par nom.
- Ajout, modification, suppression avec verification des bijoux lies.

### Module Bijoux (CRUD + photo)
- DataGrid 7 colonnes : Reference, Nom, Categorie, Matiere, Poids, Prix, Stock.
- Formulaire latéral scrollable avec ComboBox catégorie.
- Upload de photo via le sélecteur de fichiers natif (Avalonia StorageProvider).
- Recherche multi-champs (nom, référence, matière, catégorie, description).
- Bouton « Catalogue PDF » pour exporter la liste.

### Module Clients (CRUD)
- DataGrid 6 colonnes : N° Client, Nom, Prénom, Téléphone, Email, Adresse.
- Numéro client automatique (CLI001, CLI002...).
- Recherche multi-champs.

### Module Stock
- DataGrid du stock disponible avec recherche.
- Formulaire d'entrée de stock avec quantité et commentaire.
- Historique des mouvements (entrées/sorties) par bijou.
- Filtre d'alertes : afficher les bijoux dont le stock est inférieur ou égal à un seuil configurable (défaut : 5).

### Module Ventes
- Liste des ventes passées avec recherche.
- Création de vente en 3 étapes : sélection du client, ajout de lignes (bijou + quantité), validation.
- Vérification du stock disponible pour chaque ligne.
- Calcul automatique du total avec remise optionnelle.
- Décrément automatique du stock lors de la validation.
- Boutons « Facture PDF » (vente sélectionnée) et « Rapport du mois ».

### Module Employés (admin uniquement)
- DataGrid 6 colonnes : Matricule, Nom, Prénom, Fonction, Login, Rôle.
- Matricule automatique : ADM pour administrateurs, CAI pour caissiers.
- Mot de passe hashé BCrypt à la création, modification optionnelle.
- Login unique vérifié.
- Suppression bloquée si l'employé est lié à des ventes ou mouvements.

### Tableau de bord
- Cards avec statistiques : nombre de bijoux, clients, ventes, employés.
- Chiffre d'affaires total et du mois courant en MGA.
- Nombre de bijoux en stock faible (seuil ≤ 5).

### Génération PDF (QuestPDF)
- **Facture de vente** : format A4 portrait, en-tête avec identité de la bijouterie, infos client/employé, tableau des lignes, sous-total, remise, total.
- **Rapport de ventes** : format A4 paysage, liste des ventes d'un mois avec total général.
- **Catalogue de bijoux** : format A4 paysage, tous les bijoux avec référence, catégorie, matière, poids, prix, stock.
- Les PDF sont sauvegardés sur le Bureau et ouverts automatiquement.

---

## Comptes par defaut

Le seed de la base de données crée les données initiales suivantes :

| Role | Login | Mot de passe | Matricule |
|---|---|---|---|
| Administrateur | `admin` | `admin123` | ADM001 |

Catégories pré-chargées : Or, Argent, Diamant, Bracelet, Collier, Bague, Boucle d'oreille.

Bijoux pré-chargés : Bague Or Jaune (850 000 Ar), Collier Argent (120 000 Ar), Bague Diamant (2 500 000 Ar), Bracelet Or (950 000 Ar), Collier Perles (450 000 Ar).

Client pré-chargé : Jean Rakoto (CLI001).

> **Important** : la base de données SQLite est créée automatiquement au premier lancement dans `~/.local/share/BijouterieApp/bijouterie.db` (Linux). Les données de seed ne sont insérées que si la base est vide.

---

## Lancer lapplication

### Prérequis

- .NET 10 SDK (10.0.110 ou supérieur)
- Aucune dépendance système supplémentaire (Avalonia inclut tout)

### Compilation

```bash
# Cloner le depot
git clone <url-du-depot>
cd GestionBijouterie

# Compiler
dotnet build
```

### Execution

```bash
# Lancer l'application
dotnet run --project BijouterieApp.App
```

L'application s'ouvre sur l'ecran de connexion. Utilisez les identifiants `admin` / `admin123` pour se connecter en tant qu'administrateur.

### Publication (binaire autonome)

```bash
# Generer un binaire publiable
dotnet publish BijouterieApp.App -c Release -r linux-x64 --self-contained

# Le binaire sera dans :
# BijouterieApp.App/bin/Release/net10.0/linux-x64/publish/
```

### Creer un nouvel employé

En tant qu'administrateur, aller dans le menu « Employés », cliquer « Nouveau », remplir les champs (le matricule et le login sont saisis manuellement, le matricule est généré automatiquement lors de l'enregistrement), puis valider.

---

## Lancer les tests

```bash
# Executer tous les tests
dotnet test
```

Résultat attendu : **19 tests reussis**, 0 échec.

Couverture des tests :
- `LigneVenteTests` : calcul du Total (quantité normale, zéro, grande quantité, prix décimaux)
- `BijouStockTests` : init du stock, augmentation, diminution, détection de seuil d'alerte
- `VenteCalculsTests` : total sans remise, avec remise, panier vide
- `ClientNumeroTests` : génération du numéro client (théories paramétrées)

---

## Generer des PDF

Les PDF sont générés via QuestPDF et sauvegardés sur le Bureau de l'utilisateur :

| Document | Depuis | Emplacement |
|---|---|---|
| Facture de vente | Module Ventes > sélectionner une vente > « Facture PDF » | `~/BijouterieApp_Factures/Facture_0001.pdf` |
| Rapport mensuel | Module Ventes > « Rapport du mois » | `~/BijouterieApp_Rapports/Ventes_YYYYMMDD_YYYYMMDD.pdf` |
| Catalogue bijoux | Module Bijoux > « Catalogue PDF » | `~/BijouterieApp_Rapports/Bijoux_YYYYMMDD.pdf` |

Le fichier est ouvert automatiquement après la génération.

---

## Notes techniques

### Base de données

- Fichier SQLite stocké dans `~/.local/share/BijouterieApp/bijouterie.db` (Linux), `%LOCALAPPDATA%\BijouterieApp\bijouterie.db` (Windows).
- Créée automatiquement au premier lancement avec migration EF Core + données de seed.
- Les schémas sont gérés par les migrations EF Core dans `BijouterieApp.Data/Migrations/`.

### Photos des bijoux

- Les photos ne sont pas stockées en base (pas de blob).
- Un chemin relatif (`Assets/Photos/nom_fichier.jpg`) est stocké dans `PhotoPath`.
- Les fichiers physiques sont copiés dans `BijouterieApp.App/Assets/Photos/` lors de l'upload.
- Le dossier est exclu du dépôt git (`.gitignore`).

### Authentification

- Les mots de passe sont hashés avec BCrypt.Net (`$2a$...`).
- Vérification via `BCrypt.Net.BCrypt.Verify(motDePasse, hash)`.
- La session est gérée en mémoire par `SessionManager` (ObservableObject).
- Aucune persistance de session : reconnexion nécessaire après fermeture.

### Navigation et Vue

- Le shell utilise un `DockPanel` + `ListBox` pour le menu latéral (pas `FANavigationView` de FluentAvalonia qui présente des problèmes de résolution XAML).
- Chaque Vue charge ses données via `OnDataContextChanged` → exécution de la commande de chargement.
- Le switch des ViewModels dans `MainShellViewModel` résout les instances depuis le conteneur DI.

### Connaissances documentées

- `NumericUpDown` en Avalonia 12 ne supporte pas la propriété `Header` : utiliser un `TextBlock` séparé comme label.
- QuestPDF : l'ordre des appels chaînés est important — le padding doit être appliqué sur le conteneur **avant** le trait `LineHorizontal`, pas après.
- Le sélecteur de fichiers natif en Avalonia utilise `TopLevel.GetTopLevel(mainWindow)?.StorageProvider.OpenFilePickerAsync()` (pas `Microsoft.Win32.OpenFileDialog` qui n'existe pas en Avalonia).
