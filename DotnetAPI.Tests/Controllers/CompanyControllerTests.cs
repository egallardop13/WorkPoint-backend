using DotnetAPI.Controllers;
using DotnetAPI.Models;
using DotnetAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DotnetAPI.Tests.Controllers;

public class CompanyControllerTests
{
    private readonly Mock<ICompanyService> _mockService;
    private readonly CompanyController _sut;

    public CompanyControllerTests()
    {
        _mockService = new Mock<ICompanyService>();
        _sut = new CompanyController(_mockService.Object);
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(2200)]
    public void GetMetrics_InvalidYear_ReturnsBadRequest(int year)
    {
        var result = _sut.GetMetrics(year, true);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GetMetrics_ValidYear_ReturnsOk()
    {
        var metrics = new MetricsInfo
        {
            TotalEmployees = 50,
            JoinedOrLeftYearly = 10,
            MonthlyBreakdown = new List<MonthData>()
        };
        _mockService.Setup(s => s.GetMetrics(2026, true)).Returns(metrics);

        var result = _sut.GetMetrics(2026, true);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(metrics);
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(2200)]
    public void GetBudget_InvalidYear_ReturnsBadRequest(int year)
    {
        var result = _sut.GetBudget(year);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GetBudget_ValidYear_ReturnsOk()
    {
        var budgets = new List<Budget> { new Budget() };
        _mockService.Setup(s => s.GetBudget(2026)).Returns(budgets);

        var result = _sut.GetBudget(2026);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeSameAs(budgets);
    }
}
