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

            if (article.Recipes is null || article.Recipes.Count == 0)
            {
                return UnprocessableEntity();
            }

            try
            {
                foreach (var recipe in article.Recipes)
                {
                    _context.Entry(recipe).State = EntityState.Modified;
                }
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

            var originalArticle = GetById(articleId, null);
            if (originalArticle == null)
            {
                return NotFound();
            }

            if (article.UserId == null || article.UserId != originalArticle.UserId)
            {
                return Unauthorized();
            }

            if (article.Recipes is null || article.Recipes.Count == 0)
            {
                return UnprocessableEntity();
            }

            SetStates(originalArticle, EntityState.Detached);

            var originalArticleRecipes = originalArticle.Recipes.ToList();
            var newArticleRecipes = article.Recipes.ToList();

            var articleRecipesToDelete = originalArticleRecipes.Except(newArticleRecipes).ToList();
            var articleRecipesToAdd = newArticleRecipes.Except(originalArticleRecipes).ToList();

            articleRecipesToDelete.ForEach(articleRecipeId =>
                _context.Entry(_context.Recipe.Find(articleRecipeId)).State = EntityState.Deleted);
            articleRecipesToAdd.ForEach(articleRecipe =>
            {
                var match = article.Recipes.Find(x => x == articleRecipe);
                if (match != null)
                    _context.Entry(match).State = EntityState.Added;
            });

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

        private void SetStates(Article article, EntityState state)
        {
            _context.Entry(article).State = state;
            foreach (var articleRecipe in article.Recipes)
            {
                _context.Entry(articleRecipe).State = state;
            }
        }

        [HttpGet]
        public List<Article> GetAll([FromQuery] string userId)
        {
            var articles = _context.Article
                .Include(a => a.User)
                .Include(a => a.Recipes)
                .ToList();

            return articles.Select(article => IncludeInfo(article, userId)).ToList();
        }

        [HttpGet]
        [Route("user/{userId}")]
        public List<Article> GetByUser([FromRoute] string userId)
        {
            var articles = _context.Article
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .Include(a => a.Recipes)
                .ToList();

            return articles.Select(article => IncludeInfo(article, userId)).ToList();
        }

        private Article GetById(string articleId, string userId)
        {
            var article = _context.Article
                .Include(a => a.User)
                .Include(a => a.Recipes)
                .FirstOrDefault(a => a.Id == articleId);
            return article is not null ? IncludeInfo(article, userId) : null;
        }

        [HttpGet]
        [Route("{articleId}")]
        public IActionResult GetByIdRequest([FromRoute] string articleId, [FromQuery] string userId)
        {
            var article = GetById(articleId, userId);
            return article is not null ? Ok(article) : NotFound();
        }

        private Article IncludeInfo(Article article, string userId)
        {
            // article.Recipes ??= new List<Recipe>();
            foreach (var articleRecipe in article.Recipes)
            {
                var recipe = _context.Recipe.Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .FirstOrDefault(r => r.Id == articleRecipe.Id);
                if (recipe is not null)
                {
                    // article.Recipes.Add(recipe);
                }
            }

            article.LikeCount = _context.Like.Count(l => l.ArticleId == article.Id);

            if (userId is not null)
            {
                article.IsLiked =
                    _context.Like.FirstOrDefault(l => l.ArticleId == article.Id && l.UserId == userId) is not null;
            }

            article.CommentCount = _context.Comment.Count(c => c.ArticleId == article.Id);

            return article;
        }

        [HttpDelete]
        [Route("{articleId}")]
        public IActionResult Delete([FromRoute] string articleId)
        {
            var article = _context.Article
                .Include(a => a.Recipes)
                .FirstOrDefault(a => a.Id == articleId);

            if (article == null)
            {
                return NotFound();
            }

            try
            {
                foreach (var articleRecipe in article.Recipes)
                {
                    _context.Recipe.Remove(articleRecipe);
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

        [HttpPost]
        [Route("{articleId}/like")]
        public IActionResult Like([FromRoute] string articleId, [FromQuery] string userId)
        {
            if (articleId is null || userId is null)
            {
                return BadRequest();
            }

            var like = new Like
            {
                ArticleId = articleId,
                UserId = userId
            };

            try
            {
                _context.Like.Add(like);
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

            return Created("", like);
        }

        [HttpDelete]
        [Route("{articleId}/like")]
        public IActionResult DeleteLike([FromRoute] string articleId, [FromQuery] string userId)
        {
            if (articleId is null || userId is null)
            {
                return BadRequest();
            }

            var like = _context.Like.FirstOrDefault(l => l.ArticleId == articleId && l.UserId == userId);

            if (like == null)
            {
                return NotFound();
            }

            try
            {
                _context.Like.Remove(like);
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