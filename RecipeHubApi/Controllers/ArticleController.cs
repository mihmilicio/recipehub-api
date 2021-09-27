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
        
        [HttpGet]
        public List<Article> GetAll()
        {
            return _context.Article
                .Include(a => a.User)
                .ToList();
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