using Medinova.DTOs;
using Medinova.DTOs.PackageDtos;
using Medinova.Models;
using Medinova.Repositories.PackageRepositories;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminPackageController : AdminBaseController
    {
        PackageRepository repo = new PackageRepository();

        public async Task<ActionResult> Index()
        {
            var values = await repo.GetAllAsync();

            var result = values.Select(x => new ResultPackageDto
            {
                PackageID = x.PackageID,
                PackageName = x.PackageName,
                Price = x.Price,
                Period = x.Period,
                Feature1 = x.Feature1,
                Feature2 = x.Feature2,
                Feature3 = x.Feature3,
                Feature4 = x.Feature4
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
        public async Task<ActionResult> Create(CreatePackageDto p)
        {
            if (!ModelState.IsValid) return View(p);

            Package t = new Package();
            t.PackageName = p.PackageName;
            t.Price = p.Price;
            t.Period = p.Period;
            t.Feature1 = p.Feature1;
            t.Feature2 = p.Feature2;
            t.Feature3 = p.Feature3;
            t.Feature4 = p.Feature4;

            await repo.CreateAsync(t);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var value = await repo.GetByIdAsync(id);
            if (value == null) return RedirectToAction("Index");

            UpdatePackageDto model = new UpdatePackageDto
            {
                PackageID = value.PackageID,
                PackageName = value.PackageName,
                Price = value.Price,
                Period = value.Period,
                Feature1 = value.Feature1,
                Feature2 = value.Feature2,
                Feature3 = value.Feature3,
                Feature4 = value.Feature4
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdatePackageDto p)
        {
            if (!ModelState.IsValid) return View(p);

            var value = await repo.GetByIdAsync(p.PackageID);

            value.PackageName = p.PackageName;
            value.Price = p.Price;
            value.Period = p.Period;
            value.Feature1 = p.Feature1;
            value.Feature2 = p.Feature2;
            value.Feature3 = p.Feature3;
            value.Feature4 = p.Feature4;

            await repo.UpdateAsync(value);
            return RedirectToAction("Index");
        }
    }
}
