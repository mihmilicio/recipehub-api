using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeHubApi.Models
{
    public interface IAuditedEntity
    {
        [Required]
        public DateTime CreatedOn { get; set; }
        
        public DateTime? ModifiedOn { get; set; }
    }
}