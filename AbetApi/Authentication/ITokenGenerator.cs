using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Authentication
{
    public interface ITokenGenerator
    {
        public string GenerateToken(string userId, string role);
    }
}
