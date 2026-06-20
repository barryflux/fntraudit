# Architecture FntrAudit

## Objectif

FntrAudit est une application desktop WPF de gestion d'audits.

## Stack technique

- C#
- WPF
- .NET 8
- Entity Framework Core
- SQLite
- Architecture MVVM

## Structure du projet

### Data

Contient l'accès aux données et l'initialisation de la base locale.

- `AppDbContext.cs`
- `DbInitializer.cs`
- `DbPathProvider.cs`

### Models

Contient les entités métier principales.

- `Client`
- `User`
- `SocieteUser`
- `Activity`
- `BaseEntity`

### Services

Contient la logique applicative et les accès métier.

Exemples :
- authentification
- gestion des activités
- session utilisateur

### ViewModels

Contient la logique de présentation.

Principaux ViewModels :

- `AuthentificationViewModel` : écran de connexion
- `MainMenuViewModel` : menu principal
- `ClientSelectionViewModel` : sélection client
- `AdminAuditViewModel` : administration des audits
- `AdminPersonnelViewModel` : administration du personnel
- `CreateClientViewModel` : création/modification client
- `UserEditDialogViewModel` : édition utilisateur

### Views

Contient les écrans WPF XAML.

Principales vues :

- `AuthentView`
- `MainMenu`
- `ClientSelectionView`
- `AdminAuditView`
- `AdminPersonnelView`
- `NewAuditView`
- `ResumeAuditView`
- `ClientEditDialog`
- `UserEditDialog`
- `ChangePasswordDialog`

## Pattern architectural

Le projet suit une approche MVVM :

- les `Views` définissent l'interface graphique ;
- les `ViewModels` exposent les propriétés et commandes ;
- les `Services` portent la logique métier ;
- les `Models` représentent les données ;
- `Data` gère la persistance locale.

## Règles importantes

- Ne pas mettre de logique métier dans les fichiers `.xaml.cs`.
- Les Views doivent rester centrées sur l'affichage.
- Les ViewModels doivent utiliser des services plutôt qu'accéder directement à la base.
- Les commandes doivent passer par `RelayCommand` ou équivalent.
- Les modifications doivent respecter l'organisation existante.