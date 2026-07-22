using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Persistence.Ef.Configurations;

public sealed class PlayerStatConfiguration : IEntityTypeConfiguration<PlayerStat>
{
    public void Configure(EntityTypeBuilder<PlayerStat> builder)
    {
        builder.ToTable("PlayerStats");

        builder.HasKey(stat => stat.Id);

        builder.Property(stat => stat.Id)
            .ValueGeneratedOnAdd();

        builder.Property(stat => stat.Value)
            .IsRequired();

        builder.HasDiscriminator<string>("StatType")
            .HasValue<Weight>("Weight")
            .HasValue<Height>("Height");

        builder.HasIndex("PlayerId", "StatType")
            .IsUnique();
    }
}
