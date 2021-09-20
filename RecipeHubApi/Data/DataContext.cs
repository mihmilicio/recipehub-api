using RecipeHubApi.Models;
using Microsoft.EntityFrameworkCore;

namespace RecipeHubApi.Data
{
    public class DataContext : DbContext
    {
        // Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Lista de propriedades das classes que vão virar tabelas no banco
        public DbSet<User> User { get; set; }
    }
}