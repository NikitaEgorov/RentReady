using TimeEntryFunction.Infrastructure;

namespace TimeEntryFunction.Application;

public interface IDynamicsFacade
{
    Task<Guid> CreateFullDayEntryAsync(DateTime startDate, CancellationToken cancellationToken);

    Task<IEnumerable<DynamicsFacade.ITimeEntry>> GetEntriesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);
}
