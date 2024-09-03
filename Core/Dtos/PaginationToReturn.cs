namespace Core.Dtos;
public record PaginationToReturn<T>(int PageIndex, int PageSize, int Count, IReadOnlyList<T> Data)
{
    public int PageIndex { get; set; } = PageIndex;
    public int PageSize { get; set; } = PageSize;
    public int Count { get; set; } = Count;
    public IReadOnlyList<T> Data { get; set; } = Data;
}