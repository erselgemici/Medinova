using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity; // ToListAsync için
using Medinova.Models;
using Medinova.DTOs.DoctorDtos;
using Medinova.Enums; // Times sınıfı burada

namespace Medinova.Areas.Doctor.Controllers
{
    public class ScheduleController : DoctorBaseController
    {
        MedinovaContext db = new MedinovaContext();

        public async Task<ActionResult> Index(DateTime? date)
        {
            if (Session["DoctorId"] == null) return RedirectToAction("Index", "Login", new { area = "" });
            int doctorId = (int)Session["DoctorId"];

            DateTime selectedDate = date ?? DateTime.Today;
            ViewBag.SelectedDate = selectedDate;

            var appointments = await db.Appointments
                                     .Where(x => x.DoctorId == doctorId
                                              && x.IsActive == true
                                              && DbFunctions.TruncateTime(x.AppointmentDate) == selectedDate.Date)
                                     .ToListAsync();

            List<DailyScheduleDto> scheduleList = new List<DailyScheduleDto>();

            var masterTimes = Times.AppointmentHours;

            foreach (var time in masterTimes)
            {
                var existingApp = appointments.FirstOrDefault(x => x.AppointmentTime == time);

                if (existingApp != null)
                {
                    // DOLU SAAT
                    scheduleList.Add(new DailyScheduleDto
                    {
                        Time = time,
                        IsFull = true,
                        PatientName = existingApp.FullName,
                        AppointmentId = existingApp.AppointmentId 
                    });
                }
                else
                {
                    // BOŞ SAAT
                    scheduleList.Add(new DailyScheduleDto
                    {
                        Time = time,
                        IsFull = false,
                        PatientName = "Müsait",
                        AppointmentId = 0
                    });
                }
            }

            return View(scheduleList);
        }
    }
}
