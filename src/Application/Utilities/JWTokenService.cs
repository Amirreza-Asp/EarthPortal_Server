using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Utilities
{
    public static class JWTokenService
    {
        public static SymmetricSecurityKey Key =>
           new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dklsdsdfnasdklsdjlfsdjlfnlfnsdkl564564fnklefnsdklfnasdklfnsdjlfsds455wofwifewfoiwf"));

        public static string GenerateToken(String userName, String role, String ipAddress, String? email, String? phoneNumber)
        {
            var claims = new List<Claim>
            {
                new Claim(AppClaims.UserName , userName),
                new Claim(AppClaims.PhoneNumber ,phoneNumber),
                new Claim(AppClaims.Email , email),
                new Claim(AppClaims.Role , role),
                new Claim(AppClaims.IpAddress , ipAddress),
            };

            var cred = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static DateTime GetTokenExpirationTime(this ClaimsIdentity claims)
        {
            var tokenExp = claims.FindFirst(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ConvertFromUnixTimestamp(ticks);
        }


        public static String GetTokenUserName(this ClaimsIdentity claims)
        {
            return claims.FindFirst(claim => claim.Type == AppClaims.UserName).Value;
        }

        public static String GetTokenRole(this ClaimsIdentity claims)
        {
            return claims.FindFirst(claim => claim.Type == AppClaims.Role).Value;
        }

        public static String GetTokenIpAddress(this ClaimsIdentity claims)
        {
            return claims.FindFirst(claim => claim.Type == AppClaims.IpAddress).Value;
        }

        private static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }
}
