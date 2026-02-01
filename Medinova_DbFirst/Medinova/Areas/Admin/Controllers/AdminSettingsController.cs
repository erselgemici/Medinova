using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Medinova.Models;
using Medinova.Repositories.SettingsRepositories;
using Medinova.DTOs.SettingsDtos;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminSettingsController : AdminBaseController
    {
        SettingsRepository repo = new SettingsRepository();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var values = await repo.GetAllAsync();
            var setting = values.FirstOrDefault();

            if (setting == null) return View(new ResultSettingsDto());

            var model = new ResultSettingsDto
            {
                SettingID = setting.SettingID,
                PhoneNumber = setting.PhoneNumber,
                Email = setting.Email,
                Address = setting.Address,
                FooterDescription = setting.FooterDescription
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Update(int id)
        {
            var value = await repo.GetByIdAsync(id);
            if (value == null) return RedirectToAction("Index");

            var model = new UpdateSettingsDto
            {
                SettingID = value.SettingID,
                LogoUrl = value.LogoUrl,
                PhoneNumber = value.PhoneNumber,
                Email = value.Email,
                Address = value.Address,
                MapUrl = value.MapUrl,
                FooterDescription = value.FooterDescription,
                TwitterUrl = value.TwitterUrl,
                FacebookUrl = value.FacebookUrl,
                InstagramUrl = value.InstagramUrl,
                LinkedinUrl = value.LinkedinUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateSettingsDto p)
        {
            var value = await repo.GetByIdAsync(p.SettingID);

            value.LogoUrl = p.LogoUrl;
            value.PhoneNumber = p.PhoneNumber;
            value.Email = p.Email;
            value.Address = p.Address;
            value.MapUrl = p.MapUrl;
            value.FooterDescription = p.FooterDescription;
            value.TwitterUrl = p.TwitterUrl;
            value.FacebookUrl = p.FacebookUrl;
            value.InstagramUrl = p.InstagramUrl;
            value.LinkedinUrl = p.LinkedinUrl;

            await repo.UpdateAsync(value);
            return RedirectToAction("Index");
        }
    }
}
