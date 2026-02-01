using Medinova.DTOs;
using Medinova.Models;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        MedinovaContext db = new MedinovaContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(RegisterDto p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            var existingUser = db.Users.FirstOrDefault(x => x.UserName == p.Username || x.Email == p.Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Bu kullanıcı adı veya e-posta zaten sistemde kayıtlı!";
                return View(p);
            }

            // Kullanıcıyı Oluştur
            User newUser = new User
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                UserName = p.Username,
                Email = p.Email,
                Password = p.Password,
                DoctorId = null
            };

            if (newUser.Roles == null)
            {
                newUser.Roles = new List<Role>();
            }

            db.Users.Add(newUser);

            var patientRole = db.Roles.FirstOrDefault(x => x.RoleName == "Patient" || x.RoleId == 3);

            if (patientRole != null)
            {
                newUser.Roles.Add(patientRole);
            }

            db.SaveChanges();

            TempData["SuccessMessage"] = "Kaydınız başarıyla oluşturuldu. Lütfen giriş yapınız.";
            return RedirectToAction("Index", "Login");
        }
    }
}
