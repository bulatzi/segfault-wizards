using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AbetApi.Authentication
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly string secretApiKey;

        public TokenGenerator(IConfiguration Configuration)
        {
            secretApiKey = Configuration.GetValue<string>("SecretApiKey");
        }

        public string GenerateToken(string EUID, List<string> roles)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secretApiKey);

            //Creates a claim object with their name and all roles
            var subject = new ClaimsIdentity();
            subject.AddClaim(new Claim(ClaimTypes.Name, EUID));
            foreach(var role in roles)
            {
                subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            //Builds the token descriptor
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            //Actually turns that data in to a token
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
