using Backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Data.Configurations;

public class EntityConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("Entities");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.TaskForce).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.CreatedAt).IsRequired();
        builder.Property(entity => entity.UpdatedAt).IsRequired();
        builder.HasOne<Scenario>()
            .WithMany()
            .HasForeignKey(entity => entity.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => new { entity.ScenarioId, entity.Name });
        builder.HasIndex(entity => new { entity.ScenarioId, entity.Type });
        builder.HasIndex(entity => new { entity.ScenarioId, entity.TaskForce });
    }
}
