using FluentAssertions;
using FluentAssertions.Extensions;

using NSubstitute;

using TimeEntryFunction.Application;
using TimeEntryFunction.Infrastructure;

using Xunit;

namespace TimeEntryFunction.Tests;

public class TimeEntriesMakerTest
{
    private readonly TimeEntriesMaker cut;

    private readonly IDynamicsFacade dynamicsFacade;

    public TimeEntriesMakerTest()
    {
        this.dynamicsFacade = Substitute.For<IDynamicsFacade>();
        this.cut = new TimeEntriesMaker(this.dynamicsFacade);
    }

    [Fact]
    public async Task EmptyPeriod()
    {
        // Arrange
        var startDate = 01.May(2022);
        var endDate = 01.May(2022);

        // Act
        var r = await this.cut.MakeAsync(startDate, endDate, CancellationToken.None);

        // Assert
        r.Should().BeEmpty();
    }

    [Fact]
    public async Task NegativePeriod()
    {
        // Arrange
        var startDate = 02.May(2022);
        var endDate = 01.May(2022);

        // Act
        var r = async () => await this.cut.MakeAsync(startDate, endDate, CancellationToken.None);

        // Assert
        await r.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task EndDateNotIncluded()
    {
        // Arrange
        var id = Guid.NewGuid();
        var startDate = 01.May(2022);
        var endDate = 02.May(2022);

        this.dynamicsFacade.CreateFullDayEntryAsync(startDate, CancellationToken.None).Returns(id);

        // Act
        var r = await this.cut.MakeAsync(startDate, endDate, CancellationToken.None);

        // Assert
        r.Should().Equal(id);
    }

    [Fact]
    public async Task SkipExistedDates()
    {
        // Arrange
        var id = Guid.NewGuid();
        var startDate = 01.May(2022);
        var endDate = 04.May(2022);

        var existsDay1 = 01.May(2022);
        var existsDay2 = 02.May(2022);

        var existsTimeEntry1 = Substitute.For<DynamicsFacade.ITimeEntry>();
        existsTimeEntry1.StartDate.Returns(existsDay1);
        var existsTimeEntry2 = Substitute.For<DynamicsFacade.ITimeEntry>();
        existsTimeEntry2.StartDate.Returns(existsDay2);

        this.dynamicsFacade.CreateFullDayEntryAsync(Arg.Any<DateTime>(), CancellationToken.None).Returns(id);
        this.dynamicsFacade
            .GetEntriesAsync(startDate, endDate, CancellationToken.None)
            .Returns(new[] { existsTimeEntry1, existsTimeEntry2 });

        // Act
        var r = await this.cut.MakeAsync(startDate, endDate, CancellationToken.None);

        // Assert
        Received.InOrder(
            async () =>
                await this.dynamicsFacade.Received().CreateFullDayEntryAsync(03.May(2022), CancellationToken.None));
        r.Should().Equal(id);
    }
}
