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
            info.FirstName = idk[0].FirstName;
            info.LastName = idk[0].LastName;
            info.Address = idk[0].Address;
            info.Login = idk[0].Login;
            info.Id = idk[0].Id;
            return Ok(info);

        }
       /[HttpGet("movies")]
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
        [HttpGet("payment")]
        public ActionResult Payment()
        {
            string UserName = "SamWills"; //User.Identity.Name;
            Customer findPayment = new Customer();
            var findCustomer = database.Customer.Where(y => UserName == y.Login).ToList();
            //findPayment.Login = findCustomer[0].Login;
            Paymentmask findPaymentdata = new Paymentmask();
            var findPaymentList = database.PaymentMethod.Where(y => findCustomer[0].Id == y.CustomerId).ToList();
            findPaymentdata.fromPaymentMethod(findPaymentList[0]);            
            /*var findPayment = new PaymentMethod();
            findPayment.CreditcardExpire = "2020-02-01";
            findPayment.CreditcardNumber = 1234000012340000;
            findPayment.CreditcardOwner = "asd dsa";
            findPayment.CreditcardSecret = 123;
            findPayment.CreditcardTyp = "VISA";*/
            return Ok(findPaymentdata);
        }
        [HttpPost("addpayment")]
        public async Task<IActionResult> AddPaymentMethod(PaymentMethod values)
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
