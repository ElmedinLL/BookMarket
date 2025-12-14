using Book.DataAccess.Repository.IRepository;
using Book.Models;
using BookMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;

namespace BookWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Product> objProductList = 
                _unitOfWork.Product.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(Product obj)
        {
            if (obj.Title == obj.Description)
            {
                ModelState.AddModelError("Description", "The Description cannot match the name");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfuly";
                return RedirectToAction("Index" , "Product");
            }
            return View();
        }

        public IActionResult Edit(int ? id)
        {
            if (id is null || id <= 0)
            {
                return NotFound();
            }

            var productFromDB =
            _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDB == null)
            {
                return NotFound();
            }
            return View(productFromDB);

        }

        [HttpPost]
        
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfuly";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Delete(int ? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            var productFromDB =
            _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDB == null)
            {
                return NotFound(); 
            }
            return View(productFromDB);


        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int ? id)
        {
            
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var obj =
            _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Remove(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product deleted successfuly";
                return RedirectToAction("index");

            }
            return View();

        }




    }
}
