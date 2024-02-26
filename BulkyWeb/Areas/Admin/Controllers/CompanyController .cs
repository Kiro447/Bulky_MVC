using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = SD.Role_Company)]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnviroment;
    public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnviroment = webHostEnviroment;
    }
    public IActionResult Index()
    {
        List<Company> companies = _unitOfWork.Company.GetAll().ToList();

        return View(companies);
    }

    public IActionResult Upsert(int? id)
    {
        if (id is null || id == 0)
        {
            return View(new Company());
        }
        else
        {
            Company companyObj = _unitOfWork.Company.Get(x => x.Id == id);
            return View(companyObj);
        };
    }
    [HttpPost]
    public IActionResult Upsert(Company obj)
    {

        if (ModelState.IsValid)
        {
            if (obj.Id == 0)
            {
                _unitOfWork.Company.Add(obj);

            }
            else
            {
                _unitOfWork.Company.Update(obj);
            }
            _unitOfWork.Save();
            TempData["success"] = "Company created";
            return RedirectToAction("Index");
        }
        else
        {
            return View(obj);
        }
    }

    //public IActionResult Edit(int? id)
    //{
    //    if (id is null || id == 0)
    //    {
    //        return NotFound();
    //    }
    //    Product product = _unitOfWork.Product.Get(u => u.Id == id);
    //    if (product == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(product);
    //}
    //[HttpPost]
    //public IActionResult Edit(Product product)
    //{

    //    if (ModelState.IsValid)
    //    {
    //        _unitOfWork.Product.Update(product);
    //        _unitOfWork.Save();
    //        TempData["success"] = "product updated";

    //        return RedirectToAction("Index");
    //    }
    //    return View();
    //}
    //public IActionResult Delete(int? id)
    //{
    //    if (id is null || id == 0)
    //    {
    //        return NotFound();
    //    }
    //    Product product = _unitOfWork.Product.Get(u => u.Id == id);
    //    if (product == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(product);
    //}
    //[HttpPost, ActionName("Delete")]
    //public IActionResult DeletePOST(int? id)
    //{
    //    Product product = _unitOfWork.Product.Get(u => u.Id == id);
    //    if (product is null)
    //    {
    //        return NotFound();
    //    }
    //    _unitOfWork.Product.Remove(product);
    //    _unitOfWork.Save();
    //    TempData["success"] = "Category deleted";
    //    return RedirectToAction("Index");
    //}

    #region
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> companies = _unitOfWork.Company.GetAll().ToList();
        return Json(new { data = companies });
    }
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var companyToBeDeleted = _unitOfWork.Company.Get(company => company.Id == id);
        if (companyToBeDeleted is null)
        {
            return Json(new { success = false, message = "Error While Deleting" });
        }

        _unitOfWork.Company.Remove(companyToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Deleted" });

    }
    #endregion

}