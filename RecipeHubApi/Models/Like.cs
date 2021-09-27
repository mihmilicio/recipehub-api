using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Like
    {
        [Key]
        [Column(Order = 1)]
        public string ArticleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string UserId { get; set; }

        [ForeignKey("ArticleId")] [JsonIgnore]
        public Article Article { get; set; }
        [ForeignKey("UserId")] [JsonIgnore]
        public User User { get; set; }
    }
}