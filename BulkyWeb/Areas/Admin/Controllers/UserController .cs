using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = SD.Role_Company)]
[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _webHostEnviroment;
    public UserController(IWebHostEnvironment webHostEnviroment, ApplicationDbContext db)
    {
        _webHostEnviroment = webHostEnviroment;
        _db = db;
    }
    public IActionResult Index()
    {
        return View();
    }

    #region
    [HttpGet]
    public IActionResult GetAll()
    {
        List<ApplicationUser> applicationUsers = _db.ApplicationUsers.Include(u => u.Company).ToList();

        var userRoles = _db.UserRoles.ToList();
        var roles = _db.Roles.ToList();

        foreach (var user in applicationUsers)
        {

            var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            if (user.Company is null)
            {
                user.Company = new() { Name = "" };
            }
        }


        return Json(new { data = applicationUsers });
    }
    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
    {
        var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
        if (objFromDb == null)
        {
            return Json(new { success = true, message = "Error while locking/unlocking" });
        }
        if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
        {
            objFromDb.LockoutEnd = DateTime.Now;
        }
        else
        {
            objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
        }
        _db.SaveChanges();

        return Json(new { success = true, message = "Deleted" });

    }
    #endregion

}