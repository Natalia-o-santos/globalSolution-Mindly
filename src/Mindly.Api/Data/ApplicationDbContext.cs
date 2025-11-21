using Microsoft.EntityFrameworkCore;
using Mindly.Api.Domain.Entities;
using Mindly.Api.Domain.Enums;

namespace Mindly.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FocusSession> FocusSessions => Set<FocusSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FocusSession>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Description)
                .HasMaxLength(400);

            entity.Property(e => e.FocusMinutes)
                .IsRequired();

            entity.Property(e => e.BreakMinutes)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(FocusSessionStatus.Planned)
                .IsRequired();

            entity.Property(e => e.IoTIntegrationEnabled)
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });
    }
}
