using System.ComponentModel.DataAnnotations;
using Backend.Domain.Models;

namespace Backend.Api.DTOs;

public class EntityListQuery
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }
    public EntityType? Type { get; init; }
    public TaskForce? TaskForce { get; init; }
    public string SortBy { get; init; } = "name";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "asc";
}
