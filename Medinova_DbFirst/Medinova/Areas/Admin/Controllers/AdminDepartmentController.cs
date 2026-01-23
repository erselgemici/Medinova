using Medinova.DTOs.DepartmentDtos;
using Medinova.Models;
using Medinova.Repositories.DepartmentRepositories;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminDepartmentController : Controller
    {
        DepartmentRepository repository = new DepartmentRepository();
        public async Task<ActionResult> Index()
        {
            var values = await repository.GetAllAsync();
            return View(values);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateDepartmentDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Department d = new Department();
            d.Name = model.Name;

            await repository.CreateAsync(d);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var value = await repository.GetByIdAsync(id);

            if (value == null)
            {
                RedirectToAction("Index");
            }

            UpdateDepartmentDto model = new UpdateDepartmentDto
            {
                DepartmentId = value.DepartmentId,
                Name = value.Name,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateDepartmentDto p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            var value = await repository.GetByIdAsync(p.DepartmentId);

            value.Name = p.Name;
            // Sadece Adı güncelliyoruz.

            await repository.UpdateAsync(value);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            await repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
