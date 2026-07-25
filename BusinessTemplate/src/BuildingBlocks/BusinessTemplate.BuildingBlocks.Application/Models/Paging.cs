namespace BusinessTemplate.BuildingBlocks.Application.Models;

public abstract record PagedRequest(int Page = 1, int PageSize = 20)
{
    public int NormalizedPage => Page < 1 ? 1 : Page;
    public int NormalizedPageSize => Math.Clamp(PageSize, 1, 100);
}

public sealed record PagedResult<T>(int Page, int PageSize, int TotalCount, IReadOnlyCollection<T> Items);

public sealed record SortModel(string Field, bool Descending);
