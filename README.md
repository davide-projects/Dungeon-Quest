# DungeonQuest

Gioco di avventura testuale in C# per console (.NET 9.0). I dati sono persistenti su **MySQL** tramite **Entity Framework Core 9** (Pomelo).

## Requisiti

- [Docker](https://www.docker.com/) (consigliato)
- Oppure .NET SDK 9.0 + MySQL 8.0 in locale su `localhost:3306`

## Come eseguire

### Con Docker (consigliato)

```bash
docker compose up --build -d     # avvia MySQL + app (sfondo)
docker compose logs app -f       # segui l'output del gioco
docker compose run app           # esegui in primo piano
docker compose down -v           # ferma e cancella volumi
```

La prima esecuzione scarica le immagini e crea il database automaticamente.
Le migration EF Core vengono applicate all'avvio dell'app.
MySQL è esposto sulla porta **3308** dell'host per evitare conflitti.
I dati persistono nel volume `mysql_data`.

### Senza Docker (locale)

```bash
# Crea il database MySQL manualmente
mysql -u root -p -e "CREATE DATABASE dungeonquest;"

# Esegui
dotnet run --project DungeonQuest
```

## Flusso di gioco

```
Avvio
  └── Selezione eroe (o creazione/eliminazione)
       └── MenuManager.Run() → loop menu principale
            └── 0 → ritorna alla selezione eroe
```

### Schermata iniziale — Selezione eroe

```
  EROI DISPONIBILI
    1) Aris — Liv.3 | HP: 28/50 | Att: 12 | Oro: 45
    2) Luna — Liv.1 | HP: 0/30  | Att: 5  | Oro: 0   (Sconfitto!)

    C) Crea un nuovo eroe
    E) Elimina un eroe
    0) Esci
```

- Se un eroe è **morto** (HP=0), selezionarlo chiede se resettarlo (torna Liv.1, perde armi e pozioni)
- Se rifiuta, torna alla lista
- **Crea** (C) → inserisci nome, parte da Liv.1
- **Elimina** (E) → rimuove eroe, armi e pozioni

### Menu principale (7 opzioni)

```
   1) Aggiungi un'arma all'arsenale
   2) Mostra inventario
   3) Mostra l'arsenale per tipo
   4) Cerca un'arma ed equipaggiala
   5) Modifica un'arma
   6) Combatti contro un nemico
   7) Salva l'arsenale su file (CSV)
   0) Torna al menu principale
```

---

## 1) Aggiungi arma

Inserisci nome (solo lettere), tipo, danno (>0), rarità.

### WeaponType enum

| Valore | Nome    |
|--------|---------|
| 1      | Spada   |
| 2      | Arco    |
| 3      | Ascia   |
| 4      | Bastone |
| 5      | Pugnale |

### WeaponRarity enum

| Valore | Nome       |
|--------|------------|
| 0      | Comune     |
| 1      | Non Comune |
| 2      | Raro       |
| 3      | Epico      |
| 4      | Leggendario|

L'arma viene salvata su DB (`Weapons`).

---

## 2) Mostra inventario

Stampa tutte le armi e le **pozioni** possedute dall'eroe.

---

## 3) Mostra arsenale per tipo

Filtra armi per tipo.

---

## 4) Cerca ed equipaggia

Cerca per nome o parte del nome (`LIKE %text%`).

- 0 risultati → errore
- 1 risultato → equipaggiato automaticamente
- 2+ risultati → elenco numerato, utente sceglie

Equipaggiare aggiorna il campo `EquippedByHeroId` sulla tabella `Weapons`.

---

## 5) Modifica un'arma

Cerca, sceglie, modifica nome/tipo/danno/rarità. Se l'arma era equipaggiata, il riferimento viene aggiornato in memoria.

---

## 6) Combatti

Genera nemico casuale tra Goblin, Scheletro, Drago.

### Combat loop — a turni

```
while (hero.IsAlive && enemy.IsAlive)
  ├── WriteCombatHeader()        ←  hero HP bar + enemy HP bar
  ├── GetEnemyArt()              ←  ASCII art del nemico
  ├── input: "1" Attacca / "2" Fuggi / "3" Usa pozione (N)
  │
  ├── "3" → consuma pozione, recupera 50% MaxHp, turno passato
  ├── "2" → return CombatResult.Flee
  ├── "1" →
  │     └── CalculateHeroDamage(hero)
  │           ├── miss?  → return 0
  │           └── hit!   → return danno casuale 80%-120% di AttackPower
  │
  ├── enemy.AttackBehavior.Execute(enemy.AttackPower)
  │     ├── NormalAttackBehavior        → colpisce sempre
  │     ├── UnreliableAttackBehavior    → 50% manca
  │     └── FireBreathAttackBehavior    → 25% danno doppio
  │
  └── Pausa()
```

### Probabilità di mancare dell'eroe

Decresce col livello: `missChance = Math.Max(2, 20 - (level - 1) * 2)`

### Drop pozioni

- Goblin / Scheletro: 15%
- Drago: 70%

### Esito

- **Vittoria** → oro + XP + possibile pozione, salvato su DB
- **Sconfitta** → HP = 0, eroe morto
- **Fuga** → si torna al menu

### Nemici

| Nemico    | HP | Att | Ricompensa    | Comportamento                     |
|-----------|----|-----|---------------|------------------------------------|
| Goblin    | 12 | 4   | 5 oro, 15 XP  | Colpisce sempre                    |
| Scheletro | 18 | 6   | 10 oro, 25 XP | 50% manca                          |
| Drago     | 40 | 10  | 30 oro, 60 XP | 25% soffio (danno doppio)          |

---

## 7) Salva CSV

Esporta le armi in `arsenale.csv`.

---

---

## Docker

Il progetto include Docker Compose con due servizi:

| Servizio | Immagine | Porta host | Ruolo |
|----------|----------|------------|-------|
| `db`     | `mysql:8.0` | `3308` → `3306` | Database MySQL con healthcheck |
| `app`    | `dungeonquest-app` (build locale) | — | Applicazione console .NET 9 |

La connessione al DB avviene tramite la variabile d'ambiente `CONNECTION_STRING`.
In Docker punta a `server=db;port=3306`; in locale fallback su `localhost:3306`.

I file Docker sono nella root del repository:

```
DungeonQuest/
├── docker-compose.yml       ← orchestrazione servizi
├── .dockerignore
└── DungeonQuest/
    └── Dockerfile           ← multi-stage build .NET
```

## Architettura

```
DungeonQuest/
├── Program.cs              ← entry point, loop selezione eroi
├── Interfaces/
│   ├── ICombatant.cs       ← interfaccia combattente (Hero, Enemy)
│   └── IAttackBehavior.cs  ← strategy pattern per attacchi
├── Models/
│   ├── Hero.cs             ← eroe (livello, XP, level-up, equip, pozioni)
│   ├── Enemy.cs            ← nemico astratto
│   ├── Goblin.cs / Skeleton.cs / Dragon.cs
│   ├── Weapon.cs / WeaponType.cs / WeaponRarity.cs
│   ├── Potion.cs           ← pozione curativa
│   ├── Spell.cs            ← incantesimo
│   └── *AttackBehavior.cs  ← 3 implementazioni attacco nemico
├── Services/
│   ├── CombatManager.cs    ← loop combattimento a turni
│   └── ArsenalManager.cs   ← CRUD armi su DB + pozioni su DB
├── UI/
│   ├── Benvenuto.cs        ← schermata benvenuto
│   ├── GraphicsHelper.cs   ← stili, box, health bar, ASCII art
│   └── MenuManager.cs      ← menu principale e orchestrazione
├── db/
│   └── DungeonContext.cs   ← DbContext EF Core (MySQL)
├── Migrations/             ← migration EF Core generate
│   ├── RaritaArma.cs       ← aggiunta colonna Rarity
│   ├── RinominaHeroId1.cs  ← rinominata FK HeroId1 → EquippedByHeroId
│   └── TipiDa1.cs          ← WeaponType rinumerato da 1
└── Exceptions/
    ├── InvalidWeaponException.cs           ← base
    ├── InvalidWeaponNameException.cs
    ├── InvalidWeaponDamageException.cs
    ├── InvalidWeaponTypeException.cs
    ├── InvalidWeaponRarityException.cs
    └── WeaponNotFoundException.cs
```

### Design pattern

- **Strategy** (`IAttackBehavior`): attacco intercambiabile per nemico
- **Template Method** (`Enemy`): base astratta, sottoclassi concrete
- **Gerarchia eccezioni**: cattura specifica nel menu

---

## Database

Entity Framework Core 9 con MySQL (Pomelo).

### Migrations

```bash
dotnet ef migrations add <NomeMigration>
dotnet ef database update
```

### Schema

| Tabella    | Colonne                                                                 |
|------------|-------------------------------------------------------------------------|
| `Heroes`   | `Id` (PK, auto), `Name`, `Level`, `MaxHp`, `Hp`, `Xp`, `Gold`, `BaseAttack` |
| `Weapons`  | `Code` (PK, auto), `Name`, `Type`, `Damage`, `Rarity`, `HeroId` (FK → Heroes.Id), `EquippedByHeroId` (FK → Heroes.Id, unique) |
| `Potions`  | `Id` (PK, auto), `Name`, `HeroId` (FK → Heroes.Id, nullable) |

### Relazioni

- **Hero → Weapon (Weapons)**: 1 a N, FK `Weapon.HeroId`, delete `SetNull`
- **Hero → Weapon (Equipped)**: 1 a 1, FK `Weapon.EquippedByHeroId`, delete `SetNull`
- **Hero → Potion**: 1 a N, FK `Potion.HeroId`, delete `SetNull`

### Persistenza stato eroe

HP, XP, Oro, Level, MaxHp, BaseAttack sono salvati su DB e caricati a ogni avvio.
Se l'eroe sopravvive (HP > 0) alla chiusura, riparte esattamente da dove aveva lasciato.
Se muore (HP = 0), la prossima selezione offre il reset completo.

## Note

- `Weapon.Code` è auto-increment su DB (`ValueGeneratedOnAdd`)
- `Potion` è persistita su DB (migrazione `Pozioni`)
- Le eccezioni `InvalidWeaponTypeException` e `InvalidWeaponRarityException` validano i valori enum
- Warning CS8618 su `Hero.Name` / `Potion.Name` è preesistente (private set in costruttore EF) — non bloccante
- `ServerVersion` è fissato a `8.0.46-mysql` (non in AutoDetect) per compatibilità Docker
