using Medinova.DTOs;
using Medinova.DTOs.DoctorDtos;
using Medinova.Models;
using Medinova.Repositories;
using Medinova.Repositories.DoctorRepositories;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminDoctorController : AdminBaseController
    {
        DoctorRepository repo = new DoctorRepository();

        private async Task LoadDepartmentsDropdownAsync()
        {
            GenericRepository<Department> depRepo = new GenericRepository<Department>();
            var departments = await depRepo.GetAllAsync();

            ViewBag.Departments = departments.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.DepartmentId.ToString()
            }).ToList();
        }

        public async Task<ActionResult> Index()
        {
            var values = await repo.GetAllAsync();

            var doctorDtos = values.Select(x => new ResultDoctorDto
            {
                DoctorID = x.DoctorId,
                FullName = x.FullName,
                ImageUrl = x.ImageUrl,
                DepartmentName = x.Department != null ? x.Department.Name : "Bölüm Yok"
            }).ToList();

            return View(doctorDtos);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            await LoadDepartmentsDropdownAsync();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateDoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsDropdownAsync();
                return View(doctorDto);
            }

            Medinova.Models.Doctor doctor = new Medinova.Models.Doctor();

            doctor.FullName = doctorDto.FullName;
            doctor.DepartmentId = doctorDto.DepartmentId;
            doctor.ImageUrl = doctorDto.ImageUrl;
            doctor.Description = doctorDto.Description;

            await repo.CreateAsync(doctor);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            await repo.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var value = await repo.GetByIdAsync(id);

            if (value == null)
            {
                return RedirectToAction("Index");
            }

            await LoadDepartmentsDropdownAsync();

            UpdateDoctorDto model = new UpdateDoctorDto
            {
                DoctorId = value.DoctorId,
                FullName = value.FullName,
                Description = value.Description,
                ImageUrl = value.ImageUrl,
                DepartmentId = value.DepartmentId != null ? (int)value.DepartmentId : 0
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateDoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsDropdownAsync();
                return View(doctorDto);
            }

            var existingDoctor = await repo.GetByIdAsync(doctorDto.DoctorId);

            if (existingDoctor != null)
            {
                existingDoctor.FullName = doctorDto.FullName;
                existingDoctor.Description = doctorDto.Description;
                existingDoctor.ImageUrl = doctorDto.ImageUrl;

                if (doctorDto.DepartmentId > 0)
                {
                    existingDoctor.DepartmentId = doctorDto.DepartmentId;
                }

                await repo.UpdateAsync(existingDoctor);
            }

            return RedirectToAction("Index");
        }
    }
}
