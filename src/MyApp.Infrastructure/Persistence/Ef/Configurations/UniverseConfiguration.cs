using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Persistence.Ef.Configurations;

public sealed class UniverseConfiguration : IEntityTypeConfiguration<Universe>
{
    public void Configure(EntityTypeBuilder<Universe> builder)
    {
        builder.ToTable("Universes");

        builder.HasKey(universe => universe.Id);

        builder.Property(universe => universe.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(universe => universe.ApiUrl)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(universe => universe.IsEnabled);
    }
}
