using Application.Contracts.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Session
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public void SetSession(string key, string value)
        {
            _httpContextAccessor.HttpContext?.Session?.SetString(key, value);
        }

        public string? GetSession(string key)
        {
            return _httpContextAccessor.HttpContext?.Session?.GetString(key);
        }

        public void RemoveSession(string key)
        {
            _httpContextAccessor.HttpContext?.Session?.Remove(key);
        }
    }
}
