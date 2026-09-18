using Backend.Api.DTOs;
using Backend.Api.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("api/entities")]
public class EntitiesController : ControllerBase
{
    private readonly IEntityService _entityService;
    private readonly IMapper _mapper;
    private readonly ILogger<EntitiesController> _logger;

    public EntitiesController(IEntityService entityService, IMapper mapper, ILogger<EntitiesController> logger)
    {
        _entityService = entityService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("~/api/scenarios/{scenarioId:guid}/entities")]
    [ProducesResponseType(typeof(IEnumerable<EntityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EntityDto>>> GetEntitiesByScenario(
        Guid scenarioId,
        CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _entityService.GetByScenarioIdAsync(scenarioId, cancellationToken);
            if (entities is null)
                return NotFound(new ErrorResponse { Message = $"Scenario with ID '{scenarioId}' was not found." });

            return Ok(_mapper.Map<List<EntityDto>>(entities));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to retrieve entities for scenario.";
            _logger.LogError(exception, "{ErrorMessage} Scenario ID: {ScenarioId}", errorMessage, scenarioId);
            return InternalServerError(errorMessage);
        }
    }

    [HttpGet("{entityId:guid}")]
    [ProducesResponseType(typeof(EntityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EntityDto>> GetEntityById(Guid entityId, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _entityService.GetByIdAsync(entityId, cancellationToken);
            if (entity is null)
                return NotFound(new ErrorResponse { Message = $"Entity with ID '{entityId}' was not found." });

            return Ok(_mapper.Map<EntityDto>(entity));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to retrieve entity.";
            _logger.LogError(exception, "{ErrorMessage} Entity ID: {EntityId}", errorMessage, entityId);
            return InternalServerError(errorMessage);
        }
    }

    [HttpPost("~/api/scenarios/{scenarioId:guid}/entities")]
    [ProducesResponseType(typeof(EntityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EntityDto>> CreateEntity(
        Guid scenarioId,
        [FromBody] CreateEntityRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _entityService.CreateAsync(
                scenarioId,
                request.Type!.Value,
                request.TaskForce!.Value,
                request.Name,
                request.Latitude!.Value,
                request.Longitude!.Value,
                cancellationToken);
            if (entity is null)
                return NotFound(new ErrorResponse { Message = $"Scenario with ID '{scenarioId}' was not found." });

            var response = _mapper.Map<EntityDto>(entity);

            _logger.LogInformation(
                "Entity {EntityId} was created for scenario {ScenarioId}",
                entity.Id,
                entity.ScenarioId);

            return CreatedAtAction(nameof(GetEntityById), new { entityId = response.Id }, response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to create entity for scenario.";
            _logger.LogError(exception, "{ErrorMessage} Scenario ID: {ScenarioId}", errorMessage, scenarioId);
            return InternalServerError(errorMessage);
        }
    }

    private ObjectResult InternalServerError(string message)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = message });
    }
}
