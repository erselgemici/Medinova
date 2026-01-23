using Medinova.DTOs;
using Medinova.DTOs.TestimonialDtos;
using Medinova.Models;
using Medinova.Repositories.TestimonialRepositories;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminTestimonialController : AdminBaseController
    {
        TestimonialRepository repo = new TestimonialRepository();

        public async Task<ActionResult> Index()
        {
            var values = await repo.GetAllAsync();

            var result = values.Select(x => new ResultTestimonialDto
            {
                TestimonialID = x.TestimonialID,
                Name = x.Name,
                Profession = x.Profession,
                Description = x.Description,
                ImageUrl = x.ImageUrl
            }).ToList();

            return View(result);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await repo.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateTestimonialDto p)
        {
            if (!ModelState.IsValid) return View(p);

            Testimonial t = new Testimonial();
            t.Name = p.Name;
            t.Profession = p.Profession;
            t.Description = p.Description;
            t.ImageUrl = p.ImageUrl;

            await repo.CreateAsync(t);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var value = await repo.GetByIdAsync(id);
            if (value == null) return RedirectToAction("Index");

            UpdateTestimonialDto model = new UpdateTestimonialDto
            {
                TestimonialID = value.TestimonialID,
                Name = value.Name,
                Profession = value.Profession,
                Description = value.Description,
                ImageUrl = value.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateTestimonialDto p)
        {
            if (!ModelState.IsValid) return View(p);

            var value = await repo.GetByIdAsync(p.TestimonialID);

            value.Name = p.Name;
            value.Profession = p.Profession;
            value.Description = p.Description;
            value.ImageUrl = p.ImageUrl;

            await repo.UpdateAsync(value);
            return RedirectToAction("Index");
        }
    }
}
