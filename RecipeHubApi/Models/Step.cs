using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Step : IEntity
    {
        [Required] public int Order { get; set; }
        [Required] public string Description { get; set; }

        public string RecipeId { get; set; }

        [JsonIgnore] public Recipe Recipe { get; set; }

        public string Id { get; set; }
    }
}