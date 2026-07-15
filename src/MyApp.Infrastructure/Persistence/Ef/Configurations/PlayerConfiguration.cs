using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Persistence.Ef.Configurations;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(player => player.Id);

        builder.Property(player => player.Id)
            .ValueGeneratedOnAdd();

        builder.Property(player => player.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(player => player.Weight);
        builder.Property(player => player.Height);
        builder.Property(player => player.ExternalResourceId);
        builder.Property(player => player.Type)
            .HasConversion<int>();
    }
}
