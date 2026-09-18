using Backend.Domain.Models;

namespace Backend.Api.Services.Models;

public class ScenarioSummary
{
    public required Scenario Scenario { get; init; }
    public int EntityCount { get; init; }
}
