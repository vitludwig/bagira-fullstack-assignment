using Backend.Api.DTOs;
using Backend.Domain.Models;
using Mapster;

namespace Backend.Api.Mappings;

public class EntityMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Entity, EntityDto>();
    }
}
