using Book.DataAccess.Repository.IRepository;
using Book.Models;
using BookMarket.DataAccess.Data;
using BookMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.Repository
{
    public class ApplicationUserRepository : 
    Repository<ApplicationUser>, IApplicationUserRepository
    {

        private readonly ApplicationDBContext _db;
        public ApplicationUserRepository(ApplicationDBContext db) 
            : base(db)
        {
            _db = db;
        }


 

    }
}
