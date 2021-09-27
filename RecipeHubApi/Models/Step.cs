using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecipeHubApi.Models
{
    public class Step : IEntity
    {
        [Required] public int Order { get; set; }
        [Required] public string Description { get; set; }

        public string RecipeId { get; set; }

        [ForeignKey("RecipeId")] [JsonIgnore] public virtual Recipe Recipe { get; set; }

        public string Id { get; set; }
    }
}