using System;

namespace CoffeeShopApp.Utils;

public static class TimeZoneHelper
{
    // Return a TimeZoneInfo for Cambodia (Asia/Phnom_Penh) with a Windows fallback.
    public static TimeZoneInfo GetCambodiaTimeZone()
    {
        // Try IANA first (Linux/macOS), then Windows id (Windows).
        string[] ids = new[] { "Asia/Phnom_Penh", "SE Asia Standard Time" };
        foreach (var id in ids)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch { }
        }
        // Fallback to UTC to avoid exceptions
        return TimeZoneInfo.Utc;
    }

    public static DateTime ToCambodiaTime(DateTime dt)
    {
        var tz = GetCambodiaTimeZone();
        // Assume stored times are UTC or unspecified (treat as UTC)
        DateTime utc = dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        try
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
        }
        catch
        {
            return utc;
        }
    }
}
