using TimeEntryFunction.SeedWork;

namespace TimeEntryFunction.Application;

public class TimeEntriesMaker
{
    private readonly IDynamicsFacade dynamicsFacade;

    public TimeEntriesMaker(IDynamicsFacade dynamicsFacade) => this.dynamicsFacade = dynamicsFacade;

    public async Task<IEnumerable<Guid>> MakeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var existDays =
            (await this.dynamicsFacade.GetEntriesAsync(startDate, endDate, cancellationToken))
            .Select(x => x.StartDate?.Date).ToHashSet();

        var days = startDate.GenerateDaysUntil(endDate);

        var result = new List<Guid>();

        foreach (var day in days.Where(day => !existDays.Contains(day)))
        {
            result.Add(await this.dynamicsFacade.CreateFullDayEntryAsync(day, cancellationToken));
        }

        return result;
    }
}
