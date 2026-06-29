# 🗡️ DungeonQuest

Gioco di avventura testuale in C# per console (.NET 10). I dati sono persistenti su **MySQL** tramite **Entity Framework Core 10** (Pomelo).

## 📋 Requisiti

- [Docker](https://www.docker.com/) (consigliato)
- Oppure .NET SDK 10 + MySQL 8.0 in locale su `localhost:3306`
## 🚀 Come eseguire

### 🐳 Con Docker (consigliato)

```bash
docker compose up -d             # avvia MySQL + app (sfondo)
docker compose logs app -f       # segui l'output del gioco
docker compose run app           # esegui in primo piano
docker compose down -v           # ferma e cancella volumi (reset DB)
```

La prima esecuzione scarica le immagini e crea il database automaticamente.
MySQL è esposto sulla porta configurata in `.env` (`MYSQL_PORT`, default 3307).

### 💻 Senza Docker (locale)

```bash
# Crea il database MySQL manualmente
mysql -u root -p -e "CREATE DATABASE dungeonquest;"
 
# Esegui
dotnet run --project DungeonQuest
```

Per visualizzare i dati con TablePlus / MySQL Workbench:
- **Host**: `localhost`
- **Port**: la stessa di `MYSQL_PORT` in `.env`
- **User**: `root`
- **Password**: `root`
- **Database**: `dungeonquest`
## 🎮 Gameplay

### 🧙 Selezione eroe
All'avvio il gioco mostra tutti gli eroi salvati. Puoi crearne di nuovi, eliminarli o selezionarne uno esistente.

### 📜 Menu principale
```
   R) Raccogli la tua prima arma!     (solo livello 1, una volta)
 
   1) Mostra inventario
   2) Mostra l'arsenale per tipo
   3) Cerca un'arma ed equipaggiala
   4) Negozio                          (compra armi con l'oro)
   5) Combatti contro un nemico
   6) Esporta l'arsenale su file
   0) Torna al menu principale
 
   B) AFFRONTA IL BOSS FINALE!         (solo dal livello 10)
```

- **Raccogli arma** — ottieni un'arma Comune casuale all'avvio (una tantum)
- **Negozio** — acquista armi dal catalogo (22 armi in 5 rarità) con l'oro guadagnato
- **Esporta** — salva arsenale in CSV o JSON (formato selezionabile)
### ⚔️ Combattimento
Scontro a turni tra eroe e nemico. I nemici vengono generati con probabilità variabile in base al livello dell'eroe.

| Livello | Goblin | Orc | Uruk-hai |
|---------|--------|-----|----------|
| 1       | 57%    | 43% | —        |
| 4       | 44%    | 33% | 22%      |
| 10      | 44%    | 33% | 22%      |

Opzioni in combattimento:
- **Attacca** — colpisci il nemico (possibile chance di mancare in base al livello)
- **Fuggi** — torni al menu
- **Usa pozione** — recupera 50% HP massimi (non consumata se HP già al massimo)
### 👹 Nemici

| Nemico | Livello min | HP | Att | Oro | XP | Pozione | Comportamento |
|--------|-------------|----|-----|-----|----|---------|--------------|
| Goblin | 1 | 12 | 4 | 5 | 15 | 15% | Attacco normale |
| Orc | 1 | 22 | 7 | 12 | 30 | 20% | Attacco normale |
| Uruk-hai | 4 | 32 | 9 | 22 | 50 | 35% | 25% critico; rigenera 10 HP all'eroe alla vittoria |
| **Sauron** | 10 (boss) | 150 | 15 | 200 | 500 | 100% | 70% critico, 4% miss |

### 🧪 Pozioni
- Drop dopo vittoria in base alla percentuale del nemico
- Le pozioni sono persistenti su DB
- Curano il 50% della salute massima
### ✨ Bonus rarità arma
Le armi nel negozio hanno un bonus attacco in base alla rarità:

| Rarità | Bonus attacco |
|--------|:------------:|
| Comune | +0 |
| Non Comune | +1 |
| Raro | +2 |
| Epico | +3 |
| Leggendario | +4 |

L'attacco totale dell'eroe è: `Attacco base + Danno arma + Bonus rarità`.

### 🔥 Boss finale (Sauron)
Al livello 10 appare l'opzione **B) AFFRONTA IL BOSS FINALE!** nel menu. Richiede conferma prima del combattimento. Sauron ha statistiche elevate (150 HP, 15 Att), 70% di colpo critico e droppa sempre una pozione. Sconfiggerlo mostra una schermata di vittoria con ASCII art e la scritta "FINE GIOCO", dopodiché il programma termina.

## 🏗️ Architettura

```
DungeonQuest/
├── Program.cs              ← entry point
├── Models/
│   ├── Hero.cs             ← eroe (max livello 10)
│   ├── Enemy.cs            ← nemico astratto
│   ├── Goblin.cs / Orc.cs / UrukHai.cs / Sauron.cs
│   ├── EnemyFactory.cs       ← factory registry per generare nemici
│   ├── WeaponType.cs         ← value object (sostituisce enum)
│   ├── WeaponRarity.cs       ← value object (sostituisce enum), con BonusAttack
│   ├── Weapon.cs           ← arma con tipo, rarità, danno
│   ├── WeaponCatalog.cs    ← catalogo statico con 22 armi predefinite
│   ├── Potion.cs           ← pozione curativa
│   ├── Spell.cs            ← spell (reserved)
│   └── *AttackBehavior.cs  ← 4 implementazioni attacco nemico
├── Interfaces/
│   ├── IExporter.cs         ← interfaccia esportazione (Strategy)
│   ├── ICombatant.cs        ← interfaccia combattente
│   └── IAttackBehavior.cs   ← interfaccia comportamento d'attacco
├── Services/
│   ├── CombatManager.cs     ← loop combattimento a turni
│   ├── ArsenalManager.cs    ← gestione armi, pozioni
│   ├── ShopManager.cs       ← logica negozio (catalogo, acquisto)
│   ├── CsvExporter.cs       ← esportazione CSV (IExporter)
│   └── JsonExporter.cs      ← esportazione JSON (IExporter)
├── UI/
│   ├── GraphicsHelper.cs   ← stili, health bar, output colorato, schermata vittoria boss
│   ├── MenuManager.cs      ← menu principale
│   └── Welcome.cs          ← creazione nuovo eroe
├── db/
│   └── DungeonContext.cs   ← DbContext EF Core (MySQL)
└── Exceptions/             ← gerarchia eccezioni (armi, export)
```

### 🗄️ Database
Entity Framework Core 10 con MySQL (Pomelo). Lo schema è creato automaticamente via `EnsureCreated()` — non servono migrazioni manuali.

### 🐳 Docker
`docker-compose.yml` con due servizi: `db` (MySQL 8.0) e `app` (.NET 10).
Credenziali configurabili nel file `.env`.

### 🧩 Design pattern
- **Strategy** (`IAttackBehavior`): 4 comportamenti d'attacco intercambiabili per i nemici
- **Template Method** (`Enemy`): classe astratta con sottoclassi concrete e hook `OnDefeated()`
- **Registry** (`EnemyFactory`): registro estensibile per generare nemici in modo OCP-compliant
- **Value Object** (`WeaponType`, `WeaponRarity`): classi aperte all'estensione (sostituiscono enum) con registry interno
- **Strategy** (`IExporter`): esportatori intercambiabili (CSV, JSON) selezionabili a runtime
- **Catalog** (`WeaponCatalog`): catalogo statico di tutte le armi disponibili nel gioco