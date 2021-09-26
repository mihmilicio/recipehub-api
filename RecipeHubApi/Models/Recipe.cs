using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int Servings { get; set; }
        public int Time { get; set; }

        [ForeignKey("UserId")] public string UserId { get; set; }

        public virtual User User { get; set; }

        public List<Ingredient> Ingredients { get; set; }
        public List<Step> Steps { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string Id { get; set; }
    }
}