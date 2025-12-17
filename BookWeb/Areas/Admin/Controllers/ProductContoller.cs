using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Models.ViewModels;
using BookMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;

namespace BookWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)

        {
           this.webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Product> objProductList = 
                _unitOfWork.Product.GetAll().ToList();


            return View(objProductList);
        }

        public IActionResult Upsert(int ? id) //Up-Update sert-Insert
        {
            ProductVM productVM = new()
            {
             CategoryList = _unitOfWork.Category.GetAll().
             Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create product
                return View(productVM);
            }
            else
            {
                //update product
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
              
       

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM , IFormFile? file)
        {
          

            if (ModelState.IsValid)
            {
                    string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() +Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // check if we have a image - if not its a new image and we gonna skip the if and go to create
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) 
                    {

                       //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }


                    using (var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);   
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == null)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                   
                _unitOfWork.Save();
                TempData["success"] = "Product created successfuly";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().
              Select(u => new SelectListItem
              {
                  Text = u.Name,
                  Value = u.Id.ToString()
              });
                return View(productVM);
                
                };
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
