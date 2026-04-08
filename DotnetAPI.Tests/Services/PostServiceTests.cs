using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotnetAPI.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IDataContextDapper> _mockDapper;
    private readonly PostService _sut;

    public PostServiceTests()
    {
        _mockDapper = new Mock<IDataContextDapper>();
        var mockLogger = new Mock<ILogger<PostService>>();
        _sut = new PostService(_mockDapper.Object, mockLogger.Object);
    }

    [Fact]
    public void GetPosts_WithPostId_IncludesPostIdParam()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<Post>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<Post>());

        _sut.GetPosts(5, 0, "none");

        _mockDapper.Verify(d => d.LoadDataWithParameters<Post>(
            It.Is<string>(sql => sql.Contains("@PostId")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetPosts_WithSearchParam_IncludesSearchValueParam()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<Post>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<Post>());

        _sut.GetPosts(0, 0, "test query");

        _mockDapper.Verify(d => d.LoadDataWithParameters<Post>(
            It.Is<string>(sql => sql.Contains("@SearchValue")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetPosts_WithNoneSearch_ExcludesSearchValueParam()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<Post>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<Post>());

        _sut.GetPosts(0, 0, "none");

        _mockDapper.Verify(d => d.LoadDataWithParameters<Post>(
            It.Is<string>(sql => !sql.Contains("@SearchValue")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void GetMyPosts_IncludesUserIdParam()
    {
        _mockDapper
            .Setup(d => d.LoadDataWithParameters<Post>(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(new List<Post>());

        _sut.GetMyPosts(42);

        _mockDapper.Verify(d => d.LoadDataWithParameters<Post>(
            It.Is<string>(sql => sql.Contains("@UserId")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void UpsertPost_WithExistingPostId_IncludesPostIdParam()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(true);

        var post = new Post { PostId = 5, PostTitle = "Test", PostContent = "Content" };
        _sut.UpsertPost(post, 1);

        _mockDapper.Verify(d => d.ExecuteSqlWithParameter(
            It.Is<string>(sql => sql.Contains("@PostId")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void UpsertPost_NewPost_ExcludesPostIdParam()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(true);

        var post = new Post { PostId = 0, PostTitle = "Test", PostContent = "Content" };
        _sut.UpsertPost(post, 1);

        _mockDapper.Verify(d => d.ExecuteSqlWithParameter(
            It.Is<string>(sql => !sql.Contains("@PostId")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public void DeletePost_ReturnsTrue_WhenDapperSucceeds()
    {
        _mockDapper
            .Setup(d => d.ExecuteSqlWithParameter(It.IsAny<string>(), It.IsAny<DynamicParameters>()))
            .Returns(true);

        var result = _sut.DeletePost(1, 1);

        result.Should().BeTrue();
        _mockDapper.Verify(d => d.ExecuteSqlWithParameter(
            It.Is<string>(sql => sql.Contains("spPost_Delete")),
            It.IsAny<DynamicParameters>()), Times.Once);
    }
}
