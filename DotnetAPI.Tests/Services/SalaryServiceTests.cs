using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotnetAPI.Tests.Services;

public class SalaryServiceTests
{
    private readonly Mock<IDataContextDapper> _mockDapper;
    private readonly SalaryService _sut;

    public SalaryServiceTests()
    {
        _mockDapper = new Mock<IDataContextDapper>();
        var mockLogger = new Mock<ILogger<SalaryService>>();
        _sut = new SalaryService(_mockDapper.Object, mockLogger.Object);
    }

    [Fact]
    public void GetUsersSalary_CallsLoadData()
    {
        var expected = new List<UserSalary> { new UserSalary() };
        _mockDapper.Setup(d => d.LoadData<UserSalary>(It.IsAny<string>())).Returns(expected);

        var result = _sut.GetUsersSalary();

        result.Should().HaveCount(1);
        _mockDapper.Verify(d => d.LoadData<UserSalary>(
            It.Is<string>(sql => sql.Contains("UserSalary"))), Times.Once);
    }

    [Fact]
    public void GetDepartmentsInfo_WithDepartment_IncludesDepartmentParam()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<DepartmentInfo>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<DepartmentInfo>());

        _sut.GetDepartmentsInfo("Engineering", null, null);

        _mockDapper.Verify(d => d.LoadDataWithParameters<DepartmentInfo>(
            It.Is<string>(sql => sql.Contains("@Department")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetDepartmentsInfo_WithQueryAndSort_IncludesBothParams()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<DepartmentInfo>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<DepartmentInfo>());

        _sut.GetDepartmentsInfo(null, "search", "name_asc");

        _mockDapper.Verify(d => d.LoadDataWithParameters<DepartmentInfo>(
            It.Is<string>(sql => sql.Contains("@Query") && sql.Contains("@Sort")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void DeleteUserSalary_CallsExecuteSqlWithParameter()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(true);

        var result = _sut.DeleteUserSalary(1);

        result.Should().BeTrue();
        _mockDapper.Verify(d => d.ExecuteSqlWithParameter(
            It.Is<string>(sql => sql.Contains("UserSalary")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetUsersJobInfo_CallsLoadData()
    {
        var expected = new List<UserJobInfo> { new UserJobInfo() };
        _mockDapper.Setup(d => d.LoadData<UserJobInfo>(It.IsAny<string>())).Returns(expected);

        var result = _sut.GetUsersJobInfo();

        result.Should().HaveCount(1);
        _mockDapper.Verify(d => d.LoadData<UserJobInfo>(
            It.Is<string>(sql => sql.Contains("UserJobInfo"))), Times.Once);
    }

    [Fact]
    public void DeleteUserJobInfo_CallsExecuteSqlWithParameter()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(true);

        var result = _sut.DeleteUserJobInfo(1);

        result.Should().BeTrue();
        _mockDapper.Verify(d => d.ExecuteSqlWithParameter(
            It.Is<string>(sql => sql.Contains("UserJobInfo")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }
}
