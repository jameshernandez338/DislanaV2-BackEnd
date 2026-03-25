namespace Dislana.Application.Auth.Results
{
    public class LoginResult
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public string? Token { get; }
        public string? RefreshToken { get; }
        public string? FullName { get; }

        private LoginResult(bool success, string message, string token = null, string refreshToken = null, string fullName = null)
        {
            IsSuccess = success;
            Message = message;
            Token = token;
            RefreshToken = refreshToken;
            FullName = fullName;
        }

        public static LoginResult Fail(string message)
            => new LoginResult(false, message);

        public static LoginResult Success(string token, string refreshToken, string fullName)
            => new LoginResult(true, "", token, refreshToken, fullName);
    }
}
