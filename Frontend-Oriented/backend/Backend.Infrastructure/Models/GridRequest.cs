namespace Backend.Infrastructure.Models;

public class GridRequest
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? Filter { get; init; }
    public string? OrderBy { get; init; }
}
