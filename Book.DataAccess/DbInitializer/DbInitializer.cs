using Book.Models;
using Book.Utility;
using BookMarket.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly ApplicationDBContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDBContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {

            //migrate if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }catch (Exception ex)
            {
            }

            // create roles if they are not created

            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();


                // if the roles are not created, then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin1@bookmarket.com",
                    Email = "admin1@bookmarket.com",
                    Name = "Elmedin Llumnica",
                    PhoneNumber = "111-222-3333",
                    StreetAddress = "123 Main St",
                    City = "London",
                    State = "UK",
                    PostalCode = "LS1 2AB"

                }, "Admin1234*").GetAwaiter().GetResult();

                //  assign admin user to admin role

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin1@bookmarket.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }

           return;
        }
    }
}
