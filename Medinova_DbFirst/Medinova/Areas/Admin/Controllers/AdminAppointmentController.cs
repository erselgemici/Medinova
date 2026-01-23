using Medinova.DTOs.AppointmentDtos;
using Medinova.Repositories.AppointmentRepositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminAppointmentController : AdminBaseController
    {
        AppointmentRepository repo = new AppointmentRepository();

        public async Task<ActionResult> Index()
        {
            var values = await repo.GetAllAsync();

            var result = values.OrderByDescending(x => x.AppointmentDate).Select(x => new ResultAppointmentDto
            {
                AppointmentID = x.AppointmentId,
                PatientName = x.FullName,
                PatientPhone = x.PhoneNumber,
                DoctorName = x.Doctor != null ? x.Doctor.FullName : "Belirtilmemiş",
                DepartmentName = (x.Doctor != null && x.Doctor.Department != null) ? x.Doctor.Department.Name : "Belirtilmemiş",
                AppointmentDate = x.AppointmentDate.GetValueOrDefault(DateTime.Now), 
                AppointmentTime = x.AppointmentTime,

                IsActive = x.IsActive ?? false
            }).ToList();

            return View(result);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await repo.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        // Randevu durumunu (Onaylandı/Bekliyor) değiştirmek istersen buraya bir metod eklenebilir.
    }
}
