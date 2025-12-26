using Book.DataAccess.Repository.IRepository;
using Book.Models;
using Book.Models.ViewModels;
using Book.Utility;
using BookMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;

namespace BookWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = (SD.Role_Admin))]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)

        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Company> objCompanyList =
                _unitOfWork.Company.GetAll().ToList();


            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id) //Up-Update sert-Insert
        {

            if (id == null || id == 0)
            {
                //create Company
                return View(new Company());
            }

            else
            {    //update Company
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);

               
                if (companyObj == null)
                {
                    // Kthe në faqen kryesore ose trego error
                    return NotFound(); // Kthen 404
                                       // OSE: return RedirectToAction("Index");
                                       // OSE: return View("Error");
                }
                // No company found with this ID
                return View(companyObj);
            }

            

        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
               
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }


                _unitOfWork.Save();
                TempData["success"] = "Company created successfuly";
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(CompanyObj);

            };
 
        }




        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList =
                _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //delete the image from wwwroot
   

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}

