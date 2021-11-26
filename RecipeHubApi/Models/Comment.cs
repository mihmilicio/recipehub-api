using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Comment : IEntity, IAuditedEntity
    {
        public Comment()
        {
            CreatedOn = DateTime.Now;
            ModifiedOn = null;
        }

        public string ArticleId { get; set; }

        [JsonIgnore] public Article Article { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        [Required] public string Body { get; set; }

        [Required] public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string Id { get; set; }
    }
}