using System.ComponentModel.DataAnnotations.Schema;

namespace DungeonQuest.Models;

[Table("spells")]
public class Spell
{
    private string _name = string.Empty;
    private int _damage;

    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Spell name cannot be empty.");

            _name = value.Trim();
        }
    }

    [Column("damage")]
    public int Damage
    {
        get => _damage;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Spell damage must be greater than zero.");

            _damage = value;
        }
    }

    [Column("is_learned")]
    public bool IsLearned { get; set; }

    private Spell() { _name = null!; }

    public Spell(string name, int damage)
    {
        Name = name;
        Damage = damage;
        IsLearned = false;
    }

    public override string ToString()
    {
        string status = IsLearned ? "learned" : "not learned";
        return $"#{Id} {Name} — damage {Damage} ({status}).";
    }
}
