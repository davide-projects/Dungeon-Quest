# DungeonQuest

Gioco di avventura testuale in C# per console (.NET 10.0).

## Come eseguire

```bash
dotnet run --project DungeonQuest
```

## Flusso di gioco

```
Avvio
  └── Benvenuto.ChiediNome()  →  inserisci nome eroe (obbligatorio, auto-maiuscolo)
       └── new Hero(nome)
            └── MenuManager.Run()  →  loop menu principale
```

### Menu principale (7 opzioni)

```
┌─────────────────────────────────────────────────┐
│         § § §  DUNGEON QUEST  § § §             │
│           >>  Avventura testuale  <<            │
├─────────────────────────────────────────────────┤
│  Eroe — Liv.1 | HP: 30/30 | Att: 5 | ...        │
├─────────────────────────────────────────────────┤
│  1) Aggiungi un'arma all'arsenale               │
│  2) Mostra tutto l'arsenale                     │
│  3) Mostra l'arsenale per tipo                  │
│  4) Cerca un'arma ed equipaggiala               │
│  5) Combatti contro un nemico                   │
│  6) Salva l'arsenale su file (CSV)              │
│  0) Esci                                        │
└─────────────────────────────────────────────────┘
```

Ogni opzione richiede conferma con INVIO. Il menu si ripresenta finché non si sceglie 0.

---

## 1) Aggiungi arma

```
Nome arma: [solo lettere, prima lettera maiuscola automatica]
Tipo: 1=Spada  2=Arco  3=Ascia  4=Bastone  5=Pugnale
Danno: [numero > 0]
```

- Il nome viene validato: `char.IsLetter` per ogni carattere
- L'arma riceve un `Code` univoco progressivo (tramite `IdGenerator`)
- Viene aggiunta alla lista in memoria (`ArsenalManager._weapons`)
- I codici partono da 1 e non si resettano mai, neppure dopo aver ricaricato da CSV

---

## 2) Mostra tutto l'arsenale

Stampa in formato: `#Codice Nome (Tipo) — danno N.`

---

## 3) Mostra arsenale per tipo

Filtra per tipo di arma tra: `Spada`, `Arco`, `Ascia`, `Bastone`, `Pugnale`.

---

## 4) Cerca ed equipaggia

Cerca per nome o parte del nome (case-insensitive, `Contains`).

- **0 risultati**: messaggio di errore
- **1 risultato**: equipaggiato automaticamente
- **2+ risultati**: mostra elenco numerato, l'utente sceglie quale equipaggiare (0 per annullare)

L'attacco dell'eroe diventa: `BaseAttack + Arma.Damage`

---

## 5) Combatti

```
MenuManager.Combatti()
  ├── se eroe morto (Hp == 0) → errore, ritorno al menu
  ├── GeneraNemicoCasuale() → random tra Goblin, Scheletro, Drago
  ├── mostra incontro (ASCII art + testo)
  └── CombatManager.Fight(hero, enemy)
```

### Combat loop — a turni

```
CombatManager.Fight()
  │
  ├── while (hero.IsAlive && enemy.IsAlive)
  │     │
  │     ├── WriteCombatHeader()        ←  hero HP bar + enemy HP bar
  │     ├── GetEnemyArt()              ←  ASCII art del nemico
  │     ├── input: "1" Attacca / "2" Fuggi
  │     │
  │     ├── se "2" → WriteFlee()
  │     │              return CombatResult.Flee
  │     │
  │     ├── se "1" →
  │     │     └── CalculateHeroDamage(hero)
  │     │           ├── miss?     → return 0   (messaggio "manca il bersaglio")
  │     │           └── hit!      → return danno casuale 80%-120% di AttackPower
  │     │
  │     ├── enemy.AttackBehavior.Execute(enemy.AttackPower)
  │     │     ├── NormalAttackBehavior     → colpisce sempre (danno = AttackPower)
  │     │     ├── UnreliableAttackBehavior → 50% manca, 50% colpisce
  │     │     └── FireBreathAttackBehavior → 25% danno doppio, 75% danno normale
  │     │
  │     └── Pausa()  (INVIO per continuare)
  │
  └── return CombatResult.{Victory, Defeat, Flee}
```

### Probabilità di mancare dell'eroe

Decresce col livello:

| Liv. | % Miss |
|------|--------|
| 1    | 20%    |
| 2    | 18%    |
| 3    | 16%    |
| 4    | 14%    |
| 5    | 12%    |
| 6    | 10%    |
| 7    | 8%     |
| 8    | 6%     |
| 9    | 4%     |
| 10   | 2%     |

Formula: `missChance = Math.Max(2, 20 - (level - 1) * 2)`

### Esito del combattimento

- **Vittoria**: l'eroe riceve oro e XP, se l'XP supera `livello * 100` sale di livello
  - Level up: HP pieni, MaxHp+10, BaseAttack+2
- **Sconfitta**: l'eroe muore (Hp=0), non può più combattere finché non si riavvia il programma
- **Fuga**: si torna al menu senza ricompense

### Nemici

| Nemico     | HP  | Att | Ricompensa      | Comportamento                           |
|------------|-----|-----|-----------------|-----------------------------------------|
| Goblin     | 12  | 4   | 5 oro, 15 XP    | Colpisce sempre                         |
| Scheletro  | 18  | 6   | 10 oro, 25 XP   | 50% manca il bersaglio                  |
| Drago      | 40  | 10  | 30 oro, 60 XP   | 25% soffio infuocato (danno doppio)     |

---

## 6) Salva CSV

Salva l'arsenale corrente in `arsenale.csv` (codice;nome;tipo;danno). Il file viene caricato automaticamente all'avvio del programma.

Il caricamento preserva i codici originali e aggiorna `IdGenerator` al `maxCode+1`, in modo che le nuove armi non creino conflitti.

---

## Architettura

```
DungeonQuest/
├── Program.cs              ← entry point
├── Interfaces/
│   ├── ICombatant.cs       ← interfaccia combattente (Hero, Enemy)
│   └── IAttackBehavior.cs  ← strategy pattern per attacchi
├── Models/
│   ├── Hero.cs             ← eroe (livello, XP, level-up, arma equipaggiata)
│   ├── Enemy.cs            ← nemico astratto
│   ├── Goblin.cs / Skeleton.cs / Dragon.cs
│   ├── Weapon.cs / WeaponType.cs
│   └── *AttackBehavior.cs  ← 3 implementazioni attacco nemico
├── Services/
│   ├── CombatManager.cs    ← loop combattimento a turni
│   └── ArsenalManager.cs   ← gestionale arsenale + CSV save/load
├── UI/
│   ├── Benvenuto.cs        ← schermata iniziale (richiesta nome)
│   ├── GraphicsHelper.cs   ← stili, box, health bar, ASCII art
│   └── MenuManager.cs      ← menu principale e flusso
├── Utilities/
│   └── IdGenerator.cs      ← contatore statico per codici arma
└── Exceptions/             ← gerarchia eccezioni personalizzate
    ├── DungeonQuestException.cs
    ├── InvalidWeaponException.cs
    ├── InvalidWeaponNameException.cs
    ├── InvalidWeaponDamageException.cs
    └── WeaponNotFoundException.cs
```

### Design pattern usati

- **Strategy** (`IAttackBehavior`): comportamento d'attacco intercambiabile per ogni tipo di nemico
- **Template Method** (`Enemy`): base astratta con costruttore comune, sottoclassi concrete
- **Gerarchia eccezioni**: cattura specifica a livello menu per messaggi utente chiari
- **Persistenza CSV**: salvataggio/caricamento arsenale con preservazione codici
