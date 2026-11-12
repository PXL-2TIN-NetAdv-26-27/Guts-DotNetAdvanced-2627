using Microsoft.EntityFrameworkCore;
using SmurfApp.Domain;

namespace SmurfApp.Infrastructure;

public class SmurfDbContext : DbContext
{
    public DbSet<Smurf> Smurfs { get; set; }

    public SmurfDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Smurf>().Property(s => s.Name).HasMaxLength(50);
        Smurf[] smurfsToSeed = GetCoreSmurfs();
        modelBuilder.Entity<Smurf>().HasData(smurfsToSeed);
    }

    private Smurf[] GetCoreSmurfs()
    {
        return new Smurf[]
        {
            new Smurf
            {
                Id = Guid.Parse("7bd2c98e-8e90-4f50-9f7e-2fd9b5ce36f0"),
                Name = "Papa Smurf",
                Age = 546,
                Category = Category.Leader,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/papa-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("d1a1f4b2-3c6b-4f7e-9d1e-3b2c4a5f6e7a"),
                Name = "Brainy Smurf",
                Age = 101,
                Category = Category.Leader,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/brainy-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("a2b3c4d5-e6f7-4812-9a3b-0c1d2e3f4b5c"),
                Name = "Smurfette",
                Age = 80,
                Category = Category.Emotional,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/smurfette-cartoon-80s.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("f4e3d2c1-b0a9-4876-8f6e-5d4c3b2a1f0e"),
                Name = "Hefty Smurf",
                Age = 100,
                Category = Category.Skilled,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/hefty-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("c3b2a1f0-e9d8-4765-8c7b-6a5d4e3f2b1c"),
                Name = "Clumsy Smurf",
                Age = 100,
                Category = Category.Playful,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/clumsy-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("9f8e7d6c-5b4a-4321-8e9f-0a1b2c3d4e5f"),
                Name = "Vanity Smurf",
                Age = 99,
                Category = Category.Emotional,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/vanity-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("01234567-89ab-4cde-8fab-0123456789ab"),
                Name = "Jokey Smurf",
                Age = 160,
                Category = Category.Playful,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/jokey-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("11223344-5566-4777-8899-aabbccddeeff"),
                Name = "Chef Smurf",
                Age = 100,
                Category = Category.Skilled,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/baker-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("ffeeddcc-bbaa-4111-9988-776655443322"),
                Name = "Poet Smurf",
                Age = 102,
                Category = Category.Skilled,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/poet-smurf-cartoon.jpg"
            },

            new Smurf
            {
                Id = Guid.Parse("123e4567-e89b-42d3-a456-426614174000"),
                Name = "Tracker Smurf",
                Age = 100,
                Category = Category.Skilled,
                ImageUrl = "https://charactersdb.com/wp-content/uploads/tracker-smurf-cartoon.jpg"
            }
        };
    }
}