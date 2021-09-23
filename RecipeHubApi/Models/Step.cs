using System.ComponentModel.DataAnnotations;

namespace RecipeHubApi.Models
{
    public class Step
    {
        [Required] public int Order { get; set; }
        [Required] public string Description { get; set; }
    }
}