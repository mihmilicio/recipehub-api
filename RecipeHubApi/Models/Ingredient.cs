using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Ingredient : IEntity
    {
        [Required] public string Name { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }

        public string RecipeId { get; set; }

        [JsonIgnore] public Recipe Recipe { get; set; }

        public string Id { get; set; }
    }
}