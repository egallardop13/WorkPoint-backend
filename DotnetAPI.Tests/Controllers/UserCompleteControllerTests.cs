using DotnetAPI.Controllers;
using DotnetAPI.Models;
using DotnetAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DotnetAPI.Tests.Controllers;

public class UserCompleteControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly UserCompleteController _sut;

    public UserCompleteControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _sut = new UserCompleteController(_mockService.Object);
    }

    [Fact]
    public void UpsertUser_Result1_ReturnsOk()
    {
        var user = new UserComplete { FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        _mockService.Setup(s => s.UpsertUser(user)).Returns(1);

        var result = _sut.UpsertUser(user);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void UpsertUser_Result0_Returns409()
    {
        var user = new UserComplete { FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        _mockService.Setup(s => s.UpsertUser(user)).Returns(0);

        var result = _sut.UpsertUser(user);

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(409);
    }

    [Fact]
    public void DeleteUser_WhenTrue_ReturnsOk()
    {
        _mockService.Setup(s => s.DeleteUser(1)).Returns(true);

        var result = _sut.DeleteUser(1);

        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void DeleteUser_WhenFalse_ThrowsException()
    {
        _mockService.Setup(s => s.DeleteUser(999)).Returns(false);

        var act = () => _sut.DeleteUser(999);

        act.Should().Throw<Exception>().WithMessage("Failed to delete user");
    }
}
