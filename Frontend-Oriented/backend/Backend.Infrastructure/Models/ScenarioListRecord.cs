using Backend.Domain.Models;

namespace Backend.Infrastructure.Models;

public class ScenarioListRecord
{
    public required Scenario Scenario { get; init; }
    public required int EntityCount { get; init; }
}
