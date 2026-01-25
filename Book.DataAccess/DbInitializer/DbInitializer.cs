using Book.Models;
using Book.Utility;
using BookMarket.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Book.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDBContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Replace with ILogger when available
                Console.WriteLine($"Database migration failed: {ex.Message}");
            }

            // Create roles and default admin user if roles are not present
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                var adminUser = new ApplicationUser
                {
                    UserName = "admin1@bookmarket.com",
                    Email = "admin1@bookmarket.com",
                    Name = "Elmedin Llumnica",
                    PhoneNumber = "111-222-3333",
                    StreetAddress = "123 Main St",
                    City = "London",
                    State = "UK",
                    PostalCode = "LS1 2AB"
                };

                _userManager.CreateAsync(adminUser, "Admin1234*").GetAwaiter().GetResult();

                var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin1@bookmarket.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
            }

            return;
        }
    }
}