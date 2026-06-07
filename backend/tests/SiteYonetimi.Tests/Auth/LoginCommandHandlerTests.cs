using Moq;
using SiteYonetimi.Auth.Commands;
using SiteYonetimi.Auth.Services;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Tests.Helpers;

namespace SiteYonetimi.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var hash = BCrypt.Net.BCrypt.HashPassword("Test1234");
        db.Users.Add(new User { Email = "user@test.com", PasswordHash = hash, FirstName = "Test", LastName = "User", IsActive = true });
        await db.SaveChangesAsync();

        _tokenServiceMock.Setup(t => t.GenerateLoginToken(It.IsAny<User>())).Returns("access_token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh_token");

        var handler = new LoginCommandHandler(db, _tokenServiceMock.Object);
        var result = await handler.Handle(new LoginCommand("user@test.com", "Test1234"), default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("access_token", result.Data.AccessToken);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var hash = BCrypt.Net.BCrypt.HashPassword("Correct123");
        db.Users.Add(new User { Email = "user@test.com", PasswordHash = hash, IsActive = true });
        await db.SaveChangesAsync();

        var handler = new LoginCommandHandler(db, _tokenServiceMock.Object);
        var result = await handler.Handle(new LoginCommand("user@test.com", "WrongPass"), default);

        Assert.False(result.IsSuccess);
        Assert.Contains("hatalı", result.Error);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var db = InMemoryDbHelper.CreateMasterDb();

        var handler = new LoginCommandHandler(db, _tokenServiceMock.Object);
        var result = await handler.Handle(new LoginCommand("noone@test.com", "anypass"), default);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_InactiveUser_ReturnsFailure()
    {
        var db = InMemoryDbHelper.CreateMasterDb();
        var hash = BCrypt.Net.BCrypt.HashPassword("Test1234");
        db.Users.Add(new User { Email = "user@test.com", PasswordHash = hash, IsActive = false });
        await db.SaveChangesAsync();

        var handler = new LoginCommandHandler(db, _tokenServiceMock.Object);
        var result = await handler.Handle(new LoginCommand("user@test.com", "Test1234"), default);

        Assert.False(result.IsSuccess);
        Assert.Contains("aktif değil", result.Error);
    }
}
