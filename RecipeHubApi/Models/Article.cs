using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Article : IAuditedEntity, IEntity
    {
        public Article()
        {
            CreatedOn = DateTime.Now;
            ModifiedOn = null;
        }

        [Required] public string Name { get; set; }
        public string Description { get; set; }
        public List<ArticleRecipe> ArticleRecipes { get; set; }
        public List<Recipe> Recipes { get; set; }
        [ForeignKey("UserId")] public string UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Id { get; set; }
        
        [JsonIgnore] public List<Like> Likes { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
        
        [JsonIgnore] public List<Comment> Comments { get; set; }
        public int CommentCount { get; set; }


    }
}