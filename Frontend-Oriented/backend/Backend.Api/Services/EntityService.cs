using Backend.Api.Services.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Models;
using Backend.Infrastructure.Utilities;

namespace Backend.Api.Services;

public class EntityService : IEntityService
{
    private static readonly HashSet<string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "name", "type", "taskForce", "latitude", "longitude", "updatedAt"
    };
    private readonly IEntityRepository _entityRepository;
    private readonly IScenarioRepository _scenarioRepository;

    public EntityService(
        IEntityRepository entityRepository,
        IScenarioRepository scenarioRepository)
    {
        _entityRepository = entityRepository;
        _scenarioRepository = scenarioRepository;
    }

    public async Task<PagedResult<Entity>?> GetByScenarioIdAsync(
        Guid scenarioId,
        int page,
        int pageSize,
        string? search,
        EntityType? type,
        TaskForce? taskForce,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken)
    {
        if (await _scenarioRepository.GetByIdAsync(scenarioId, cancellationToken) is null)
            return null;

        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            filters.Add(GridifyFilterBuilder.Contains("name", search.Trim()));
        if (type.HasValue)
            filters.Add(GridifyFilterBuilder.Equals("type", type.Value));
        if (taskForce.HasValue)
            filters.Add(GridifyFilterBuilder.Equals("taskForce", taskForce.Value));

        var normalizedSortBy = SortFields.Contains(sortBy) ? sortBy : "name";
        var direction = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        return await _entityRepository.GetByScenarioIdAsync(scenarioId, new GridRequest
        {
            Page = page,
            PageSize = pageSize,
            Filter = filters.Count == 0 ? null : GridifyFilterBuilder.And(filters),
            OrderBy = $"{normalizedSortBy} {direction}"
        }, cancellationToken);
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
