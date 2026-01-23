using Medinova.DTOs;
using Medinova.DTOs.HeroDtos;
using Medinova.Models;
using Medinova.Repositories.HeroRepositories;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminHeroController : AdminBaseController
    {
        HeroRepository _repository = new HeroRepository();
        public async Task<ActionResult> Index()
        {
            var values = await _repository.GetAllAsync();

            var result = values.Select(x => new ResultHeroDto
            {
                HeroID = x.HeroID,
                Title = x.Title,
                Description = x.Description
            }).ToList();

            return View(result);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateHeroDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            Hero h = new Hero();
            h.Title = dto.Title;
            h.Description = dto.Description;

            await _repository.CreateAsync(h);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Update(int id)
        {
            var value = await _repository.GetByIdAsync(id);
            if (value == null) return RedirectToAction("Index");

            UpdateHeroDto model = new UpdateHeroDto
            {
                HeroID = value.HeroID,
                Title = value.Title,
                Description = value.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateHeroDto p)
        {
            if (!ModelState.IsValid)
            {
                return View(p);
            }

            var value = await _repository.GetByIdAsync(p.HeroID);

            value.Title = p.Title;
            value.Description = p.Description;

            await _repository.UpdateAsync(value);
            return RedirectToAction("Index");
        }
    }
}
