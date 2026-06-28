using DungeonQuest.Models;
using Microsoft.EntityFrameworkCore;

namespace DungeonQuest.db;

public class DungeonContext : DbContext
{
    public DbSet<Weapon> Weapons { get; set; }
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<Potion> Potions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string conn = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                      ?? "server=localhost;port=3306;database=dungeonquest;user=root;password=root;";
        options.UseMySql(conn, ServerVersion.Parse("8.0.46-mysql"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hero>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Id).ValueGeneratedOnAdd();

            entity.Property(h => h.Hp).HasField("_hp")
                  .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            entity.Property(h => h.Xp).HasField("_xp")
                  .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
            entity.Property(h => h.Gold).HasField("_gold")
                  .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

            entity.HasMany(h => h.Weapons)
                  .WithOne(w => w.Hero)
                  .HasForeignKey(w => w.HeroId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(h => h.EquippedWeapon)
                  .WithOne()
                  .HasForeignKey<Weapon>(w => w.EquippedByHeroId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Weapon>(entity =>
        {
            entity.HasKey(w => w.Code);
            entity.Property(w => w.Rarity)
                  .HasDefaultValue(WeaponRarity.Common);
        });

        modelBuilder.Entity<Potion>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).IsRequired();
            entity.HasOne(p => p.Hero)
                  .WithMany()
                  .HasForeignKey(p => p.HeroId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}