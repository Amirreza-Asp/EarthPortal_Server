using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Infrastructure.Services
{
    public interface ISessionService
    {
        void SetSession(string key, string value);
        string? GetSession(string key);
        void RemoveSession(string key);
    }
}
