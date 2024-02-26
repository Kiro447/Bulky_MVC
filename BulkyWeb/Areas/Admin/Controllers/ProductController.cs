using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnviroment;
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnviroment = webHostEnviroment;
    }
    public IActionResult Index()
    {
        List<Product> categories = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

        return View(categories);
    }

    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new ProductVM()
        {

            CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),

            Product = new Product()
        };
        if (id is null || id == 0) 
        {
            return View(productVM);
        }
        else
        {
            productVM.Product = _unitOfWork.Product.Get(x => x.Id == id);
            return View(productVM);
        };
    }
    [HttpPost]
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnviroment.WebRootPath;
            if (file is not null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");
                if(!string.IsNullOrWhiteSpace(obj.Product.ImageUrl))
                {
                    //delete
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.Trim('\\'));

                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);

                }
                obj.Product.ImageUrl = @"\images\product\" + fileName;
            }
            if(obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);

            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);
            }
            _unitOfWork.Save();
            TempData["success"] = "Product created";
            return RedirectToAction("Index");
        }
        else
        {
            obj.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
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
        List<Product> categories = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        return Json(new { data = categories });
    }
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var prodToBeDeletd = _unitOfWork.Product.Get(product => product.Id == id);
        if(prodToBeDeletd is null)
        {
            return Json(new { success = false, message = "Error While Deleting" });
        }
        var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath,
                                        prodToBeDeletd.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Product.Remove(prodToBeDeletd);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Deleted" });

    }
    #endregion

}