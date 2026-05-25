namespace PRN232.EduSystem.API.Models.Responses;

public class PagedResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<T>? Data { get; set; }
    public object? Errors { get; set; }
    public PaginationMetadata Pagination { get; set; } = new();

    public static PagedResponse<T> Ok(IEnumerable<T> data, int page, int pageSize, int totalItems) =>
        new()
        {
            Success = true,
            Message = "Request processed successfully",
            Data    = data,
            Pagination = new PaginationMetadata
            {
                Page       = page,
                PageSize   = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            }
        };
}
