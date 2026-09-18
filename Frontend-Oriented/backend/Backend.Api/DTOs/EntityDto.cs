using Backend.Domain.Models;

namespace Backend.Api.DTOs;

/// <summary>
/// DTO for entity
/// </summary>
public class EntityDto
{
    public Guid Id { get; set; }
    public Guid ScenarioId { get; set; }
    public EntityType Type { get; set; }
    public TaskForce TaskForce { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
