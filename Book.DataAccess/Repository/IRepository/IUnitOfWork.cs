using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICateogoryRepository Category { get; }
        IProductRepository Product{ get; }
        IShoppingCartRepository ShoppingCart { get; }
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }

        void Save();  


    }
}
