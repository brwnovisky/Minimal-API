using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entity;

namespace MinimalApi.Infrastructure.Db;

public class Context(IConfiguration configAppSettings) : DbContext
{
    public DbSet<Administrator> Administrators { get; init; } = default!;
    public DbSet<Vehicle> Vehicles { get; init; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        var connectionString = configAppSettings.GetConnectionString
        (
            "MySql"
        );

        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseMySql
            (
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator {
                Id = 1,
                Email = "administrador@teste.com",
                Password = "123456",
                Profile = "Adm"
            }
        );
    }
}