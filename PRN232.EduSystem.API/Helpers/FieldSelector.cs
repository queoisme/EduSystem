using System.Reflection;

namespace PRN232.EduSystem.API.Helpers;

public static class FieldSelector
{
    public static object? Apply(object obj, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields)) return obj;

        var requested = fields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var dict      = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var type      = obj.GetType();

        foreach (var field in requested)
        {
            var prop = type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
                dict[prop.Name] = prop.GetValue(obj);
        }

        return dict.Count > 0 ? dict : obj;
    }

    public static IEnumerable<object?> ApplyToList(IEnumerable<object> list, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields)) return list;
        return list.Select(item => Apply(item, fields));
    }
}
