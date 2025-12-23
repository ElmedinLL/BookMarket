using Book.DataAccess.Repository.IRepository;
using Book.Models;
using BookMarket.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDBContext _db;
        public OrderHeaderRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
      public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader;
        }
    }
}
