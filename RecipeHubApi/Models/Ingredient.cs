using System.ComponentModel.DataAnnotations;

namespace RecipeHubApi.Models
{
    public class Ingredient
    {
        [Required] public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
    }
}