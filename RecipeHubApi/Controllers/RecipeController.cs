using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        [HttpGet]
        public List<Recipe> GetAll()
        {
            var recipes = _context.Recipe.ToList();
            var mappedRecipes = new List<Recipe>();
            foreach (var recipe in recipes)
            {
                mappedRecipes.Add(MapProperties(recipe));
            }

            return mappedRecipes;
        }

        [HttpGet]
        [Route("{recipeId}")]
        public IActionResult GetById([FromRoute] string recipeId)
        {
            var recipe = _context.Recipe.Find(recipeId);
            return recipe is not null
                ? Ok(MapProperties(recipe))
                : NotFound();
        }

        private Recipe MapProperties(Recipe recipe)
        {
            recipe.Ingredients = _context.Ingredient.Where(ingredient => ingredient.RecipeId == recipe.Id).ToList();
            recipe.Steps = _context.Step.Where(step => step.RecipeId == recipe.Id).ToList();
            return recipe;
        }
    }
}