using Microsoft.EntityFrameworkCore;

namespace BookWeb.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext
            (DbContextOptions<ApplicationDBContext> options) 
            : base(options)
        {
        }
    }
}
