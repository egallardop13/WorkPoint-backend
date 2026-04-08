using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotnetAPI.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IDataContextDapper> _mockDapper;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _mockDapper = new Mock<IDataContextDapper>();
        var mockLogger = new Mock<ILogger<UserService>>();
        _sut = new UserService(_mockDapper.Object, mockLogger.Object);
    }

    [Fact]
    public void GetUsers_WithUserId_IncludesUserIdParameter()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<UserComplete>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<UserComplete>());

        _sut.GetUsers(42, false);

        _mockDapper.Verify(d => d.LoadDataWithParameters<UserComplete>(
            It.Is<string>(sql => sql.Contains("@UserId")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetUsers_WithoutUserId_ExcludesUserIdParameter()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<UserComplete>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<UserComplete>());

        _sut.GetUsers(0, false);

        _mockDapper.Verify(d => d.LoadDataWithParameters<UserComplete>(
            It.Is<string>(sql => !sql.Contains("@UserId")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetUsers_WithActiveFilter_IncludesActiveParameter()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<UserComplete>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<UserComplete>());

        _sut.GetUsers(0, true);

        _mockDapper.Verify(d => d.LoadDataWithParameters<UserComplete>(
            It.Is<string>(sql => sql.Contains("@Active")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetUsersWithPagination_DefaultsPageTo1_WhenZero()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<dynamic>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<dynamic>());

        _sut.GetUsersWithPagination(0, 10, null, null);

        _mockDapper.Verify(d => d.LoadDataWithParameters<dynamic>(
            It.Is<string>(sql => sql.Contains("@Page") && sql.Contains("@Limit")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetUsersWithPagination_IncludesQueryParam_WhenProvided()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<dynamic>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<dynamic>());

        _sut.GetUsersWithPagination(1, 10, "John", null);

        _mockDapper.Verify(d => d.LoadDataWithParameters<dynamic>(
            It.Is<string>(sql => sql.Contains("@Query")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void UpsertUser_CallsLoadDataSingleWithParameters()
    {
        var user = new UserComplete
        {
            UserId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Gender = "Male",
            Active = true,
            JobTitle = "Dev",
            Department = "Engineering",
            Salary = 80000,
            DateHired = DateTime.UtcNow
        };

        _mockDapper
            .Setup(d => d.LoadDataSingleWithParameters<int>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(1);

        var result = _sut.UpsertUser(user);

        result.Should().Be(1);
        _mockDapper.Verify(d => d.LoadDataSingleWithParameters<int>(
            It.Is<string>(sql => sql.Contains("spUser_Upsert")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void DeleteUser_ReturnsTrue_WhenDapperSucceeds()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(true);

        var result = _sut.DeleteUser(1);

        result.Should().BeTrue();
        _mockDapper.Verify(d => d.ExecuteSqlWithParameter(
            It.Is<string>(sql => sql.Contains("spUser_Delete")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void DeleteUser_ReturnsFalse_WhenDapperFails()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(false);

        var result = _sut.DeleteUser(999);

        result.Should().BeFalse();
    }
}
