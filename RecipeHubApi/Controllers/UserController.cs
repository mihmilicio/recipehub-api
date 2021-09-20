using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RecipeHubApi.Data;
using RecipeHubApi.Models;

namespace RecipeHubApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context) => _context = context;

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            if (GetById(user.Id) != null)
            {
                return Conflict();
            }

            if (GetByEmail(user.Email) != null)
            {
                return Conflict();
            }

            if (GetByUsername(user.Username) != null)
            {
                return Conflict();
            }

            try
            {
                _context.User.Add(user);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return BadRequest();
            }

            return Created("", user);
        }


        private User? GetById(string id)
        {
            return _context.User.Find(id);
        }

        private User? GetByEmail(string email)
        {
            return _context.User.FirstOrDefault(u => u.Email == email);
        }

        private User? GetByUsername(string username)
        {
            return _context.User.FirstOrDefault(u => u.Username == username);
        }
    }
}