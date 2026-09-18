namespace Backend.Infrastructure.Models;

public class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required int TotalCount { get; init; }
}
