using Backend.Api.Services.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;

namespace Backend.Api.Services;

public class EntityService : IEntityService
{
    private readonly IEntityRepository _entityRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public EntityService(
        IEntityRepository entityRepository,
        IScenarioRepository scenarioRepository)
    {
        _entityRepository = entityRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<IReadOnlyCollection<Entity>?> GetByScenarioIdAsync(
        Guid scenarioId,
        CancellationToken cancellationToken)
    {
        var scenario = await _scenarioRepository.GetByIdAsync(scenarioId, cancellationToken);
        if (scenario is null)
            return null;

        return await _entityRepository.GetByScenarioIdAsync(scenarioId, cancellationToken);
    }

    public Task<Entity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken)
    {
        return _entityRepository.GetByIdAsync(entityId, cancellationToken);
    }

    public Task<Entity?> CreateAsync(
        Guid scenarioId,
        EntityType type,
        TaskForce taskForce,
        string name,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var entity = new Entity
        {
            ScenarioId = scenarioId,
            Type = type,
            TaskForce = taskForce,
            Name = name.Trim(),
            Latitude = latitude,
            Longitude = longitude
        };

        return _entityRepository.AddToScenarioAsync(entity, cancellationToken);
    }
}
