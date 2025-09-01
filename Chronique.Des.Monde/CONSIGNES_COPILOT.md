# 🧠 Consignes de Développement pour GitHub Copilot

## 🎯 Conventions de code à respecter

- **Pas de underscore** (`_xxx`) pour les variables privées ou membres.
- **Toujours utiliser `this.`** pour accéder aux membres de la classe (propriétés, méthodes, champs).
- **Pas de variables en camelCase** pour les propriétés publiques : utiliser PascalCase.
- **Utiliser PascalCase** pour les noms de classes, méthodes, propriétés, enums, DTOs, etc.
- **Utiliser camelCase** pour les variables locales et paramètres de méthode.
- **Pas de code mort** ou de TODO non justifié dans les livrables.
- **Respecter l’architecture** : séparer Business, Data, API, Models, Services.
- **Toujours valider les permissions** côté API/business pour toute action sensible.
- **Préférer les exceptions métier** (`BusinessException`) pour la logique métier.
- **Respecter la structure des dossiers** (Business, Models, Services, Endpoints, etc).
- **Prioriser Blazor** pour l’UI si besoin d’interface.
- **Pour les services de fond** : utiliser `BackgroundService` si Worker Service.
- **Cibler .NET 9** pour tous les nouveaux projets/fichiers.
- **Documenter** les endpoints et services complexes.
- **Utiliser les logs** pour les actions importantes (ex : envoi d’email, erreurs critiques).

## 🛠️ Bonnes pratiques générales

- **Commit message** : explicite, mentionner le ticket ADO si possible (ex : `Fixes AB#160`).
- **Documentation** : ajouter un résumé Markdown pour chaque fonctionnalité majeure.
- **Tests** : valider le build après chaque modification majeure.
- **Respecter les conventions C#** (visibilité, noms, etc).
- **Pas de code dupliqué** : factoriser si besoin.
- **Utiliser l’injection de dépendances** partout (services, business, etc).
- **Pas de hardcoding de secrets** (utiliser la config/appsettings).

## 🔄 Rappels pour la continuité

- **Copier ce fichier sur chaque nouvelle machine** ou le garder dans le repo.
- **Mettre à jour ce fichier** si de nouvelles conventions sont décidées.
- **Se référer à ce fichier** pour toute question de style ou d’architecture.

## Règles Générales

- Toujours suivre l'architecture du projet existant.
- Prendre exemple sur l'existant pour toute nouvelle fonctionnalité ou modification.
- Toujours privilégier la cohérence avec le code et la structure déjà présents.

---

**Ce fichier sert de référence pour GitHub Copilot et tout développeur du projet.**

---

*Dernière mise à jour : 2025-08-24*