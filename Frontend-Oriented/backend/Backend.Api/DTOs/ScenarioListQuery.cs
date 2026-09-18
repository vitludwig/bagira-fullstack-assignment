using System.ComponentModel.DataAnnotations;

namespace Backend.Api.DTOs;

public class ScenarioListQuery
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }
    public string SortBy { get; init; } = "updatedAt";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "desc";
}
