﻿using Filmothek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Filmothek.Controllers
{

    [Route("api")]
    [ApiController, Authorize]
    public class AccountController : ControllerBase
    {
        private readonly VideoContext database;

        public AccountController(VideoContext context)
        {
            database = context; 
        }
        

        //returns true if logged in user is a mod
        private bool ModAuthorization()
        {
            string UserName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == UserName)) return true;
            return false;
        }

        //returns true if logged in user is a mod and has higher permission level
        private bool AdminAuthorization()
        {
            string UserName = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == UserName && x.Rights>2)) return true;
            return false;
        }

        //writes the log Entry for a admin action
        //use with await
        private async Task LogAction(string action, int editedId, string editedName)
        {
            string editor = User.Identity.Name;
            ModeratorHistory newActivity = new ModeratorHistory();
            newActivity.ModeratorId = database.Moderator.FirstOrDefault(x => x.Login == editor).Id; 
            newActivity.Activity = String.Format("Admin {0} "+action+" with Id '{1}' and name '{2}' on {3} .", editor, editedId, editedName, DateTime.Now);
            newActivity.Date = DateTime.Now;
            database.ModeratorHistory.Add(newActivity);
            await database.SaveChangesAsync();
        }


        //Registering a new User
        [HttpPost("register"), AllowAnonymous]
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
        [HttpPost("login"), AllowAnonymous]
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
            if (findUser == null) return NoContent();
            User info = new User(findUser);
            return Ok(info);
        }
        [HttpGet("user/{id}")]
        public IActionResult UserById(int id)
        {
            if (!ModAuthorization()) return Unauthorized();
            var findUser = database.Customer.FirstOrDefault(x => x.Id == id);
            User user = new User(findUser);
            return Ok(user);
        }


        [HttpGet("allUsers")]
        public IActionResult GetAllUsers()
        {
            if (!ModAuthorization()) return Unauthorized();
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
            if (!AdminAuthorization()) return Unauthorized();
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
        //abuse of password Model. .password = new, .username = old
        [HttpPost("editPW")]
        public async Task<IActionResult> EditUserdataAsync([FromBody]Password password)
        {
            string UserName = User.Identity.Name;
            var findUser = database.Customer.Where(a => a.Login == UserName).FirstOrDefault();
            if (findUser.Pw != password.username) return Unauthorized();
            findUser.Pw = password.password;
            database.Customer.Update(findUser);
            await database.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("resetPw")]
        public async Task<IActionResult> ResetPassword([FromBody]int id)
        {
            if (!ModAuthorization()) return Unauthorized();
            Customer UserEntry = database.Customer.FirstOrDefault(x => x.Id == id);
            if (UserEntry == null) return NotFound();

            //create the random new PW
            char[] chars = new char[62];
            chars ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data;
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                data = new byte[10];
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(10);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            UserEntry.Pw = result.ToString();
            await LogAction("reset the password of an user", UserEntry.Id, UserEntry.Login);
            database.Update(UserEntry);
            await database.SaveChangesAsync();
            return  Ok(JsonConvert.SerializeObject(result.ToString())); // stupid but we have to return to know it somehow for now
        }

        [HttpPost("resetAdminPw")]
        public async Task<IActionResult> ResetAdminPassword([FromBody]int id)
        {
            if (!AdminAuthorization()) return Unauthorized();
            Moderator UserEntry = database.Moderator.FirstOrDefault(x => x.Id == id);
            if (UserEntry == null) return NotFound();

            //create the random new PW
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data;
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                data = new byte[10];
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(10);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            string key = result.ToString();

            UserEntry.Pw = result.ToString();
            await LogAction("reset the password of an admin", UserEntry.Id, UserEntry.Login);
            database.Update(UserEntry);
            await database.SaveChangesAsync();
            return Ok(JsonConvert.SerializeObject(result.ToString())); // stupid but we have to return to know it somehow for now
        }

        [HttpPost("editUserAdmin")]
        public async Task<IActionResult> EditForeignUserData(User user)
        {
            if (!ModAuthorization()) return Unauthorized();
            var databaseEntry = database.Customer.FirstOrDefault(x => x.Id == user.Id);
            if (database == null) return NoContent();
            databaseEntry.LastName = user.LastName;
            databaseEntry.FirstName = user.FirstName;
            databaseEntry.Login = user.Login;
            databaseEntry.Address = user.Address;
            database.Customer.Update(databaseEntry);
            await LogAction("edited the data of a user", databaseEntry.Id, databaseEntry.Login);
            await database.SaveChangesAsync();
            return Ok();
        }

        //get List of all movies
        [HttpGet("movies"), AllowAnonymous]
        public List<Movie> Movies() => database.Movie.ToList();

        [HttpGet("movies/{searchParam}/{column}/{pageNumber}/{itemsPerPage}/{sortKey}/{sortOrder}"), AllowAnonymous]
        public IActionResult GetMovies(string searchParam, string column, int pageNumber, int itemsPerPage, string sortKey, string sortOrder)
        {
            if (searchParam == null || column == null || pageNumber == 0 || itemsPerPage == 0 || sortKey == null || sortOrder == null) return BadRequest();

            //sanitizing Inputs
            char[] Sanitizer = searchParam.ToCharArray();
            Sanitizer = Array.FindAll<char>(Sanitizer, c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
            searchParam = new string(Sanitizer);

            Sanitizer = column.ToCharArray();
            Sanitizer = Array.FindAll<char>(Sanitizer, c => char.IsLetterOrDigit(c));
            column = new string(Sanitizer);

            Sanitizer = sortKey.ToCharArray();
            Sanitizer = Array.FindAll<char>(Sanitizer, c => char.IsLetterOrDigit(c));
            sortKey = new string(Sanitizer);

            Sanitizer = sortOrder.ToCharArray();
            Sanitizer = Array.FindAll<char>(Sanitizer, c => char.IsLetterOrDigit(c));
            sortOrder = new string(Sanitizer);

            //alternative to Sanitizing, but apparently slower?
            /*
            Regex sanitizer = new Regex("[A-Za-z0-9]");
            searchParam = sanitizer.Replace(searchParam, "");
            column = sanitizer.Replace(column, "");
            sortKey = sanitizer.Replace(sortKey, "");
            sortOrder = sortOrder.Replace(sortOrder, "");
            */

#pragma warning disable EF1000 // Possible SQL injection vulnerability.
            var result = database.Movie.FromSql("" +
                "SELECT * FROM Movie " +
                "WHERE " + column + " " +
                "LIKE '%" + searchParam + "%' " +
                "ORDER BY " + sortKey + " " + sortOrder)
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
                .ToList();
            int hits = result.Count;
            result = result.Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            MovieListData returnValue = new MovieListData(result, hits);

            return Ok(returnValue);
        }



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
                    await LogAction("added a new Movie", mDetails.Id, mDetails.MovieName);
                    database.Movie.Add(mDetails);
                    await database.SaveChangesAsync();
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
                await LogAction("edited a Movie with", findMovie.Id, findMovie.MovieName);
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
                var findMovie = await database.Movie.FindAsync(id);
                await LogAction("deleted a Movie with", findMovie.Id, findMovie.MovieName);
                database.Movie.Remove(findMovie);
                await database.SaveChangesAsync();
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
                await LogAction("deleted a Customer with", findUser.Id, findUser.Login);
                database.Customer.Remove(findUser);
                await database.SaveChangesAsync();
                return RedirectToAction();
            }
            return Unauthorized();
        }

        //return full mod log, accesible by admin only
        [HttpGet("adminHistory/{id}")]
        public IActionResult ShowModeratorHistory(int id)
        {
            if (!AdminAuthorization()) return Unauthorized();
            return Ok(database.ModeratorHistory.Where(x => x.ModeratorId == id).ToList());
        }

        //edit a mod
        [HttpPut("editAdmin")]
        public async Task<IActionResult> EditModerator(Moderator user)
        {
            string modLogin = User.Identity.Name;
            if (modLogin == null) return Unauthorized();
            if (database.Moderator.Any(x => x.Login == modLogin && x.Rights == 3))
            {          
                var databaseEntry = database.Moderator.FirstOrDefault(x => x.Id == user.Id);
                if (database == null) return NotFound();
                databaseEntry.LastName = user.LastName;
                databaseEntry.FirstName = user.FirstName;
                databaseEntry.Login = user.Login;
                databaseEntry.Address = user.Address;
                databaseEntry.Rights = user.Rights;
                database.Moderator.Update(databaseEntry);
                await database.SaveChangesAsync();
                await LogAction("edited an admin", databaseEntry.Id, databaseEntry.Login);
      
                return NoContent();
            }
            return Unauthorized();
        }

        [HttpPost("createAdmin")]
        public async Task<IActionResult> CreateModerator(Moderator moderator)
        {
            if (!AdminAuthorization()) return Unauthorized();
            database.Moderator.Add(moderator);
            await database.SaveChangesAsync();
            await LogAction("created an admin", moderator.Id, moderator.Login);


            return Ok();
        }

        //delete a mod
        [HttpDelete("deleteAdmin/{id}")]
        public async Task<IActionResult> DeleteModerator(int id)
        {
            string modLogin = User.Identity.Name;
            if (database.Moderator.Any(x => x.Login == modLogin && database.Moderator.Any(y => y.Rights == 3)))
            {
                var findUser = database.Moderator.FirstOrDefault(a => a.Id == id);
                if (findUser == null) return NotFound();
                database.Moderator.Remove(findUser);
                await database.SaveChangesAsync();
                await LogAction("delete an admin", findUser.Id, findUser.Login);
                
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
            if (FindUser == null) return NoContent();
            var MovieEntries = database.CustomerHistory.Where(a => a.CustomerId == FindUser.Id && a.MovieId == id).FirstOrDefault();
            if (MovieEntries == null) return NoContent();
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
            if (UserName == null) return NoContent();
            var findId = database.Customer.FirstOrDefault(x => x.Login == UserName);
            var findHistory = database.CustomerHistory.Where(x => x.IsBorrowing == true || findId.Id == x.CustomerId).ToList();
            if (findHistory == null) return NoContent();
            List<CustomerHistorymask> History = new List<CustomerHistorymask>(findHistory.Count);
            for (int i=0; i<findHistory.Count;i++)
            {
                string MovieName = database.Movie.FirstOrDefault(x => x.Id == findHistory[i].MovieId).MovieName;
                CustomerHistorymask tempHistory = new CustomerHistorymask(findHistory[i].Id, MovieName, findHistory[i].StartDate, findHistory[i].EndDate);
                History.Add(tempHistory);
            }

            return Ok(History);
        }

        [HttpGet("history/{id}")]
        public IActionResult GetCustomerHistoryById(int id)
        {
            if (!ModAuthorization()) return Unauthorized();
            var findHistory = database.CustomerHistory.Where(x => id == x.CustomerId || x.IsBorrowing == true).ToList();
            if (findHistory == null) return NoContent();
            List<CustomerHistorymask> History = new List<CustomerHistorymask>(findHistory.Count);
            for(int i=0;i<findHistory.Count;i++)
            {
                string MovieName = database.Movie.FirstOrDefault(x => x.Id == findHistory[i].MovieId).MovieName;
                CustomerHistorymask tempHistory = new CustomerHistorymask(findHistory[i].Id, MovieName, findHistory[i].StartDate, findHistory[i].EndDate);
                History.Add(tempHistory);
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
            var findCustomer = database.Customer.Where(y => UserName == y.Login).FirstOrDefault();
            if (findCustomer == null) return NoContent();
            PaymentMask findPaymentdata = new PaymentMask();
            var findPaymentList = database.PaymentMethod.Where(y => findCustomer.Id == y.CustomerId).FirstOrDefault();
            if (findPaymentList == null) return NoContent();
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
