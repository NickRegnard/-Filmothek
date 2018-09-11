using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Filmothek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Filmothek.Controllers
{
    //edited
    [Route("api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly VideoContext database;
        
        AccountController(VideoContext context)
        {
            database = context;
        }
        [HttpGet("user"), AllowAnonymous]
        public IActionResult Userdata()
        {
            var UserName = User.Identity.Name;
            Customer info = new Customer();
            //info.Id = database.Customer.Where(x => x.Login == UserName).ToInt();
            info.FirstName = database.Customer.Where(x => x.Login == UserName).ToString();
            info.LastName = database.Customer.Where(x => x.Login == UserName).ToString();
            info.Address = database.Customer.Where(x => x.Login == UserName).ToString();
            info.Login = database.Customer.Where(x => x.Login == UserName).ToString();

            return Ok(info);

        }
        [HttpGet("movies"), AllowAnonymous]
        public IActionResult Movies()
        {
            //var UserName = User.Identity.Name;
            //Customer info = new Customer();
            ////info.Id = database.Customer.Where(x => x.Login == UserName).ToInt();
            //info.FirstName = database.Customer.Where(x => x.Login == UserName).ToString();
            //info.LastName = database.Customer.Where(x => x.Login == UserName).ToString();
            //info.Address = database.Customer.Where(x => x.Login == UserName).ToString();
            //info.Login = database.Customer.Where(x => x.Login == UserName).ToString();

            return Ok();

        }

        [HttpPost("login")]
        public IActionResult Login(Password values)
        {
            //habs nicht verstanden... variable ist username von values vom file Password :D
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, values.username),
            };
            if (values.password == null || values.username == null) return NotFound();

            if (database.Customer.Any((y => values.username == y.Login && values.password == y.Pw)))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:50000",
                audience: "http://localhost:4200",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(new { Token = tokenString });

            }

            return Unauthorized();
        }


        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
