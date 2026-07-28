using Dislana.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dislana.Application.Common.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(value, out var userId)
                ? userId
                : null;
        }

        public string? GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindFirst("UserName")?.Value;
        }

        public string? GetEmail()
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
