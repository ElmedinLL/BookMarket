using Microsoft.AspNetCore.Mvc;
using BookWeb.Models;
using BookWeb.Data;

namespace BookWeb.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDBContext _db;

        public CategoryController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
       List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }
    }
}
