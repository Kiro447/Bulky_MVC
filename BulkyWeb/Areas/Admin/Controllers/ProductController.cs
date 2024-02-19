using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers;

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
        List<Product> categories = _unitOfWork.Product.GetAll().ToList();

        return View(categories);
    }

    public IActionResult Create()
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


        return View(productVM);
    }
    [HttpPost]
    public IActionResult Create(ProductVM obj)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Add(obj.Product);
            _unitOfWork.Save();
            TempData["success"] = "Product created";
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id is null || id == 0)
        {
            return NotFound();
        }
        Product product = _unitOfWork.Product.Get(u => u.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    [HttpPost]
    public IActionResult Edit(Product product)
    {

        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();
            TempData["success"] = "product updated";

            return RedirectToAction("Index");
        }
        return View();
    }
    public IActionResult Delete(int? id)
    {
        if (id is null || id == 0)
        {
            return NotFound();
        }
        Product product = _unitOfWork.Product.Get(u => u.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        Product product = _unitOfWork.Product.Get(u => u.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        TempData["success"] = "Category deleted";
        return RedirectToAction("Index");
    }

}