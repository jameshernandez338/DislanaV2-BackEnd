using Dislana.Application.Auth.DTOs;
using Dislana.Application.Auth.Interfaces;
using Dislana.Application.Auth.Results;
using Dislana.Domain.Auth.Interfaces;

namespace Dislana.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IUserCredentialRepository userCredentialRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _userCredentialRepository = userCredentialRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByUserNameAsync(request.Email, cancellationToken);
            if (existingUser != null)
                return LoginResult.Fail("El correo electrónico ya está registrado.");

            var hash = _passwordHasher.Hash(request.Password);

            var user = await _userRepository.CreateUserWithCredentialAsync(
               request.Name,
               request.LastName,
               request.Email,
               hash,
               cancellationToken);

            var token = _jwtTokenGenerator.Generate(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            // Save refresh token with 7 days expiration
            await _refreshTokenRepository.SaveRefreshTokenAsync(
                refreshToken, 
                user.Id, 
                DateTime.UtcNow.AddDays(7), 
                cancellationToken);

            return LoginResult.Success(token, refreshToken, user.FullName);
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken);

            if (user is null)
                return LoginResult.Fail("Credenciales inválidas");

            var credentials = await _userCredentialRepository.GetCredentialByUserIdAsync(user.Id, cancellationToken);
            if (credentials is null)
                return LoginResult.Fail("Credenciales inválidas");

            if (!_passwordHasher.Verify(request.Password, credentials.PasswordHash))
                return LoginResult.Fail("Credenciales inválidas");

            if (!user.IsActive)
                return LoginResult.Fail("Usuario inactivo");

            var token = _jwtTokenGenerator.Generate(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();            

            // Revoke all previous refresh tokens for this user
            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);

            await _refreshTokenRepository.SaveRefreshTokenAsync(
                refreshToken, 
                user.Id, 
                DateTime.UtcNow.AddDays(7), 
                cancellationToken);

            return LoginResult.Success(token, refreshToken, user.FullName);
        }

        public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
           if (string.IsNullOrWhiteSpace(refreshToken))
                return LoginResult.Fail("Refresh token es requerido");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

            if (storedToken == null)
                return LoginResult.Fail("Refresh token inválido");

            if (!storedToken.IsValid)
                return LoginResult.Fail("Refresh token expirado o revocado");

            var user = await _userRepository.GetUserByIdAsync(storedToken.UserId, cancellationToken);

            if (user == null || !user.IsActive)
                return LoginResult.Fail("Usuario no encontrado o inactivo");

            // Generate new tokens
            var newAccessToken = _jwtTokenGenerator.Generate(user);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            // Revoke old refresh token
            await _refreshTokenRepository.RevokeTokenAsync(refreshToken, cancellationToken);

            // Save new refresh token
            await _refreshTokenRepository.SaveRefreshTokenAsync(
                newRefreshToken, 
                user.Id, 
                DateTime.UtcNow.AddDays(7), 
                cancellationToken);

            return LoginResult.Success(newAccessToken, newRefreshToken, user.FullName);
        }
    }
}
