using AspLearn.Exceptions;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Repositories;
using AspLearn.Services;
using BCrypt.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog.Core;

namespace AspLearn.UnitTests;

public class AuthServiceTests
{
    private readonly AuthService sut;
    private readonly Mock<IUsersRepository> usersRepository;
    private readonly Mock<IConfiguration> config;
    private readonly Mock<ILogger<AuthService>> logger;

    public AuthServiceTests()
    {
        usersRepository = new Mock<IUsersRepository>();
        logger = new Mock<ILogger<AuthService>>();

        config = new Mock<IConfiguration>();

        config.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyThatIsAtLeast32CharactersLong!");
        config.Setup(c => c["Jwt:Issuer"]).Returns("TaskFlowApp");
        config.Setup(c => c["Jwt:Audience"]).Returns("TaskFlowApp");

        sut = new AuthService(config.Object, usersRepository.Object, logger.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserDoesNotExist_CreatesUser()
    {
        var registerDto = new RegisterDto("fill", "aut");

        await sut.RegisterAsync(registerDto);

        usersRepository.Verify(
           repo => repo.AddUserAsync(It.Is<User>(u => u.Login == "fill")),
           Times.Once
);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserAlreadyExists_ThrowsBadRequestException()
    {
        usersRepository.Setup(repo => repo
            .GetUserByLoginAsync("fill"))
            .ReturnsAsync(new User { Login = "fill"});

        var registerDto = new RegisterDto("fill", "aut");

        var register = async () => await sut.RegisterAsync(registerDto);

        await register.Should().ThrowAsync<BadRequestException>();

        usersRepository.Verify(repo => repo.GetUserByLoginAsync("fill"), Times.Once);
        usersRepository.Verify(repo => repo.AddUserAsync(It.Is<User>(u => u.Login == "fill")), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ThrowsUnauthorizedException()
    {
        var hashPassword = BCrypt.Net.BCrypt.HashPassword("correct_password");

        usersRepository
            .Setup(repo => repo.GetUserByLoginAsync("fill"))
            .ReturnsAsync(new User { Login = "fill", Password = hashPassword});

        var loginDto = new LoginDto("fill", "wrong_password");

        var login = async () => await sut.LoginAsync(loginDto);

        await login.Should().ThrowAsync<UnauthorizedException>();

        usersRepository.Verify(repo => repo.GetUserByLoginAsync("fill"), Times.Once);
    }
}
