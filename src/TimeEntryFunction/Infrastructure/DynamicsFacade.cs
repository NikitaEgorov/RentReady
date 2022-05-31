using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using TimeEntryFunction.Application;

namespace TimeEntryFunction.Infrastructure;

public class DynamicsFacade : IDynamicsFacade
{
    private readonly IOrganizationServiceAsync2 service;

    public DynamicsFacade(IOrganizationServiceAsync2 service) => this.service = service;

    public async Task<Guid> CreateFullDayEntryAsync(DateTime startDate, CancellationToken cancellationToken)
    {
        const int minutesInDay = 60 * 24;
        return await this.service.CreateAsync(new MsTimeEntry { StartDate = startDate, Duration = minutesInDay }, cancellationToken);
    }

    public async Task<IEnumerable<ITimeEntry>> GetEntriesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var filter = new FilterExpression();
        filter.AddCondition(TimeEntryWrapper.StartAttributeName, ConditionOperator.GreaterEqual, startDate.ToUniversalTime());
        filter.AddCondition(TimeEntryWrapper.StartAttributeName, ConditionOperator.LessEqual, endDate.ToUniversalTime());

        var query = new QueryExpression(MsTimeEntry.EntityLogicalName);
        query.ColumnSet
             .AddColumns(
                 TimeEntryWrapper.StartAttributeName,
                 TimeEntryWrapper.EndAttributeName,
                 TimeEntryWrapper.DurationAttributeName);

        query.Criteria.AddFilter(filter);

        var r = await this.service.RetrieveMultipleAsync(query, cancellationToken);
        return r.Entities.Select(x => new TimeEntryWrapper(x)).ToList();
    }

    public interface ITimeEntry
    {
        public DateTime? StartDate { get; }
    }

    private class TimeEntryWrapper : ITimeEntry
    {
        public const string StartAttributeName = "msdyn_start";

        public const string EndAttributeName = "msdyn_end";

        public const string DurationAttributeName = "msdyn_duration";

        private readonly Entity entity;

        public TimeEntryWrapper(Entity entity) => this.entity = entity;

        public DateTime? StartDate
        {
            get
            {
                var attributeValue = this.entity.GetAttributeValue<DateTime?>(StartAttributeName);
                return attributeValue;
            }
        }
    }

    private class MsTimeEntry : Entity
    {
        public const string EntityLogicalName = "msdyn_timeentry";

        public MsTimeEntry()
            : base(EntityLogicalName)
        {
        }

        public DateTime? StartDate
        {
            init =>
                this.SetAttributeValue(
                    TimeEntryWrapper.StartAttributeName,
                    value != null ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null); // TODO timezone?
        }

        public int? Duration { init => this.SetAttributeValue(TimeEntryWrapper.DurationAttributeName, value); }
    }
}
