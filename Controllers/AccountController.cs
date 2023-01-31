using LiteraturePlatformWebApi.Data;
using LiteraturePlatformWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace LiteraturePlatformWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
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
            if (us != null)
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

                return Ok(encodedJwt);

            }
            return BadRequest("Email or password is incorrect");
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
        public async Task<ActionResult<string>> Register(RegisterModel registerModel)
        {
            User a = await _context.Users.Where(e => e.Email == registerModel.Email).FirstOrDefaultAsync();
            if (a != null)
            {
                return BadRequest("User with this email already exist");
            }

            User user = new User()
            {
                Login = registerModel.Login,
                Email = registerModel.Email,
                Password = registerModel.Password
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("nice");
        }
    }
}
