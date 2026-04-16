using Dislana.Application.Auth;
using Dislana.Application.Auth.DTOs;
using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using FluentAssertions;
using Moq;

namespace Dislana.Tests.Application
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserCredentialRepository> _credentialRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;

        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _credentialRepositoryMock = new Mock<IUserCredentialRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();

            _service = new AuthService(
                _userRepositoryMock.Object,
                _credentialRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtGeneratorMock.Object
            );
        }

        [Fact]
        public async Task Register_Should_Fail_When_Email_Already_Exists()
        {
            var request = new RegisterRequest("John", "Doe", "john@test.com", "123456");

            // Usar Reconstitute en lugar del constructor
            var existingUser = UserEntity.Reconstitute(1, "john@test.com", "john@test.com", "John", "Doe", true);

            _userRepositoryMock
                .Setup(x => x.GetUserByUserNameAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var result = await _service.RegisterAsync(request, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Register_Should_Create_User_And_Return_Token()
        {
            var request = new RegisterRequest("John", "Doe", "john@test.com", "123456");

            _userRepositoryMock
                .Setup(x => x.GetUserByUserNameAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            var createdUser = UserEntity.Reconstitute(1, "john@test.com", "john@test.com", "John", "Doe", true);

            _userRepositoryMock
                .Setup(x => x.CreateUserWithCredentialAsync(
                    It.IsAny<UserEntity>(),
                    It.IsAny<UserCredentialEntity>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Usar un hash realista (BCrypt tiene ~60 caracteres)
            var validHash = "$2a$11$K6RrqQgz7G0s1xOeK6RrqOeK6RrqOeK6RrqOeK6RrqOeK6RrqOe";
            _passwordHasherMock
                .Setup(x => x.Hash(request.Password))
                .Returns(validHash);

            _jwtGeneratorMock
                .Setup(x => x.Generate(createdUser))
                .Returns("token123");

            _jwtGeneratorMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns("refreshToken123");

            // Mock para guardar refresh token
            _refreshTokenRepositoryMock
                .Setup(x => x.SaveRefreshTokenAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.RegisterAsync(request, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Login_Should_Fail_When_User_Not_Found()
        {
            var request = new LoginRequest("john@test.com", "123456");

            _userRepositoryMock
                .Setup(x => x.GetUserByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            var result = await _service.LoginAsync(request, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Login_Should_Fail_When_Password_Is_Invalid()
        {
            var request = new LoginRequest("john@test.com", "wrong");

            var user = UserEntity.Reconstitute(1, "john@test.com", "john@test.com", "John", "Doe", true);

            _userRepositoryMock
                .Setup(x => x.GetUserByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Usar un hash realista (BCrypt tiene ~60 caracteres)
            var validHash = "$2a$11$K6RrqQgz7G0s1xOeK6RrqOeK6RrqOeK6RrqOeK6RrqOeK6RrqOe";
            var credential = UserCredentialEntity.Reconstitute(user.Id, validHash, DateTime.UtcNow);

            _credentialRepositoryMock
                .Setup(x => x.GetCredentialByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(credential);

            _passwordHasherMock
                .Setup(x => x.Verify("wrong", validHash))
                .Returns(false);

            var result = await _service.LoginAsync(request, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Login_Should_Return_Token_When_Success()
        {
            var request = new LoginRequest("john@test.com", "123456");

            var user = UserEntity.Reconstitute(1, "john@test.com", "john@test.com", "John", "Doe", true);

            _userRepositoryMock
                .Setup(x => x.GetUserByUserNameAsync(request.UserName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Usar un hash realista (BCrypt tiene ~60 caracteres)
            var validHash = "$2a$11$K6RrqQgz7G0s1xOeK6RrqOeK6RrqOeK6RrqOeK6RrqOeK6RrqOe";
            var credential = UserCredentialEntity.Reconstitute(user.Id, validHash, DateTime.UtcNow);

            _credentialRepositoryMock
                .Setup(x => x.GetCredentialByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(credential);

            _passwordHasherMock
                .Setup(x => x.Verify("123456", validHash))
                .Returns(true);

            _jwtGeneratorMock
                .Setup(x => x.Generate(user))
                .Returns("token123");

            _jwtGeneratorMock
                .Setup(x => x.GenerateRefreshToken())
                .Returns("refreshToken123");

            // Mock para revocar tokens anteriores
            _refreshTokenRepositoryMock
                .Setup(x => x.RevokeAllUserTokensAsync(user.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Mock para guardar nuevo refresh token
            _refreshTokenRepositoryMock
                .Setup(x => x.SaveRefreshTokenAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.LoginAsync(request, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }
    }
}
