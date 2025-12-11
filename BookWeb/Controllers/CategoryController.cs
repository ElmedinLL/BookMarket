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


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError
             ("displayorder","The Display Order cannot match the name");
            }
        
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");

            }
            return View();
        }
    }
}
