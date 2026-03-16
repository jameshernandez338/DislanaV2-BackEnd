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
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IUserCredentialRepository userCredentialRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _userCredentialRepository = userCredentialRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
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

            return LoginResult.Success(token, user.FullName);
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

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

            return LoginResult.Success(token, user.FullName);
        }
    }
}
