using Filmothek.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
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


        //Registering a new User
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

        //Login
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
                var findUser = database.Customer.Where(a => a.Login == values.username).FirstOrDefault();
                int permission = findUser.Rights;
                return Ok(new { Token = tokenString, permission });

            }
            else if (database.Moderator.Any((y => values.username == y.Login && values.password == y.Pw)))
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
                var findUser = database.Moderator.Where(a => a.Login == values.username).FirstOrDefault();
                int permission = findUser.Rights;
                return Ok(new { Token = tokenString, permission });

            }

            return Unauthorized();
        }

        //send info about logged in user
        [HttpGet("user")]
        public IActionResult Userdata()
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            User info = new User(findUser);
            return Ok(info);
        }
        [HttpGet("user/{id}")]
        public IActionResult UserById(int id)
        {
            var findUser = database.Customer.FirstOrDefault(x => x.Id == id);
            User user = new User(findUser);
            return Ok(user);
        }


        [HttpGet("allUsers")]
        public IActionResult GetAllUsers()
        {
            var Customers = database.Customer.ToList();
            List<User> Users = new List<User>();
            for(int i=0;i<Customers.Count;i++)
            {
                User user = new User(Customers[i]);
                Users.Add(user);
            }
            return Ok(Users);
        }

        [HttpGet("allAdmins")]
        public IActionResult GetAllAdmins()
        {
            var Admins = database.Moderator.ToList();
            List<User> Users = new List<User>();
            for (int i = 0; i < Admins.Count; i++)
            {
                User user = new User(Admins[i]);
                Users.Add(user);
            }
            return Ok(Users);
        }

        //edit PW and Address of user
        [HttpPost("edituser")]
        public async Task<IActionResult> EditUserdataAsync(string password, string address)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            var info = new Customer();
            info.Id = findUser.Id;
            if (password != "")
                info.Pw = password;
            if (address != "")
                info.Address = address;
            database.Customer.Update(info);
            await database.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("editUserAdmin")]
        public async Task<IActionResult> EditForeignUserData(User user)
        {
            
        }

        //get List of all movies
        [HttpGet("movies")]
        public List<Movie> Movies() => database.Movie.ToList();

        //get single movie by ID
        [HttpGet("movie/{id}")]
        public ActionResult<Movie> GetById(int id)
        {
            var movie = database.Movie.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }

        //add a new movie to the DB
        [HttpPost("addMovie")]
        public async Task<IActionResult> AddMovie(Movie mDetails)
        {
            string UserName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == UserName))
            {
                var findUser = database.Moderator.Where(a => a.Login == UserName).FirstOrDefault();
                if (!(database.Movie.Any(x => x == mDetails)))
                {
                    Movie newMovie = new Movie();
                    newMovie = mDetails;
                    //ModeratorHistory newActivity = new ModeratorHistory();
                    //newActivity.ModeratorId = findUser.Id;
                    //newActivity.Activity = String.Format("Moderator {0} added a new movie with Id {1} and {2} on {3}.", findUser.Login, newMovie.Id, newMovie.MovieName, DateTime.Now);
                    //newActivity.Date = DateTime.Now;
                    database.Movie.Add(newMovie);
                    await database.SaveChangesAsync();
                    //database.ModeratorHistory.Add(newActivity);
                    //await database.SaveChangesAsync();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        //edit a movie on the db, requires mod permission
        [HttpPut("editMovie")]
        public async Task<IActionResult> EditMovie(Movie mDetails, int id)
        {
            string UserName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == UserName))
            {
                var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
                var findMovie = database.Movie.Where(a => a.Id == id).FirstOrDefault();
                findMovie = mDetails;
                findMovie.Id = id;
                database.Movie.Update(findMovie);
                await database.SaveChangesAsync();
                ModeratorHistory newActivity = new ModeratorHistory();
                newActivity.ModeratorId = findUser.Id;
                newActivity.Activity = String.Format("Moderator {0} edited a movie with Id {1} and {2} on {3}.", findUser.Login, findMovie.Id, findMovie.MovieName, DateTime.Now);
                newActivity.Date = DateTime.Now;
                return NoContent();
            }
            return Unauthorized();
        }

        //delete a movie on the db, requires mod permission
        [HttpDelete("deleteMovie/{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            string UserName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == UserName))
            {
                var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
                var findMovie = await database.Movie.FindAsync(id);
                ModeratorHistory newActivity = new ModeratorHistory();
                newActivity.ModeratorId = findUser.Id;
                newActivity.Activity = String.Format("Moderator {0} deleted a movie with Id {1} and {2} on {3}.", findUser.Login, findMovie.Id, findMovie.MovieName, DateTime.Now);
                newActivity.Date = DateTime.Now;
                database.Movie.Remove(findMovie);
                await database.SaveChangesAsync();
                return NoContent();
            }
            return Unauthorized();
        }

        //edit a customer by a mod
        [HttpPut("editCusomter/{id}")]
        public async Task<IActionResult> EditCustomer(Customer cInfo, int id)
        {
            string modLogin = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modLogin))
            {
                var findUser = database.Customer.Where(a => a.Id == id).FirstOrDefault();
                var findModerator = database.Moderator.Where(a => a.Login == modLogin).FirstOrDefault();
                findUser = cInfo;
                findUser.Id = id;
                database.Customer.Update(findUser);
                await database.SaveChangesAsync();
                ModeratorHistory newActivity = new ModeratorHistory();
                newActivity.ModeratorId = findModerator.Id;
                newActivity.Activity = String.Format("Moderator {0} edited a movie with Id {1} and {2} on {3}.", findModerator.Login, findUser.Id, findUser.FirstName, findUser.LastName, DateTime.Now);
                newActivity.Date = DateTime.Now;
                return NoContent();
            }
            return Unauthorized();
        }

        //delete a customer by a mod
        [HttpDelete("deleteCustomer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            string modLogin = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modLogin))
            {
                var findModerator = database.Moderator.Where(a => a.Login == modLogin).FirstOrDefault();
                var findUser = database.Customer.Where(a => a.Id == id).FirstOrDefault();
                ModeratorHistory newActivity = new ModeratorHistory();
                newActivity.ModeratorId = findModerator.Id;
                newActivity.Activity = String.Format("Moderator {0} deleted a user with Id {1} and {2} {3} on {4}.", findModerator.Login, findUser.Id, findUser.FirstName, findUser.LastName, DateTime.Now);
                newActivity.Date = DateTime.Now;
                database.Customer.Remove(findUser);
                await database.SaveChangesAsync();
                return RedirectToAction();
            }
            return Unauthorized();
        }

        //return full mod log, accesible by admin only
        [HttpGet("moderatorHistory")]
        public List<ModeratorHistory> ShowModeratorHistory()
        {
            string modName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modName && database.Moderator.Any(y => y.Rights == 3)))
            {
                var findMod = database.Moderator.Where(a => a.Login == modName).FirstOrDefault();
                var history = database.ModeratorHistory.Where(x => x.ModeratorId == findMod.Id).ToList();
                return history;
            }
            return new List<ModeratorHistory>();
        }

        //create new mod
        [HttpPost("registerMod")]
        public async Task<IActionResult> RegisterModerator(Account values)
        {
            string modName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modName && database.Moderator.Any(y => y.Rights == 3)))
            {
                if (!(database.Customer.Any(y => values.Login == y.Login)) || (!(database.Moderator.Any(y => values.Login == y.Login))))
                {
                    var newUser = new Moderator()
                    {
                        LastName = values.LastName,
                        FirstName = values.FirstName,
                        Address = values.Address,
                        Login = values.Login,
                        Pw = values.Pw,
                        Rights = 2
                    };
                    database.Moderator.Add(newUser);
                    await database.SaveChangesAsync();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        //edit a mod
        [HttpPut("editModerator/{id}")]
        public async Task<IActionResult> EditModerator(Moderator cInfo, int id)
        {
            string modLogin = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modLogin && database.Moderator.Any(y => y.Rights == 3)))
            {
                var findUser = database.Moderator.Where(a => a.Id == id).FirstOrDefault();
                var findModerator = database.Moderator.Where(a => a.Login == modLogin).FirstOrDefault();
                findUser = cInfo;
                findUser.Id = id;
                database.Moderator.Update(findUser);
                await database.SaveChangesAsync();
                ModeratorHistory newActivity = new ModeratorHistory();
                newActivity.ModeratorId = findModerator.Id;
                newActivity.Activity = String.Format("Moderator {0} edited a movie with Id {1} and {2} on {3}.", findModerator.Login, findUser.Id, findUser.FirstName, findUser.LastName, DateTime.Now);
                newActivity.Date = DateTime.Now;
                return NoContent();
            }
            return Unauthorized();
        }

        //delete a mod
        [HttpDelete("deleteModerator/{id}")]
        public async Task<IActionResult> DeleteModerator(int id)
        {
            string modLogin = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modLogin && database.Moderator.Any(y => y.Rights == 3)))
            {
                var findModerator = database.Moderator.Where(a => a.Login == modLogin).FirstOrDefault();
                var findUser = database.Customer.Where(a => a.Id == id).FirstOrDefault();
                ModeratorHistory newActivity = new ModeratorHistory();
                newActivity.ModeratorId = findModerator.Id;
                newActivity.Activity = String.Format("Moderator {0} deleted a user with Id {1} and {2} {3} on {4}.", findModerator.Login, findUser.Id, findUser.FirstName, findUser.LastName, DateTime.Now);
                newActivity.Date = DateTime.Now;
                database.Customer.Remove(findUser);
                await database.SaveChangesAsync();
                return NoContent();
            }
            return Unauthorized();
        }

        //return movies by searchparameter name
        [HttpGet("searchmovie/{id}")]
        public ActionResult<Movie> GetByName(string Mname)
        {
            var movie = database.Movie.Find(Mname);
            return movie == null ? (ActionResult<Movie>)NotFound() : (ActionResult<Movie>)movie;
        }

        //Customer check if a movie is rented by them
        [HttpGet("check/{id}")]
        public ActionResult<Movie> CheckMovie(string Mname, int id)
        {
            string UserName = User.Identity.Name;
            var FindUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            var MovieEntries = database.CustomerHistory.Where(a => a.CustomerId == FindUser.Id && a.MovieId == id).FirstOrDefault();
            bool available = false;
            if (database.CustomerHistory.Any(x => x.IsBorrowing == true && x.MovieId == id))
            {
                if ((MovieEntries.StartDate < DateTime.Now) && (MovieEntries.EndDate > DateTime.Now))
                {
                    available = true;
                }
            }
            return Ok(available);
        }

        //borrow a movie
        [HttpPost("rent")]
        public async Task<IActionResult> RentMovie([FromBody]int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            var findMovie = database.Movie.Where(a => a.Id == id).FirstOrDefault();
            if (!(database.CustomerHistory.Any(a => a.MovieId == findMovie.Id) && !(database.CustomerHistory.Any(b => b.CustomerId == findUser.Id))))
            {
                var newHistory = new CustomerHistory()
                {
                    MovieId = findMovie.Id,
                    CustomerId = findUser.Id,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    IsBorrowing = true
                };
                database.CustomerHistory.Add(newHistory);
                await database.SaveChangesAsync();
            }
            return NoContent();

        }

        //add a movie to the current wishlist
        [HttpPost("addWishlist/{id}")]
        public async Task<IActionResult> AddToWishlist(int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            var findMovie = database.Movie.Where(a => a.Id == id).FirstOrDefault();
            var newHistory = new CustomerHistory()
            {
                MovieId = findMovie.Id,
                CustomerId = findUser.Id,
                StartDate = DateTime.Now,
                IsBorrowing = false
            };
            database.CustomerHistory.Add(newHistory);
            await database.SaveChangesAsync();
            return NoContent();

        }

        //Customer: delete a wishlist entry
        [HttpDelete("deletewishlist/{id}")]
        public async Task<IActionResult> DeleteWishlistMovie(int id)
        {
            if (database.CustomerHistory.Any(x => x.IsBorrowing == false))
            {
                string UserName = User.Identity.Name;
                var findMovie = await database.CustomerHistory.FindAsync(id, UserName);
                database.CustomerHistory.Remove(findMovie);
                await database.SaveChangesAsync();
            }
            return NoContent();
        }


        [HttpPost("note/{id}")]
        public async Task<IActionResult> Note(string text, int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            var findActivity = database.CustomerHistory.Where(x => x.MovieId == id && x.CustomerId == findUser.Id).FirstOrDefault();
            var addNote = new CustomerHistory();
            addNote.Id = findActivity.Id;
            addNote.Note = text;
            database.CustomerHistory.Update(addNote);
            await database.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("deleteNote/{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            var findActivity = database.CustomerHistory.Where(x => x.MovieId == id && x.CustomerId == findUser.Id).FirstOrDefault();
            var deleteNote = new CustomerHistory();
            deleteNote.Id = findActivity.Id;
            deleteNote.Note = "";
            database.CustomerHistory.Update(deleteNote);
            await database.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("history")]
        public IActionResult GetCustomerHistory()
        {
            string UserName = User.Identity.Name;
            var findId = database.Customer.FirstOrDefault(x => x.Login == UserName);
            var findHistory = database.CustomerHistory.Where(x => x.IsBorrowing == true || findId.Id == x.CustomerId).ToList();
            List<CustomerHistorymask> History = new List<CustomerHistorymask>(findHistory.Count);
            int i = 0;
            for (i=0; i<findHistory.Count;i++)
            {
                string MovieName = database.Movie.FirstOrDefault(x => x.Id == findHistory[i].MovieId).MovieName;
                CustomerHistorymask tempHistory = new CustomerHistorymask(findHistory[i].Id, MovieName, findHistory[i].StartDate, findHistory[i].EndDate);
                History.Add(tempHistory);
               /* History[i].Id = findHistory[i].Id;
                History[i].MovieId = findHistory[i].MovieId;
                History[i].StartDate = findHistory[i].StartDate;
                History[i].EndDate = findHistory[i].EndDate;*/

            }

            return Ok(History);
        }

        [HttpGet("wishlist")]
        public List<CustomerHistory> ShowWishlist() => database.CustomerHistory.Where(x => x.IsBorrowing == false).ToList();


        //Customer: view receive payment info
        [HttpGet("payment")]
        public ActionResult Payment()
        {
            string UserName = User.Identity.Name;
            Customer findPayment = new Customer();
            var findCustomer = database.Customer.Where(y => UserName == y.Login).FirstOrDefault();
            PaymentMask findPaymentdata = new PaymentMask();
            var findPaymentList = database.PaymentMethod.Where(y => findCustomer.Id == y.CustomerId).FirstOrDefault();
            findPaymentdata.fromPaymentMethod(findPaymentList);
            return Ok(findPaymentdata);
        }

        //Customer: edit payment info
        [HttpPost("addpayment")]
        public async Task<IActionResult> AddPaymentMethod(PaymentMethod values)
        {
            string UserName = User.Identity.Name;
            var idk = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            if (!(database.PaymentMethod.Any((y => idk.Id == y.CustomerId))))
            {
                var newPaymentMethod = new PaymentMethod();
                newPaymentMethod = values;
                database.PaymentMethod.Add(newPaymentMethod);
                await database.SaveChangesAsync();
            }
            else
            {
                var findPaymentMethod = database.PaymentMethod.Where(x => x.CustomerId == idk.Id).FirstOrDefault();
                findPaymentMethod = values;
                findPaymentMethod.CustomerId = idk.Id;
                database.PaymentMethod.Update(findPaymentMethod);
                await database.SaveChangesAsync();
            }
            return NoContent();


        }
    }
}
