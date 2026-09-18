using Backend.Api.DTOs;
using Backend.Api.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("api/scenarios")]
public class ScenariosController : ControllerBase
{
    private readonly IScenarioService _scenarioService;
    private readonly IMapper _mapper;
    private readonly ILogger<ScenariosController> _logger;

    public ScenariosController(IScenarioService scenarioService, IMapper mapper, ILogger<ScenariosController> logger)
    {
        _scenarioService = scenarioService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ScenarioListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ScenarioListItemDto>>> GetScenarios(CancellationToken cancellationToken)
    {
        try
        {
            var scenarios = await _scenarioService.GetAllAsync(cancellationToken);
            var response = scenarios.Select(summary =>
            {
                var dto = _mapper.Map<ScenarioListItemDto>(summary.Scenario);
                dto.EntityCount = summary.EntityCount;
                return dto;
            }).ToList();

            return Ok(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to retrieve scenarios.";
            _logger.LogError(exception, "{ErrorMessage}", errorMessage);
            return InternalServerError(errorMessage);
        }
    }

    [HttpGet("{scenarioId:guid}")]
    [ProducesResponseType(typeof(ScenarioDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ScenarioDetailsDto>> GetScenarioById(Guid scenarioId, CancellationToken cancellationToken)
    {
        try
        {
            var scenario = await _scenarioService.GetByIdAsync(scenarioId, cancellationToken);
            if (scenario is null)
                return NotFound(new ErrorResponse { Message = $"Scenario with ID '{scenarioId}' was not found." });

            return Ok(_mapper.Map<ScenarioDetailsDto>(scenario));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to retrieve scenario.";
            _logger.LogError(exception, "{ErrorMessage} Scenario ID: {ScenarioId}", errorMessage, scenarioId);
            return InternalServerError(errorMessage);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScenarioDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ScenarioDetailsDto>> CreateScenario(
        [FromBody] CreateScenarioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var scenario = await _scenarioService.CreateAsync(request.Name, request.Description, cancellationToken);
            var response = _mapper.Map<ScenarioDetailsDto>(scenario);

            _logger.LogInformation(
                "Scenario {ScenarioId} was created with name {ScenarioName}",
                scenario.Id,
                scenario.Name);

            return CreatedAtAction(nameof(GetScenarioById), new { scenarioId = response.Id }, response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            const string errorMessage = "Failed to create scenario.";
            _logger.LogError(exception, "{ErrorMessage}", errorMessage);
            return InternalServerError(errorMessage);
        }
    }

    private ObjectResult InternalServerError(string message)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = message });
    }
}
