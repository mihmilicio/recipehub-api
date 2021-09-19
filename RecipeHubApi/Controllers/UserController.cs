using RecipeHubApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace RecipeHubApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public User Create(User user)
        {
            return user;
        }
    }
}