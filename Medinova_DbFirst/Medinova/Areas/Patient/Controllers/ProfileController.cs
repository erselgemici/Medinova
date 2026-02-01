using Medinova.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity; 

namespace Medinova.Areas.Patient.Controllers
{
    public class ProfileController : PatientBaseController
    {
        MedinovaContext db = new MedinovaContext();

        public ActionResult Index()
        {
            int userId = (int)Session["UserId"];

            var user = db.Users.Find(userId);
            string userFullName = user.FirstName + " " + user.LastName;

            var myAppointments = db.Appointments
                                   .Include(x => x.Doctor)
                                   .Where(x => x.FullName == userFullName) 
                                   .OrderByDescending(x => x.AppointmentDate)
                                   .ToList();

            return View(myAppointments);
        }

        public ActionResult CancelAppointment(int id)
        {
            var appointment = db.Appointments.Find(id);
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            // Bu randevu gerçekten giriş yapan kişiye mi ait? (Başkası iptal edemesin)
            if (appointment != null && appointment.FullName == (user.FirstName + " " + user.LastName))
            {
                // Randevuyu silmiyoruz, pasife çekiyoruz (İptal edildi olarak görünsün)
                appointment.IsActive = false;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit()
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User p)
        {
            if (string.IsNullOrEmpty(p.FirstName) || string.IsNullOrEmpty(p.Password))
            {
                ViewBag.ErrorMessage = "Ad, Soyad ve Şifre alanları boş bırakılamaz.";
                return View(p);
            }

            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            user.FirstName = p.FirstName;
            user.LastName = p.LastName;
            user.Email = p.Email;
            user.UserName = p.UserName;
            user.Password = p.Password; 

            db.SaveChanges();

            Session["UserInfo"] = user.FirstName + " " + user.LastName;

            TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Edit");
        }
    }
}
