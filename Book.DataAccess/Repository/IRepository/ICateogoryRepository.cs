using BookMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.Repository.IRepository
{
    public interface ICateogoryRepository : IRepository<Category>
    {
        void Update(Category obj);
     
    }
}
