using Backend.Api.DTOs;
using Backend.Domain.Models;
using Mapster;

namespace Backend.Api.Mappings;

public class ScenarioMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Scenario, ScenarioDetailsDto>();
        config.NewConfig<Scenario, ScenarioListItemDto>()
            .Ignore(destination => destination.EntityCount);
    }
}
