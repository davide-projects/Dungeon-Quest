using System.ComponentModel.DataAnnotations.Schema;
using DungeonQuest.Utilities;

namespace DungeonQuest.Models;

[Table("incantesimi")]
public class Spell
{
    private string _nome = string.Empty;
    private int _danno;

    [Column("id")]
    public int Id { get; }

    [Column("nome")]
    public string Name
    {
        get => _nome;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Il nome dell'incantesimo non può essere vuoto.");

            _nome = value.Trim();
        }
    }

    [Column("danno")]
    public int Damage
    {
        get => _danno;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Il danno dell'incantesimo deve essere maggiore di zero.");

            _danno = value;
        }
    }

    [Column("appreso")]
    public bool IsLearned { get; set; }

    public Spell(string name, int damage)
    {
        Id = IdGenerator.NextSpellId();
        Name = name;
        Damage = damage;
        IsLearned = false;
    }

    public override string ToString()
    {
        string stato = IsLearned ? "appreso" : "non appreso";
        return $"#{Id} {Name} — danno {Damage} ({stato}).";
    }
}
