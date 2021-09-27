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
    [Route("api/article")]
    [Produces("application/json")]
    public class ArticleController : ControllerBase
    {
        private readonly DataContext _context;
        public ArticleController(DataContext context) => _context = context;
        
        [HttpPost]
        public IActionResult Create([FromBody] Article article)
        {
            if (article.Id == "")
            {
                return UnprocessableEntity();
            }

            if (article.UserId == null || UserUtils.GetById(article.UserId, _context) == null)
            {
                return Unauthorized();
            }

            if (article.ArticleRecipes is null || article.ArticleRecipes.Count == 0)
            {
                return UnprocessableEntity();
            }
            
            try
            {
                _context.Article.Add(article);
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

            return Created("", article);
        }
        
        [HttpPut]
        [Route("{articleId}")]
        public IActionResult Update([FromBody] Article article, [FromRoute] string articleId)
        {
            if (articleId != article.Id || articleId is null or "")
            {
                return UnprocessableEntity();
            }

            var originalArticle = _context.Article.Find(articleId);
            if (originalArticle == null)
            {
                return NotFound();
            }

            if (article.UserId == null || article.UserId != originalArticle.UserId)
            {
                return Unauthorized();
            }
            
            _context.Entry(originalArticle).State = EntityState.Detached;
            
            try
            {
                article.CreatedOn = originalArticle.CreatedOn;
                article.ModifiedOn = DateTime.Now;
                article.User = _context.User.Find(article.UserId);
                _context.Article.Update(article);
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

            return Ok(article);
        }
        
        [HttpGet]
        public List<Article> GetAll()
        {
            var articles = _context.Article
                .Include(a => a.User)
                .Include(a => a.ArticleRecipes)
                .ToList();

            return articles.Select(IncludeRecipes).ToList();
        }
        
        [HttpGet]
        [Route("{articleId}")]
        public Article GetById([FromRoute] string articleId)
        {
            var article = _context.Article
                .Include(a => a.User)
                .Include(a => a.ArticleRecipes)
                .FirstOrDefault(a => a.Id == articleId);
            return IncludeRecipes(article);
        }

        private Article IncludeRecipes(Article article)
        {
            article.Recipes ??= new List<Recipe>();
            foreach (var articleRecipe in article.ArticleRecipes)
            {
                var recipe = _context.Recipe.Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .FirstOrDefault(r => r.Id == articleRecipe.RecipeId);
                if (recipe is not null)
                {
                    article.Recipes.Add(recipe);
                }
            }
        
            return article;
        }
        
        [HttpDelete]
        [Route("{articleId}")]
        public IActionResult Delete([FromRoute] string articleId)
        {
            var article = _context.Article.Find(articleId);

            if (article == null)
            {
                return NotFound();
            }

            try
            {
                foreach (var articleRecipe in article.ArticleRecipes)
                {
                    _context.ArticleRecipe.Remove(articleRecipe);
                }
                _context.Article.Remove(article);
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