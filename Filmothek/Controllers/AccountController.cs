using Filmothek.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            info = idk[0];
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
            var movie = database.Movie.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }
        [HttpGet("movie{id}", Name = "")]
        public ActionResult<Movie> GetByName(string Mname)
        {
            var movie = database.Movie.Find(Mname);
            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }
        [HttpPost("login")]
        public IActionResult Login(Password values)
        {
            //hab nicht verstanden... variable ist username von values vom file Password :D
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
        [HttpGet("history")]
        public ActionResult CustomerHistory()
        {
            string UserName = User.Identity.Name;
            Customer findHistory = new Customer();
            var findCustomer = database.Customer.Where(y => UserName == y.Login).ToList();
            //findPayment.Login = findCustomer[0].Login;
            CustomerHistory findPaymentdata = new CustomerHistory();
            findHistory.Login = findCustomer[0].Login;
            return Ok(findHistory);
        }
        [HttpGet("payment")]
        public IActionResult Payment()
        {
            string UserName = User.Identity.Name;
            Customer findPayment = new Customer();
            var findCustomer = database.Customer.Where(y => UserName == y.Login).ToList();
            //findPayment.Login = findCustomer[0].Login;
            PaymentMethod findPaymentdata = new PaymentMethod();

            var findPaymentList = database.PaymentMethod.Where(y => findCustomer[0].Id == y.CustomerId).ToList();

            //findPaymentdata.CustomerId = findCustomer[0].Id;
            findPaymentdata = findPaymentList[0];

            return Ok(findPaymentdata);
        }
        [HttpPost("addpayment")]
        public async Task<IActionResult> AddPaymentMethod(Paymentmask values)
        {
            string UserName = User.Identity.Name;
            Customer info = new Customer();
            var idk = database.Customer.Where(a => a.Login == UserName).ToList();
            if (!(info.Id == idk[0].Id))
            {
                var check = database.PaymentMethod.Find(idk[0].Id);
                if (!(check.CustomerId == values.CustomerId))
                {

                }
                return Ok(idk);
            }
            return NoContent();

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(Account values)
        {
            if (!(database.Customer.Any(y => values.Login == y.Login)) || (!(database.Moderator.Any(y => values.Login == y.Login))))
            {
                var newUser = new Customer() {
                    LastName = values.LastName,
                    FirstName = values.FirstName,
                    Address = values.Address,
                    Login = values.Login,
                    Pw = values.Pw,
                    Rights = 1
                };
                database.Customer.Add(newUser);
                await database.SaveChangesAsync();
            }
            return NoContent();
        }


        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
