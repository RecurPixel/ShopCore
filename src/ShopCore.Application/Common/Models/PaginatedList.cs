namespace ShopCore.Application.Common.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}
