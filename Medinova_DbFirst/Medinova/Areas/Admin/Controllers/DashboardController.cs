using Medinova.Repositories.AppointmentRepositories;
using Medinova.Repositories.DepartmentRepositories;
using Medinova.Repositories.DoctorRepositories;
using Medinova.Repositories.ServiceRepositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        AppointmentRepository _appointmentRepo = new AppointmentRepository();
        DoctorRepository _doctorRepo = new DoctorRepository();
        DepartmentRepository _deptRepo = new DepartmentRepository();
        ServiceRepository _serviceRepo = new ServiceRepository();

        public async Task<ActionResult> Index()
        {
            var appointments = await _appointmentRepo.GetAllAsync();
            var doctors = await _doctorRepo.GetAllAsync();
            var departments = await _deptRepo.GetAllAsync();
            var services = await _serviceRepo.GetAllAsync();

            ViewBag.AppointmentCount = appointments.Count;
            ViewBag.DoctorCount = doctors.Count;
            ViewBag.DepartmentCount = departments.Count;
            ViewBag.ServiceCount = services.Count;

            var recentAppointments = appointments
                                        .OrderByDescending(x => x.AppointmentDate)
                                        .Take(5)
                                        .ToList();

            var today = DateTime.Today;
            var last7Days = Enumerable.Range(0, 7)
                                      .Select(i => today.AddDays(-6 + i))
                                      .ToList();

            var dbAppointments = await _appointmentRepo.GetAllAsync();

            var chartData = from date in last7Days
                            join app in dbAppointments on date equals app.AppointmentDate?.Date into joined
                            select new
                            {
                                Date = date.ToString("dd MMM"), // Örn: 24 Oca
                                Count = joined.Count()
                            };

            ViewBag.ChartDates = chartData.Select(x => x.Date).ToList();
            ViewBag.ChartCounts = chartData.Select(x => x.Count).ToList();

            return View(recentAppointments);
        }
    }
}
