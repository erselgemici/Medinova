using Medinova.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

namespace Medinova.Areas.Doctor.Controllers
{
    public class ProfileController : DoctorBaseController
    {
        MedinovaContext db = new MedinovaContext();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            if (Session["DoctorId"] == null) return RedirectToAction("Index", "Login", new { area = "" });

            int id = (int)Session["DoctorId"];

            var value = await db.Doctors.FindAsync(id);

            return View(value);
        }

        [HttpPost]
        public async Task<ActionResult> Index(Medinova.Models.Doctor p)
        {
            var value = await db.Doctors.FindAsync(p.DoctorId);

            if (value != null)
            {
                // Sadece izin verilen alanları güncelle
                value.FullName = p.FullName;
                value.ImageUrl = p.ImageUrl;
                value.Description = p.Description;

                // Doktor branşını kendi değiştirememeli, Admin değiştirmeli.

                await db.SaveChangesAsync();

                // Layout'taki ismin hemen değişmesi için Session'ı güncelle
                Session["UserInfo"] = value.FullName;

                ViewBag.Success = "Profil bilgileriniz başarıyla güncellendi.";
            }

            return View(value);
        }
    }
}
