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
    [Route("api/article/{articleId}/comment")]
    [Produces("application/json")]
    public class CommentController : ControllerBase
    {
        private readonly DataContext _context;
        public CommentController(DataContext context) => _context = context;
        
        [HttpPost]
        public IActionResult Create([FromRoute] string articleId, [FromBody] Comment comment)
        {
            if (comment.Id is null or "")
            {
                return UnprocessableEntity();
            }
            
            if (comment.ArticleId is null or "" || comment.ArticleId != articleId)
            {
                return UnprocessableEntity();
            }

            if (comment.UserId == null || UserUtils.GetById(comment.UserId, _context) == null)
            {
                return Unauthorized();
            }

            if (comment.Body is null or "")
            {
                return UnprocessableEntity();
            }
            
            try
            {
                _context.Comment.Add(comment);
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

            return Created("", comment);
        }
        
        [HttpPut]
        [Route("{commentId}")]
        public IActionResult Update([FromBody] Comment comment, [FromRoute] string commentId, [FromRoute] string articleId)
        {
            if (commentId is null or "" || commentId != comment.Id )
            {
                return UnprocessableEntity();
            }

            var originalComment = GetById(commentId);
            if (originalComment == null)
            {
                return NotFound();
            }

            if (comment.UserId == null || comment.UserId != originalComment.UserId)
            {
                return Unauthorized();
            }
            
            if (comment.ArticleId == null || comment.ArticleId != originalComment.ArticleId)
            {
                return UnprocessableEntity();
            }
            
            _context.Entry(originalComment).State = EntityState.Detached;
            
            try
            {
                comment.CreatedOn = originalComment.CreatedOn;
                comment.ModifiedOn = DateTime.Now;
                _context.Comment.Update(comment);
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

            return Ok(comment);
        }
        
        [HttpGet]
        public List<Comment> GetAll([FromRoute] string articleId)
        {
            return _context.Comment
                .Where(c => c.ArticleId == articleId)
                .Include(c => c.User)
                .ToList();
        }
        
        private Comment GetById(string commentId)
        {
            return _context.Comment
                .Include(a => a.User)
                .FirstOrDefault(c => c.Id == commentId);
        }
        
        [HttpGet]
        [Route("{commentId}")]
        public IActionResult GetByIdRequest([FromRoute] string commentId)
        {
            var comment = GetById(commentId);
            return comment is not null ? Ok(comment) : NotFound();
        }
        
        [HttpDelete]
        [Route("{commentId}")]
        public IActionResult Delete([FromRoute] string commentId)
        {
            var comment = _context.Comment.Find(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            try
            {
                _context.Comment.Remove(comment);
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