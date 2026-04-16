using Dislana.Application.Auth.DTOs;
using Dislana.Application.Auth.Interfaces;
using Dislana.Application.Auth.Results;
using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Domain.Exceptions;

namespace Dislana.Application.Auth
{
    /// <summary>
    /// Application Service: Solo oquesta, toda la lógica de negocio está en el Dominio
    /// </summary>
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
            try
            {
                // Verificar si el email ya existe
                var existingUser = await _userRepository.GetUserByUserNameAsync(request.Email, cancellationToken);
                if (existingUser != null)
                    return LoginResult.Fail("El correo electrónico ya está registrado.");

                // Crear entidades de dominio (validaciones en Factory Methods)
                var user = UserEntity.CreateForRegistration(request.Name, request.LastName, request.Email);
                var passwordHash = _passwordHasher.Hash(request.Password);
                var credential = UserCredentialEntity.CreateForNewUser(passwordHash);

                // Persistir (el repositorio asigna el ID)
                var createdUser = await _userRepository.CreateUserWithCredentialAsync(user, credential, cancellationToken);
                if (createdUser == null)
                    return LoginResult.Fail("Error al crear el usuario");

                // Generar tokens
                var accessToken = _jwtTokenGenerator.Generate(createdUser);
                var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
                var refreshToken = RefreshTokenEntity.Create(refreshTokenValue, createdUser.Id);

                // Guardar refresh token
                await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken, cancellationToken);

                return LoginResult.Success(accessToken, refreshTokenValue, createdUser.FullName);
            }
            catch (DomainException ex)
            {
                return LoginResult.Fail(ex.Message);
            }
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener usuario
                var user = await _userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken);
                if (user is null)
                    return LoginResult.Fail("Credenciales inválidas");

                // Validar que puede hacer login (regla de negocio en el dominio)
                user.ValidateCanLogin();

                // Verificar credenciales
                var credentials = await _userCredentialRepository.GetCredentialByUserIdAsync(user.Id, cancellationToken);
                if (credentials is null)
                    return LoginResult.Fail("Credenciales inválidas");

                if (!_passwordHasher.Verify(request.Password, credentials.PasswordHash))
                    return LoginResult.Fail("Credenciales inválidas");

                // Generar tokens
                var accessToken = _jwtTokenGenerator.Generate(user);
                var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
                var refreshToken = RefreshTokenEntity.Create(refreshTokenValue, user.Id);

                // Revocar tokens anteriores y guardar nuevo
                await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);
                await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken, cancellationToken);

                return LoginResult.Success(accessToken, refreshTokenValue, user.FullName);
            }
            catch (DomainException ex)
            {
                return LoginResult.Fail(ex.Message);
            }
        }

        public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                    return LoginResult.Fail("Refresh token es requerido");

                // Obtener token
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
                if (storedToken == null)
                    return LoginResult.Fail("Refresh token inválido");

                // Validar (regla de negocio en el dominio)
                storedToken.ValidateForUse();

                // Obtener usuario
                var user = await _userRepository.GetUserByIdAsync(storedToken.UserId, cancellationToken);
                if (user == null)
                    return LoginResult.Fail("Usuario no encontrado");

                user.ValidateCanLogin();

                // Generar nuevos tokens
                var newAccessToken = _jwtTokenGenerator.Generate(user);
                var newRefreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
                var newRefreshToken = RefreshTokenEntity.Create(newRefreshTokenValue, user.Id);

                // Revocar token anterior y guardar nuevo
                await _refreshTokenRepository.RevokeTokenAsync(refreshToken, cancellationToken);
                await _refreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken, cancellationToken);

                return LoginResult.Success(newAccessToken, newRefreshTokenValue, user.FullName);
            }
            catch (DomainException ex)
            {
                return LoginResult.Fail(ex.Message);
            }
        }
    }
}
