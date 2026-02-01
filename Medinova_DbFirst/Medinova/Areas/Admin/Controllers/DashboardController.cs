using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Medinova.Models;
using Medinova.DTOs;
using System.Globalization;
using Medinova.Services; // ML Servisi için ekledik

namespace Medinova.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        MedinovaContext db = new MedinovaContext();

        public ActionResult Index()
        {
            AdminDashboardDto model = new AdminDashboardDto();

            // GENEL SAYAÇLAR
            model.TotalDoctors = db.Doctors.Count();
            model.TotalDepartments = db.Departments.Count();
            model.TotalAppointments = db.Appointments.Count();
            model.TotalActiveAppointments = db.Appointments.Count(x => x.IsActive == true);
            model.TotalRegisteredPatients = db.Users.Count(x => x.Roles.Any(r => r.RoleName == "Patient"));

            // EN ÇOK RANDEVU ALINAN DOKTORLAR (Top 5)
            var doctorStats = db.Appointments
                .Where(x => x.IsActive == true)
                .GroupBy(x => x.Doctor.FullName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            model.DoctorNames = doctorStats.Select(x => x.Name).ToList();
            model.AppointmentCounts = doctorStats.Select(x => x.Count).ToList();

            // BÖLÜM YOĞUNLUĞU (Pasta Grafik)
            var deptStats = db.Appointments
                .Where(x => x.IsActive == true && x.Doctor.Department != null)
                .GroupBy(x => x.Doctor.Department.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            model.DepartmentNames = deptStats.Select(x => x.Name).ToList();
            model.DepartmentPercentages = deptStats.Select(x => x.Count).ToList();

            // SON 6 AYIN PERFORMANSI (Çizgi Grafik)
            model.Last6Months = new List<string>();
            model.MonthlyAppointmentCounts = new List<int>();

            var today = DateTime.Today;
            for (int i = 5; i >= 0; i--) // Bugünden geriye 6 ay
            {
                var monthDate = today.AddMonths(-i);
                var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                int count = db.Appointments.Count(x => x.AppointmentDate >= monthStart && x.AppointmentDate <= monthEnd);

                model.Last6Months.Add(monthDate.ToString("MMMM", new CultureInfo("tr-TR")));
                model.MonthlyAppointmentCounts.Add(count);
            }

            // SON RANDEVULAR TABLOSU
            model.LastAppointments = db.Appointments
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.AppointmentDate)
                .Take(5)
                .ToList();

            try
            {
                AppointmentForecaster forecaster = new AppointmentForecaster();
                var predictions = forecaster.PredictNext3Months();

                ViewBag.ForecastDates = predictions.Select(x => x.Date.ToString("dd MMM")).ToList();
                ViewBag.ForecastCounts = predictions.Select(x => x.Count).ToList();

                // 1. Toplam Tahmin
                float totalPredicted = predictions.Sum(x => x.Count);
                ViewBag.TotalPredicted = totalPredicted;

                // 2. En Yoğun Gün Tahmini
                var maxDay = predictions.OrderByDescending(x => x.Count).FirstOrDefault();
                ViewBag.MaxDayDate = maxDay?.Date.ToString("dd MMMM dddd", new CultureInfo("tr-TR"));
                ViewBag.MaxDayCount = maxDay?.Count;

                // 3. Artış/Azalış Durumu (İlk hafta vs Son hafta)
                var firstWeek = predictions.Take(7).Sum(x => x.Count);
                var lastWeek = predictions.Skip(predictions.Count - 7).Sum(x => x.Count);
                ViewBag.Trend = lastWeek > firstWeek ? "Artış Bekleniyor 📈" : "Düşüş Bekleniyor 📉";
            }
            catch (Exception ex)
            {
                ViewBag.ForecastDates = new List<string>();
                ViewBag.ForecastCounts = new List<float>();
                ViewBag.MLError = "ML Tahmin Hatası: " + ex.Message;
            }

            return View(model);
        }
    }
}
