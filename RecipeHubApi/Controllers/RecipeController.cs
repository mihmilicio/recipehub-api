using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var stepOrder = recipe.Steps.Select(s => s.Order).ToList();
            stepOrder.Sort();

            if (stepOrder.Select((value, i) => new { i, value }).Any(item => item.value != item.i))
            {
                return UnprocessableEntity("Steps Order doesn't match");
            }

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
            var recipes = _context.Recipe
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .ToList();

            foreach (var recipe in recipes)
            {
                recipe.Steps = recipe.Steps.OrderBy(i => i.Order).ToList();
            }

            return recipes;
        }

        [HttpGet]
        [Route("user/{userId}")]
        public List<Recipe> GetByUser([FromRoute] string userId)
        {
            var recipes = _context.Recipe
                .Where(r => r.UserId == userId)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .ToList();
            foreach (var recipe in recipes)
            {
                recipe.Steps = recipe.Steps.OrderBy(i => i.Order).ToList();
            }

            return recipes;
        }

        private Recipe GetById(string id)
        {
            return _context.Recipe
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .FirstOrDefault(r => r.Id == id);
        }

        private Ingredient GetIngredientById(string id)
        {
            return _context.Ingredient.Find(id);
        }

        private Step GetStepById(string id)
        {
            return _context.Step.Find(id);
        }


        [HttpGet]
        [Route("{recipeId}")]
        public IActionResult GetByIdRequest([FromRoute] string recipeId)
        {
            var recipe = GetById(recipeId);
            return recipe is not null ? Ok(recipe) : NotFound();
        }

        [HttpDelete]
        [Route("{recipeId}")]
        public IActionResult Delete([FromRoute] string recipeId)
        {
            var recipe = _context.Recipe.Find(recipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            try
            {
                var ingredients = _context.Ingredient.Where(ingredient => ingredient.RecipeId == recipe.Id);
                foreach (var ingredient in ingredients)
                {
                    _context.Ingredient.Remove(ingredient);
                }

                var steps = _context.Step.Where(step => step.RecipeId == recipe.Id);
                foreach (var step in steps)
                {
                    _context.Step.Remove(step);
                }

                _context.Recipe.Remove(recipe);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return BadRequest();
            }

            return NoContent();
        }

        private void SetStates(Recipe recipe, EntityState state)
        {
            _context.Entry(recipe).State = state;
            foreach (var ingredient in recipe.Ingredients)
            {
                _context.Entry(ingredient).State = state;
            }

            foreach (var step in recipe.Steps)
            {
                _context.Entry(step).State = state;
            }
        }

        // Apenas para debugging
        [HttpGet]
        [Route("ingredients")]
        public IActionResult GetAllIngredients()
        {
            return Ok(_context.Ingredient.ToList());
        }

        // Apenas para debugging
        [HttpGet]
        [Route("steps")]
        public IActionResult GetAllSteps()
        {
            return Ok(_context.Step.ToList());
        }

        [HttpPut]
        [Route("{recipeId}")]
        public IActionResult Update([FromBody] Recipe recipe, [FromRoute] string recipeId)
        {
            if (recipeId != recipe.Id || recipeId is null or "")
            {
                return UnprocessableEntity();
            }

            var originalRecipe = GetById(recipeId);
            if (originalRecipe == null)
            {
                return NotFound();
            }

            if (recipe.UserId == null || recipe.UserId != originalRecipe.UserId)
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

            var stepOrder = recipe.Steps.Select(s => s.Order).ToList();
            stepOrder.Sort();

            if (stepOrder.Select((value, i) => new { i, value }).Any(item => item.value != item.i))
            {
                return UnprocessableEntity("Steps Order doesn't match");
            }

            SetStates(originalRecipe, EntityState.Detached);

            var originalIngredients = originalRecipe.Ingredients.Select(i => i.Id).ToList();
            var newIngredients = recipe.Ingredients.Select(i => i.Id).ToList();

            var ingredientsToDelete = originalIngredients.Except(newIngredients).ToList();
            var ingredientsToAdd = newIngredients.Except(originalIngredients).ToList();

            ingredientsToDelete.ForEach(i => _context.Entry(GetIngredientById(i)).State = EntityState.Deleted);
            ingredientsToAdd.ForEach(i =>
            {
                var match = recipe.Ingredients.Find(x => x.Id == i);
                if (match != null)
                    _context.Entry(match).State = EntityState.Added;
            });


            var originalSteps = originalRecipe.Steps.Select(i => i.Id).ToList();
            var newSteps = recipe.Steps.Select(i => i.Id).ToList();

            var stepsToDelete = originalSteps.Except(newSteps).ToList();
            var stepsToAdd = newSteps.Except(originalSteps).ToList();

            stepsToDelete.ForEach(s => _context.Entry(GetStepById(s)).State = EntityState.Deleted);
            stepsToAdd.ForEach(s =>
            {
                var match = recipe.Steps.Find(x => x.Id == s);
                if (match != null)
                    _context.Entry(match).State = EntityState.Added;
            });

            try
            {
                recipe.CreatedOn = originalRecipe.CreatedOn;
                recipe.ModifiedOn = DateTime.Now;
                recipe.User = _context.User.Find(recipe.UserId);
                _context.Recipe.Update(recipe);
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

            return Ok(recipe);
        }
    }
}