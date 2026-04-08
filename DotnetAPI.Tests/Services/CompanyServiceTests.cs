using System.Dynamic;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotnetAPI.Tests.Services;

public class CompanyServiceTests
{
    private readonly Mock<IDataContextDapper> _mockDapper;
    private readonly CompanyService _sut;

    public CompanyServiceTests()
    {
        _mockDapper = new Mock<IDataContextDapper>();
        var mockLogger = new Mock<ILogger<CompanyService>>();
        _sut = new CompanyService(_mockDapper.Object, mockLogger.Object);
    }

    [Fact]
    public void GetCompanyInfo_CallsLoadDataSingleWithParameters()
    {
        var expected = new CompanyInfo();
        _mockDapper
            .Setup(d => d.LoadDataSingleWithParameters<CompanyInfo>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(expected);

        var result = _sut.GetCompanyInfo();

        result.Should().BeSameAs(expected);
        _mockDapper.Verify(d => d.LoadDataSingleWithParameters<CompanyInfo>(
            It.Is<string>(sql => sql.Contains("spGet_CompanyInfo")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetMetrics_PassesYearAndStatusParams()
    {
        dynamic metricsRaw = new ExpandoObject();
        metricsRaw.TotalEmployees = 50;
        metricsRaw.JoinedOrLeftYearly = 10;
        metricsRaw.MonthlyBreakdown = "Jan:5,Feb:3";

        _mockDapper
            .Setup(d => d.LoadDataSingleWithParameters<dynamic>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns((object)metricsRaw);

        var result = _sut.GetMetrics(2026, true);

        result.TotalEmployees.Should().Be(50);
        _mockDapper.Verify(d => d.LoadDataSingleWithParameters<dynamic>(
            It.Is<string>(sql => sql.Contains("@Year") && sql.Contains("@Status")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetBudget_PassesYearParam()
    {
        var expected = new List<Budget> { new Budget() };
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<Budget>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(expected);

        var result = _sut.GetBudget(2026);

        result.Should().HaveCount(1);
        _mockDapper.Verify(d => d.LoadDataWithParameters<Budget>(
            It.Is<string>(sql => sql.Contains("spGet_Budget") && sql.Contains("@Year")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }
}
