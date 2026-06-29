# DungeonQuest

Gioco di avventura testuale in C# per console (.NET 10). I dati sono persistenti su **MySQL** tramite **Entity Framework Core 10** (Pomelo).

## Requisiti

- [Docker](https://www.docker.com/) (consigliato)
- Oppure .NET SDK 10 + MySQL 8.0 in locale su `localhost:3306`

## Come eseguire

### Con Docker (consigliato)

```bash
docker compose up -d             # avvia MySQL + app (sfondo)
docker compose logs app -f       # segui l'output del gioco
docker compose run app           # esegui in primo piano
docker compose down -v           # ferma e cancella volumi (reset DB)
```

La prima esecuzione scarica le immagini e crea il database automaticamente.
MySQL è esposto sulla porta **3307** dell'host per evitare conflitti.

### Senza Docker (locale)

```bash
# Crea il database MySQL manualmente
mysql -u root -p -e "CREATE DATABASE dungeonquest;"

# Esegui
dotnet run --project DungeonQuest
```

Per visualizzare i dati con TablePlus / MySQL Workbench:
- **Host**: `localhost`
- **Port**: `3307`
- **User**: `root`
- **Password**: `root`
- **Database**: `dungeonquest`

## Gameplay

### Selezione eroe
All'avvio il gioco mostra tutti gli eroi salvati. Puoi crearne di nuovi, eliminarli o selezionarne uno esistente.

### Menu principale
1. Aggiungi un'arma all'arsenale
2. Mostra inventario
3. Mostra l'arsenale per tipo
4. Cerca un'arma ed equipaggiala
5. Modifica un'arma
6. Combatti contro un nemico
7. Salva l'arsenale su file (CSV)
0. Torna al menu principale

### Combattimento
Scontro a turni tra eroe e nemico (Goblin, Scheletro, Drago). Il drago appare dal livello 2 in poi con probabilità crescente.

| Livello | Drago | Scheletro | Goblin |
|---------|-------|-----------|--------|
| 1       | 0%    | 50%       | 50%    |
| 2       | 5%    | 47.5%     | 47.5%  |
| ...     | ...   | ...       | ...    |
| 10      | 45%   | 27.5%     | 27.5%  |

Opzioni in combattimento:
- **Attacca** — colpisci il nemico (possibile chance di mancare)
- **Fuggi** — torni al menu
- **Usa pozione** — recupera 50% HP massimi (non consumata se HP già al massimo)

### Pozioni
- Drop dopo vittoria: Goblin/Scheletro 15%, Drago 70%
- Le pozioni sono persistenti su DB

## Architettura

```
DungeonQuest/
├── Program.cs              ← entry point
├── Models/
│   ├── Hero.cs             ← eroe (max livello 10)
│   ├── Enemy.cs            ← nemico astratto
│   ├── Goblin.cs / Skeleton.cs / Dragon.cs
│   ├── EnemyFactory.cs       ← factory registry per generare nemici
│   ├── WeaponType.cs         ← value object (sostituisce enum)
│   └── WeaponRarity.cs       ← value object (sostituisce enum)
│   ├── Weapon.cs           ← arma con tipo, rarità, danno
│   ├── Potion.cs           ← pozione curativa
│   ├── Spell.cs            ← spell (reserved)
│   └── *AttackBehavior.cs  ← 3 implementazioni attacco nemico
├── Interfaces/
│   └── IExporter.cs         ← interfaccia esportazione (Strategy)
├── Services/
│   ├── CombatManager.cs     ← loop combattimento a turni
│   ├── ArsenalManager.cs    ← gestione armi, pozioni
│   ├── CsvExporter.cs       ← esportazione CSV (IExporter)
│   └── JsonExporter.cs      ← esportazione JSON (IExporter)
├── UI/
│   ├── GraphicsHelper.cs   ← stili, health bar, ASCII art
│   ├── MenuManager.cs      ← menu principale
│   └── Welcome.cs          ← creazione nuovo eroe
├── db/
│   └── DungeonContext.cs   ← DbContext EF Core (MySQL)
└── Exceptions/             ← gerarchia eccezioni (armi, export)
```

### Database
Entity Framework Core 10 con MySQL (Pomelo). Lo schema è creato automaticamente via `EnsureCreated()` — non servono migrazioni manuali.

### Docker
`docker-compose.yml` con due servizi: `db` (MySQL 8.0, porta 3307) e `app` (.NET 10).
Credenziali configurabili nel file `.env`.

### Design pattern
- **Strategy** (`IAttackBehavior`): tre comportamenti d'attacco per i nemici
- **Template Method** (`Enemy`): classe astratta con sottoclassi concrete
- **Registry** (`EnemyFactory`): registro estensibile per generare nemici in modo OCP-compliant
- **Value Object** (`WeaponType`, `WeaponRarity`): classi aperte all'estensione (sostituiscono enum) con registry interno
- **Strategy** (`IExporter`): esportatori intercambiabili (CSV, JSON) selezionabili a runtime
