using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("UserId")] public string UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Id { get; set; }
    }
}