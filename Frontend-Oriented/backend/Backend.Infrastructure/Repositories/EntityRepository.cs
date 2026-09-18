using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Models;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class EntityRepository(BackendDbContext dbContext) : IEntityRepository
{
    private static readonly IGridifyMapper<Entity> GridMapper = new GridifyMapper<Entity>()
        .AddMap("id", entity => entity.Id)
        .AddMap("name", entity => entity.Name)
        .AddMap("type", entity => entity.Type)
        .AddMap("taskForce", entity => entity.TaskForce)
        .AddMap("latitude", entity => entity.Latitude)
        .AddMap("longitude", entity => entity.Longitude)
        .AddMap("updatedAt", entity => entity.UpdatedAt);
    public async Task<PagedResult<Entity>> GetByScenarioIdAsync(
        Guid scenarioId,
        GridRequest request,
        CancellationToken cancellationToken)
    {
        var result = await dbContext.Entities
            .AsNoTracking()
            .Where(entity => entity.ScenarioId == scenarioId)
            .GridifyAsync(new GridifyQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Filter = request.Filter,
                OrderBy = $"{request.OrderBy},id asc"
            }, cancellationToken, GridMapper);

        return new PagedResult<Entity>
        {
            Items = result.Data.ToList(),
            TotalCount = result.Count
        };
    }

    public Task<Entity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Entities
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<Entity> AddAsync(Entity entity, CancellationToken cancellationToken)
    {
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        var now = DateTime.UtcNow;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;

        dbContext.Entities.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Entity?> AddToScenarioAsync(
        Entity entity,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Scenarios.AnyAsync(
                scenario => scenario.Id == entity.ScenarioId,
                cancellationToken))
            return null;

        return await AddAsync(entity, cancellationToken);
    }

}
