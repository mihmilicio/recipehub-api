using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeHubApi.Data;
using RecipeHubApi.Models;
using RecipeHubApi.Utils;

namespace RecipeHubApi.Controllers
{
    [ApiController]
    [Route("api/recipe")]
    [Produces("application/json")]
    public class RecipeController : ControllerBase
    {
        private readonly DataContext _context;
        public RecipeController(DataContext context) => _context = context;

        [HttpPost]
        public IActionResult Create([FromBody] Recipe recipe)
        {
            if (recipe.Id == "")
            {
                return UnprocessableEntity();
            }

            if (recipe.UserId == null || UserUtils.GetById(recipe.UserId, _context) == null)
            {
                return Unauthorized();
            }

            if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
            {
                return UnprocessableEntity();
            }

            if (recipe.Steps == null || recipe.Steps.Count == 0)
            {
                return UnprocessableEntity();
            }

            // TODO delete all ingredients and steps before adding new ones (transaction with add)
            // TODO check order of steps

            try
            {
                _context.Recipe.Add(recipe);
                _context.SaveChanges();
            }
            catch (ArgumentException e)
            {
                // Capta IDs repetidos
                Debug.WriteLine(e);
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return BadRequest(e.Message);
            }

            return Created("", recipe);
        }

        private Recipe GetById(string id)
        {
            return _context.Recipe.Find(id);
        }
    }
}