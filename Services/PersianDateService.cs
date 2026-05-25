using System.Globalization;

namespace InventoryTool.Services;

public static class PersianDateService
{
    public static string ToPersianDate(DateOnly date)
    {
        var pc = new PersianCalendar();
        var dt = date.ToDateTime(TimeOnly.MinValue);

        return $"{pc.GetYear(dt):0000}/{pc.GetMonth(dt):00}/{pc.GetDayOfMonth(dt):00}";
    }

    public static bool TryParsePersianDate(string? value, out DateOnly date)
    {
        date = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var parts = value.Trim()
            .Replace("-", "/")
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var year) ||
            !int.TryParse(parts[1], out var month) ||
            !int.TryParse(parts[2], out var day))
            return false;

        try
        {
            var pc = new PersianCalendar();
            var dt = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            date = DateOnly.FromDateTime(dt);
            return true;
        }
        catch
        {
            return false;
        }
    }
}