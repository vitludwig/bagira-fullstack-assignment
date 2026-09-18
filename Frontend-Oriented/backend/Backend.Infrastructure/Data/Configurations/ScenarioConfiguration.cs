using Backend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Data.Configurations;

public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("Scenarios");
        builder.HasKey(scenario => scenario.Id);
        builder.Property(scenario => scenario.Name).HasMaxLength(200).IsRequired();
        builder.Property(scenario => scenario.Description).HasMaxLength(1000);
        builder.Property(scenario => scenario.CreatedAt).IsRequired();
        builder.Property(scenario => scenario.UpdatedAt).IsRequired();
        builder.HasIndex(scenario => scenario.UpdatedAt);
    }
}
