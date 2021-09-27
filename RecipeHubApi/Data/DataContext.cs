using Microsoft.EntityFrameworkCore;
using RecipeHubApi.Models;

namespace RecipeHubApi.Data
{
    public class DataContext : DbContext
    {
        // Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        // Lista de propriedades das classes que vão virar tabelas no banco
        public DbSet<User> User { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Step> Step { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<ArticleRecipe> ArticleRecipe { get; set; }

    }
}