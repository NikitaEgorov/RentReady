using System.Globalization;

using FluentAssertions;
using FluentAssertions.Extensions;

using TimeEntryFunction.Models;

using Xunit;

namespace TimeEntryFunction.Tests;

public class EntityRequestTest
{
    [Fact]
    public void Validate_OK()
    {
        // Arrange
        var req = new EntityRequest { StartOn = 01.May(2022), EndOn = 01.May(2022) };

        // Act
        req.Validate();

        // Assert
        //OK
    }

    [Theory]
    [InlineData("2022-05-01 14:00:00", "2022-05-01")]
    [InlineData("2022-05-01", "2022-05-01 10:00:00")]
    [InlineData("2022-05-01 03:00:01", "2022-05-01 10:00:00")]
    public void Validate_StartDateHasTimePart(string startDateIn, string endDateIn)
    {
        // Arrange
        var req = new EntityRequest
                  {
                      StartOn = DateTime.Parse(startDateIn, CultureInfo.InvariantCulture),
                      EndOn = DateTime.Parse(endDateIn, CultureInfo.InvariantCulture)
                  };

        // Act
        var result = () => req.Validate();

        // Assert
        result.Should().Throw<ArgumentException>();
    }
}
