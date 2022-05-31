using FluentAssertions;
using FluentAssertions.Extensions;

using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using NSubstitute;

using TimeEntryFunction.Infrastructure;

using Xunit;

namespace TimeEntryFunction.Tests;

public class DynamicsFacadeTest
{
    private const string StartAttributeName = "msdyn_start";

    private const string DurationAttributeName = "msdyn_duration";

    private readonly IOrganizationServiceAsync2 service;

    private readonly DynamicsFacade cut;

    public DynamicsFacadeTest()
    {
        this.service = Substitute.For<IOrganizationServiceAsync2>();
        this.cut = new DynamicsFacade(this.service);
    }

    [Fact]
    public async Task CreateFullDayEntryAsync()
    {
        // Arrange
        Entity? entity = null;
        var id = Guid.NewGuid();
        var startDate = 10.May(2022).AsLocal();
        const int minutesInDay = 60 * 24;

        this.service.CreateAsync(Arg.Do<Entity>(x => entity = x), CancellationToken.None).Returns(id);

        // Act
        await this.cut.CreateFullDayEntryAsync(startDate, CancellationToken.None);

        // Assert
        entity.Should().NotBeNull();
        var start = entity!.GetAttributeValue<DateTime>(StartAttributeName);
        start.Should().Be(startDate);
        start.Should().BeIn(DateTimeKind.Utc);
        entity!.GetAttributeValue<int>(DurationAttributeName).Should().Be(minutesInDay);
    }

    [Fact]
    public async Task GetEntriesAsync()
    {
        // Arrange
        var startDate = 10.May(2022);
        var endDate = 11.May(2022);
        var entity = new Entity();
        entity.Attributes.Add(StartAttributeName, startDate);

        this.service.RetrieveMultipleAsync(Arg.Any<QueryBase>(), CancellationToken.None)
            .Returns(new EntityCollection(new List<Entity> { entity }));

        // Act
        var result = await this.cut.GetEntriesAsync(startDate, endDate, CancellationToken.None);

        // Assert
        entity.Should().NotBeNull();
        result.Should().ContainSingle(x => x.StartDate == startDate);
    }
}
