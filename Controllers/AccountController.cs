using LiteraturePlatformWebApi.Data;
using LiteraturePlatformWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LiteraturePlatformWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController
    {
        private LiteraturePlatformContext _context;
        public AccountController(LiteraturePlatformContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("Login")]
        public ActionResult<string> Login(User user)
        {
            var us = _context.Users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password);
            if (us!=null)
            {
                var identity = GetIdentity(us); // аутентификация
                var now = DateTime.UtcNow;

                var jwt = new JwtSecurityToken(
                   issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                   signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return encodedJwt;

            }
            return null;
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim("Id", Convert.ToString(user.UserId)),
                    new Claim("Login", Convert.ToString(user.Login))
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IResult> Register(RegisterModel registerModel)
        {

            User user = new User()
            {
                Login = registerModel.Login,
                Email = registerModel.Email,
                Password = registerModel.Password
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return Results.Ok();
        }


    }
}
