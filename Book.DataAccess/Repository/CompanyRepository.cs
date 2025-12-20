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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {

        private readonly ApplicationDBContext _db;

        public CompanyRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
           _db.Companies.Update(company);
        }
    }
    }

