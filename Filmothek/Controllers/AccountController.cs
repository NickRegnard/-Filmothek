using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Filmothek.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filmothek.Controllers
{
    //edited
    [Route("home")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly VideoContext _context;
        public AccountController(VideoContext context)
        {
            _context = context;
            var dbcustomer = _context.Customer.Find();
        }
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
          //  return ;
        //}

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string username, string password)
        {

            var dbcustomer = _context.Customer.Find(username);
            if (username == dbcustomer.Login)
            {
                if (password == dbcustomer.Pw)
                {
                    return NoContent();
                }
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
