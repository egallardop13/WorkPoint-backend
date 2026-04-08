using DotnetAPI.Helpers;
using FluentAssertions;

namespace DotnetAPI.Tests.Helpers;

public class DataParserHelperTests
{
    [Fact]
    public void ParseMonthlyData_ValidInput_ReturnsCorrectList()
    {
        var result = DataParserHelper.ParseMonthlyData("Jan:5,Feb:3,Mar:10");

        result.Should().HaveCount(3);
        result[0].Month.Should().Be("Jan");
        result[0].Count.Should().Be(5);
        result[1].Month.Should().Be("Feb");
        result[1].Count.Should().Be(3);
        result[2].Month.Should().Be("Mar");
        result[2].Count.Should().Be(10);
    }

    [Fact]
    public void ParseMonthlyData_NullInput_ReturnsEmptyList()
    {
        var result = DataParserHelper.ParseMonthlyData(null);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseMonthlyData_EmptyString_ReturnsEmptyList()
    {
        var result = DataParserHelper.ParseMonthlyData("");

        result.Should().BeEmpty();
    }
}
