using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class ArticleRecipe
    {
        // EF screams at me if this doesnt have a primary key
        public string Id { get; set; }
        public string ArticleId { get; set; }
        [ForeignKey("ArticleId")] [JsonIgnore] public virtual Article Article { get; set; }
        public string RecipeId { get; set; }
        [ForeignKey("RecipeId")] [JsonIgnore] public virtual Recipe Recipe { get; set; }
    }
}