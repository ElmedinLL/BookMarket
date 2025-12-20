using Book.DataAccess.Repository.IRepository;
using Book.Models;
using BookMarket.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {

        private readonly ApplicationDBContext _db;
        public ShoppingCartRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }

    }
}
