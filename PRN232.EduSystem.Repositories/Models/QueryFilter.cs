namespace PRN232.EduSystem.Repositories.Models;

public class QueryFilter
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<string> Expand { get; set; } = new();
}
