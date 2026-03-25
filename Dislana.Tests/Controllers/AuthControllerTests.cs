using Dislana.Api.Controllers;
using Dislana.Application.Auth.DTOs;
using Dislana.Application.Auth.Interfaces;
using Dislana.Application.Auth.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Dislana.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);

            // Setup HttpContext so Response.Cookies works in tests
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Register_Should_Return_Ok_When_Success()
        {
            var request = new RegisterRequest("John", "Doe", "john@test.com", "123456");

            _authServiceMock
                .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoginResult.Success("token123", "refreshToken123", "John Doe"));

            var result = await _controller.Register(request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            _controller.Response.Headers.SetCookie.ToString().Should().Contain("refreshToken");
        }

        [Fact]
        public async Task Register_Should_Return_Conflict_When_Email_Exists()
        {
            var request = new RegisterRequest("John", "Doe", "john@test.com", "123456");

            _authServiceMock
                .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoginResult.Fail("El correo electrónico ya está registrado."));

            var result = await _controller.Register(request, CancellationToken.None);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized_When_Fails()
        {
            var request = new LoginRequest("john@test.com", "wrongpass");

            _authServiceMock
                .Setup(x => x.LoginAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoginResult.Fail("Credenciales inválidas"));

            var result = await _controller.Login(request, CancellationToken.None);

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_Should_Return_Ok_And_Set_Cookie_When_Success()
        {
            var request = new LoginRequest("john@test.com", "123456");

            _authServiceMock
                .Setup(x => x.LoginAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoginResult.Success("token123", "refreshToken123", "John Doe"));

            var result = await _controller.Login(request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            _controller.Response.Headers.SetCookie.ToString().Should().Contain("refreshToken");
        }

        [Fact]
        public async Task RefreshToken_Should_Return_Unauthorized_When_Cookie_Missing()
        {
            // No cookie set — default empty request
            var result = await _controller.RefreshToken(CancellationToken.None);

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task RefreshToken_Should_Return_Ok_When_Valid_Cookie()
        {
            // Simulate cookie in the request
            _controller.HttpContext.Request.Headers.Append("Cookie", "refreshToken=validToken123");

            _authServiceMock
                .Setup(x => x.RefreshTokenAsync("validToken123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(LoginResult.Success("newToken", "newRefreshToken", "John Doe"));

            var result = await _controller.RefreshToken(CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
