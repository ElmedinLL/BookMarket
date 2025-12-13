using Book.DataAccess.Repository.IRepository;
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
    public class CategoryRepository : 
    Repository<Category>, ICateogoryRepository
    {

        private readonly ApplicationDBContext _db;
        public CategoryRepository(ApplicationDBContext db) 
            : base(db)
        {
            _db = db;
        }


        public void Update(Category obj)
        {
           _db.Categories.Update(obj);
        }

    }
}
