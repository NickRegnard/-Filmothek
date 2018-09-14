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
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(Account values)
        {
            if (!(database.Customer.Any(y => values.Login == y.Login)) || (!(database.Moderator.Any(y => values.Login == y.Login))))
            {
                var newUser = new Customer()
                {
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
        [HttpGet("user")]
        public IActionResult Userdata()
        {
            string UserName = User.Identity.Name;
            Customer info = new Customer();
            var findUser = database.Customer.Where(a => a.Login == UserName).ToList();
            info = findUser[0];
            return Ok(info);

        }
        [HttpPost("edituser")]
        public async Task<IActionResult> EditUserdataAsync(string password, string address)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).ToList();
            var info = new Customer();
            info.Id = findUser[0].Id;
            if (password != "")
                info.Pw = password;
            if (address != "")
                info.Address = address;
            database.Customer.Update(info);
            await database.SaveChangesAsync();
            return NoContent();

        }
        [HttpGet("movies")]
        public List<Movie> Movies() => database.Movie.ToList();

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
        [HttpPost("rentMoive{id}")]
        public async Task<IActionResult> RentMovie(int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).ToList();
            var findMovie = database.Movie.Where(a => a.Id == id).ToList();
            var newHistory = new CustomerHistory()
            {
                MovieId = findMovie[0].Id,
                CustomerId = findUser[0].Id,
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(30),
                isBorrowing = true
            };
            database.CustomerHistory.Add(newHistory);
            await database.SaveChangesAsync();
            return NoContent();

        }
        [HttpPost("addwishlist{id}")]
        public async Task<IActionResult> AddToWishlist(int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).ToList();
            var findMovie = database.Movie.Where(a => a.Id == id).ToList();
            var newHistory = new CustomerHistory()
            {
                MovieId = findMovie[0].Id,
                CustomerId = findUser[0].Id,
                startDate = DateTime.Now,
                isBorrowing = false
            };
            database.CustomerHistory.Add(newHistory);
            await database.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("deletewishlist{id}")]
        public async Task<IActionResult> DeleteWishlistMovie(int id)
        {
            if (database.CustomerHistory.Any(x => x.isBorrowing == false))
            {
                string UserName = User.Identity.Name;
                var findMovie = await database.CustomerHistory.FindAsync(id, UserName);
                database.CustomerHistory.Remove(findMovie);
                await database.SaveChangesAsync();
            }
            return NoContent();
        }
        [HttpPost("note{id}")]
        public async Task<IActionResult> Note(string text, int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).ToList();
            var findActivity = database.CustomerHistory.Where(x => x.MovieId == id && x.CustomerId == findUser[0].Id).ToList();
            var addNote = new CustomerHistory();
            addNote.Id = findActivity[0].Id;
            addNote.Note = text;
            database.CustomerHistory.Update(addNote);
            await database.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("deleteNote{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).ToList();
            var findActivity = database.CustomerHistory.Where(x => x.MovieId == id && x.CustomerId == findUser[0].Id).ToList();
            var deleteNote = new CustomerHistory();
            deleteNote.Id = findActivity[0].Id;
            deleteNote.Note = "";
            database.CustomerHistory.Update(deleteNote);
            await database.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("history")]
        public List<CustomerHistory> ShowHistory() => database.CustomerHistory.Where(x => x.isBorrowing == true).ToList();

        [HttpGet("wishlist")]
        public List<CustomerHistory> ShowWishlist() => database.CustomerHistory.Where(x => x.isBorrowing == false).ToList();

        [HttpGet("payment")]
        public ActionResult Payment()
        {
            string UserName = "SamWills"; //User.Identity.Name;
            Customer findPayment = new Customer();
            var findCustomer = database.Customer.Where(y => UserName == y.Login).ToList();
            //findPayment.Login = findCustomer[0].Login;
            PaymentMask findPaymentdata = new PaymentMask();
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
            var idk = database.Customer.Where(a => a.Login == UserName).ToList();
            if (!(database.PaymentMethod.Any((y => idk[0].Id == y.CustomerId))))
            {
                var newPaymentMethod = new PaymentMethod();
                newPaymentMethod = values;
                database.PaymentMethod.Add(newPaymentMethod);
                await database.SaveChangesAsync();
            }
            else
            {
                var newPaymentMethod = new PaymentMethod() { CustomerId = idk[0].Id };
                var thing = database.PaymentMethod.ToList();
                foreach (var x in thing)
                {
                    switch (x)
                    {
                        case null:
                            break;
                        default:
                            newPaymentMethod = x;
                            break;
                    }
                }                
                database.PaymentMethod.Update(newPaymentMethod);
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
