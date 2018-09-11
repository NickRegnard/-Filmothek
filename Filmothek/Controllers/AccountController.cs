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

        public AccountController(VideoContext context)
        {
            database = context;
        }
        [HttpGet("user")]
        public IActionResult Userdata()
        {
            string UserName = User.Identity.Name;
            Customer info = new Customer();
            var idk = database.Customer.Where(a => a.Login == UserName).ToList();
            info.FirstName = idk[0].FirstName;
            info.LastName = idk[0].LastName;
            info.Address = idk[0].Address;
            info.Login = idk[0].Login;
            info.Id = idk[0].Id;
            return Ok(info);

        }
        [HttpGet("movies")]
        public List<Movie> Movies()
        {
            return database.Movie.ToList();
        }
        [HttpGet("movie{id}")]
        public ActionResult<Movie> GetById(int id)
        {
            var item = database.Movie.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
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
                claims: claims,
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
