using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        
        public string Id { get; set; }
        public string ArticleId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("ArticleId")] [JsonIgnore]
        public Article Article { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        
        [Required]
        public string Body { get; set; }
    }
}