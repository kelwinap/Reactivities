using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAcessor;
        public UserAccessor(IHttpContextAccessor httpContextAcessor)
        {
            _httpContextAcessor = httpContextAcessor;
        }

        public string GetUserName()
        {
            return _httpContextAcessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}