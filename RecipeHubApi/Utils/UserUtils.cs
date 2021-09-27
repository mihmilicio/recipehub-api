using RecipeHubApi.Data;
using RecipeHubApi.Models;

namespace RecipeHubApi.Utils
{
    public class UserUtils
    {
        public static User GetById(string id, DataContext _context)
        {
            return _context.User.Find(id);
        }
    }
}