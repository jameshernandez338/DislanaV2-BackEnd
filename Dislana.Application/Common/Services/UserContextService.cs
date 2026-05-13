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

        public string? GetId()
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
