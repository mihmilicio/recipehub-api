using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeHubApi.Models
{
    public class User : IAuditedEntity, IEntity
    {
        public User()
        {
             CreatedOn = DateTime.Now;
             ModifiedOn = null;
        }

        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public string? Image { get; set; }
    }
}