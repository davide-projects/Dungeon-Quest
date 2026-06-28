# ⚔️ DungeonQuest

Un gioco di ruolo testuale a turni sviluppato in **C# / .NET**, con persistenza dei dati tramite **Entity Framework Core** e un'interfaccia a menu interattiva da terminale.
 
---

## 🗂️ Struttura del progetto

```
DungeonQuest/
├── db/
│   └── DungeonContext.cs       # DbContext EF Core
├── Models/
│   ├── Hero.cs                 # Modello eroe
│   ├── Potion.cs               # Modello pozione
│   └── Weapon.cs               # Modello arma
├── UI/
│   ├── GraphicsHelper.cs       # Rendering ASCII / titoli / separatori
│   ├── MenuManager.cs          # Gestione del menu principale
│   └── Welcome.cs              # Schermata di benvenuto
└── Program.cs                  # Entry point
```
 
---

## 🚀 Avvio rapido

### Prerequisiti

- [.NET 8 SDK](https://dotnet.microsoft.com/download) o superiore
- EF Core CLI tools (`dotnet-ef`)
### Installazione

```bash
git clone https://github.com/tuonome/DungeonQuest.git
cd DungeonQuest
dotnet restore
dotnet ef database update
dotnet run
```
 
---

## 🎮 Funzionalità

### Selezione eroe

All'avvio, il gioco mostra tutti gli eroi salvati nel database. Da qui puoi:

| Comando | Azione |
|---------|--------|
| `1`–`N` | Seleziona un eroe esistente |
| `C` | Crea un nuovo eroe |
| `E` | Elimina un eroe |
| `0` | Esci dal gioco |

### Eroe morto

Se selezioni un eroe con **HP ≤ 0**, il gioco ti chiede se vuoi resettarlo:

- Confermi con `s` → inventario azzerato (pozioni e armi rimosse), eroe ripristinato allo stato iniziale
- Annulli → torni al menu di selezione
### Arma equipaggiata

Quando carichi un eroe esistente, l'arma equipaggiata viene caricata automaticamente via lazy loading (`Reference(...).LoadAsync()`).
 
---

## 🗄️ Database

Il progetto usa **SQLite** (predefinito) tramite Entity Framework Core. Le migrazioni vengono applicate automaticamente all'avvio:

```csharp
db.Database.Migrate();
```

### Entità principali

- **Hero** — nome, HP, statistiche, arma equipaggiata
- **Weapon** — appartiene a un eroe (`HeroId`)
- **Potion** — appartiene a un eroe (`HeroId`)
---

## 🧱 Architettura

```
Program.cs
│
├── DungeonContext          ← EF Core (SQLite)
│
├── GraphicsHelper          ← Titoli ASCII, separatori, messaggi
├── Welcome                 ← Input nome nuovo eroe
│
└── MenuManager             ← Loop di gioco principale
```

Il flusso principale è un **doppio ciclo `while(true)`**:

1. **Ciclo esterno** — riavvia l'intera sessione dopo che un eroe finisce la sua avventura
2. **Ciclo interno** — gestisce la selezione / creazione / eliminazione dell'eroe
---

## 🛠️ Tecnologie

| Tecnologia | Utilizzo |
|-----------|----------|
| C# / .NET 8 | Linguaggio e runtime |
| Entity Framework Core | ORM + migrazioni |
| SQLite | Database locale |
| LINQ async | Query asincrone sul DB |
 
---

## 📝 Licenza

Distribuito sotto licenza **MIT**. Vedi [`LICENSE`](LICENSE) per i dettagli.