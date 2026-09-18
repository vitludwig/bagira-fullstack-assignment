namespace Backend.Api.DTOs;

/// <summary>
/// DTO for scenario details
/// </summary>
public class ScenarioDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
