using Medinova.Models;
using Medinova.Repositories.ServiceRepositories;
using Medinova.DTOs.ServiceDtos;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Medinova.DTOs;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminServiceController : AdminBaseController
    {
        ServiceRepository _repository = new ServiceRepository();


        public async Task<ActionResult> Index()
        {
            var values = await _repository.GetAllAsync();

            var serviceList = values.Select(x => new ResultServiceDto
            {
                ServiceID = x.ServiceID,
                Title = x.Title,
                Description = x.Description,
                Icon = x.Icon
            }).ToList();

            return View(serviceList);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateServiceDto p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            Service s = new Service();
            s.Title = p.Title;
            s.Description = p.Description;
            s.Icon = p.Icon;

            await _repository.CreateAsync(s);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var value = await _repository.GetByIdAsync(id);

            if (value == null)
            {
                return RedirectToAction("Index");
            }

            UpdateServiceDto model = new UpdateServiceDto
            {
                ServiceID = value.ServiceID,
                Title = value.Title,
                Description = value.Description,
                Icon = value.Icon
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateServiceDto p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            var value = await _repository.GetByIdAsync(p.ServiceID);

            value.Title = p.Title;
            value.Description = p.Description;
            value.Icon = p.Icon;

            await _repository.UpdateAsync(value);

            return RedirectToAction("Index");
        }
    }
}
