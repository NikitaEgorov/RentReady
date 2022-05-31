using FluentAssertions;
using FluentAssertions.Extensions;

using TimeEntryFunction.SeedWork;

using Xunit;

namespace TimeEntryFunction.Tests;

public class DateTimeExtensionsTest
{
    [Fact]
    public void GenerateDaysUntil_StartLessThanEnd()
    {
        // Arrange
        var startDate = 02.May(2022);
        var endDate = 01.May(2022);

        // Act
        var result = () => startDate.GenerateDaysUntil(endDate);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GenerateDaysUntil_StartEqEnd()
    {
        // Arrange
        var startDate = 01.May(2022);
        var endDate = 01.May(2022);

        // Act
        var result = startDate.GenerateDaysUntil(endDate);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GenerateDaysUntil_StartGreatEnd()
    {
        // Arrange
        var startDate = 01.May(2022);
        var endDate = 02.May(2022);

        // Act
        var result = startDate.GenerateDaysUntil(endDate);

        // Assert
        result.Should().Equal(startDate);
    }
}
