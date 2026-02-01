using System;
using System.Linq;
using System.Web.Mvc;
using Medinova.Models;

namespace Medinova.Areas.Doctor.Controllers
{
    public class DashboardController : DoctorBaseController
    {
        MedinovaContext db = new MedinovaContext();

        public ActionResult Index()
        {
            if (Session["DoctorId"] == null) return RedirectToAction("Index", "Login", new { area = "" });

            int doctorId = (int)Session["DoctorId"];

            var myAppointments = db.Appointments
                                   .Where(x => x.DoctorId == doctorId && x.IsActive == true)
                                   .OrderBy(x => x.AppointmentDate)
                                   .ThenBy(x => x.AppointmentTime)
                                   .ToList();

            return View(myAppointments);
        }

        [HttpGet]
        public ActionResult DeleteAppointment(int id)
        {
            var appointment = db.Appointments.Find(id);

            int currentDoctorId = (int)Session["DoctorId"];
            if (appointment.DoctorId != currentDoctorId)
            {
                return RedirectToAction("Index");
            }

            appointment.IsActive = false;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
