using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RecipeHubApi.Data;
using RecipeHubApi.Models;
using RecipeHubApi.Models.DTO;

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


        private User GetById(string id)
        {
            return _context.User.Find(id);
        }

        private User GetByEmail(string email)
        {
            return _context.User.FirstOrDefault(u => u.Email == email);
        }

        private User GetByUsername(string username)
        {
            return _context.User.FirstOrDefault(u => u.Username == username);
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var user = GetByUsername(login.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Password != login.Password)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpPut]
        [Route("{userId}")]
        public IActionResult Update([FromBody] User user, [FromRoute] string userId)
        {
            if (userId != user.Id || userId is null or "")
            {
                return BadRequest();
            }

            var originalUser = GetById(userId);
            if (originalUser == null)
            {
                return NotFound();
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
                _context.User.Update(user);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return BadRequest();
            }

            return Ok(user);
        }

        [HttpDelete]
        [Route("{userId}")]
        public IActionResult Delete([FromRoute] string userId)
        {
            var user = GetById(userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return BadRequest();
            }

            return NoContent();
        }
    }
}