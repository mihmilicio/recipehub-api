using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("ArticleId")]
        public Article Article { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}