using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Persistence.Ef.Configurations;

public sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(team => team.Id);

        builder.Property(team => team.Id)
            .ValueGeneratedOnAdd();

        builder.Property(team => team.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(team => team.AttackersCount);
        builder.Property(team => team.DefendersCount);
        builder.Property(team => team.UniverseId);

        builder.HasOne<Universe>()
            .WithMany()
            .HasForeignKey(team => team.UniverseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(team => team.Players)
            .WithOne(player => player.Team)
            .HasForeignKey("TeamId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
