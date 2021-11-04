using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Recipe : IAuditedEntity, IEntity
    {
        public Recipe()
        {
            CreatedOn = DateTime.Now;
            ModifiedOn = null;
        }

        [Required] public string Name { get; set; }
        public string Description { get; set; }
        public int? Servings { get; set; }
        public int? Time { get; set; }

        [JsonIgnore] public List<Article> Articles { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public List<Ingredient> Ingredients { get; set; }
        public List<Step> Steps { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string Id { get; set; }
    }
}