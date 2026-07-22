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

        builder.Ignore(player => player.Weight);
        builder.Ignore(player => player.Height);

        builder.HasMany(player => player.Stats)
            .WithOne()
            .HasForeignKey("PlayerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Navigation(player => player.Stats)
            .AutoInclude();

        builder.Property(player => player.ExternalResourceId);
        builder.Property(player => player.Type)
            .HasConversion<int>();
    }
}
