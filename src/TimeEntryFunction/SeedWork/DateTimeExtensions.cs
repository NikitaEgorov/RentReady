namespace TimeEntryFunction.SeedWork;

public static class DateTimeExtensions
{
    public static List<DateTime> GenerateDaysUntil(this DateTime start, DateTime end) =>
        Enumerable.Range(0, (int)Math.Ceiling((end - start).TotalDays))
                  .Select(i => start.Date.AddDays(i))
                  .ToList();
}
