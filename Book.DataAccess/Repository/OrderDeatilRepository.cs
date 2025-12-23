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
    public class OrderDeatilRepository : Repository<OrderDetail>, IOrderDetailRepository

    {
        private ApplicationDBContext _db;

        public OrderDeatilRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }

    }
}
