using PRN232.EduSystem.API.Models.Requests;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.API.Helpers;

public static class QueryParameterParser
{
    public static QueryFilter ToQueryFilter(QueryParameters qp)
    {
        var filter = new QueryFilter
        {
            Search   = qp.Search,
            Page     = qp.Page < 1 ? 1 : qp.Page,
            PageSize = qp.Size < 1 ? 10 : qp.Size > 100 ? 100 : qp.Size,
        };

        if (!string.IsNullOrWhiteSpace(qp.Sort))
        {
            var sort = qp.Sort.Trim();
            if (sort.StartsWith('-'))
            {
                filter.SortBy     = sort[1..];
                filter.Descending = true;
            }
            else
            {
                filter.SortBy     = sort;
                filter.Descending = false;
            }
        }

        if (!string.IsNullOrWhiteSpace(qp.Expand))
        {
            filter.Expand = qp.Expand
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        return filter;
    }
}
