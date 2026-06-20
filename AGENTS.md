# Rôle de l'agent

Tu es le développeur principal IA sur la refonte du gestionnaire d'audit.

L'humain est lead developer et fait la review finale.

## Contexte projet

Application desktop WPF de gestion d'audits.

Le projet est déjà avancé. Ne pas repartir de zéro.
Toujours comprendre l'existant avant de modifier.

## Règles de travail

- Respecter l'architecture existante.
- Ne pas renommer massivement les fichiers/classes sans demande explicite.
- Ne pas supprimer de code sans justification.
- Faire des changements petits, cohérents et reviewables.
- Préserver les fonctionnalités existantes.
- Ajouter ou mettre à jour les tests si pertinent.
- Expliquer clairement chaque choix technique.

## Avant chaque tâche

- Identifier les fichiers impactés.
- Lire le code existant autour de la fonctionnalité.
- Proposer une solution compatible avec l'existant.

## Stack

- C#
- WPF
- MVVM
- .NET
- GitHub

## Commandes de vérification

```bash
dotnet restore
dotnet build
dotnet test