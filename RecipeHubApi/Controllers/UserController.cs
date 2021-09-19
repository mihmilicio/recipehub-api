using System;
using System.Diagnostics;
using RecipeHubApi.Models;
using Microsoft.AspNetCore.Mvc;
using RecipeHubApi.Data;

namespace RecipeHubApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context) => _context = context;
        
        // TODO como customizar erro de parse do body
        
        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            // TODO proibir email e username repetido (implementar getByEmail, getByUsername)
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
    }
}