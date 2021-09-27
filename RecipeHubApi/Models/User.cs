using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class User : IAuditedEntity, IEntity
    {
        public User()
        {
            CreatedOn = DateTime.Now;
            ModifiedOn = null;
        }

        [Required] public string Name { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }

        public string Image { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string Id { get; set; }
        
        [JsonIgnore] public List<Like> Likes { get; set; }
    }
}