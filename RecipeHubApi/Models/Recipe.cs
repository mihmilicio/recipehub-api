using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required] public string Description { get; set; }
        public int Servings { get; set; }
        public int Time { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }
        public ICollection<Step> Steps { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Id { get; set; }
    }
}