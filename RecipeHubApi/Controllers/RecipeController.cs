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
            return _context.Recipe
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .ToList();
        }
        
        [HttpGet]
        [Route("user/{userId}")]
        public List<Recipe> GetByUser([FromRoute] string userId)
        {
            return _context.Recipe
                .Where(r =>  r.UserId == userId)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .ToList();
        }

        [HttpGet]
        [Route("{recipeId}")]
        public IActionResult GetById([FromRoute] string recipeId)
        {
            var recipe = _context.Recipe
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .First(r => r.Id == recipeId);
            return recipe is not null ? Ok(recipe) : NotFound();
        }
        
        [HttpDelete]
        [Route("{recipeId}")]
        public IActionResult Delete([FromRoute] string recipeId)
        {
            var recipe = _context.Recipe.Find(recipeId);;
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
    }
}