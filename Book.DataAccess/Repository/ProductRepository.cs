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
    public class ProductRepository :
    Repository<Product>, IProductRepository
    {

        private readonly ApplicationDBContext _db;
        public ProductRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }


        public void Update(Product obj)
        {
          var objFromDB = _db.Products.FirstOrDefault(u=> u.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Price = obj.Price;
                objFromDB.Price50 = obj.Price50;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price100 = obj.Price100;
                objFromDB.Description = obj.Description;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.Author= obj.Author;
                objFromDB.ImageUrl = obj.ImageUrl;
                if (obj.ImageUrl != null)
                {
                    objFromDB.ImageUrl = obj.ImageUrl;
                }

            }
        }

    } 
}
