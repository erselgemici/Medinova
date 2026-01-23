using Medinova.DTOs.AccountDtos;
using Medinova.Models;
using Medinova.Repositories.UserRepositories;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Medinova.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        MedinovaContext db = new MedinovaContext();
        UserRepository userRepository = new UserRepository();

        public ActionResult Index()
        {
            if (Session["AdminId"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var user = userRepository.ValidateUser(model);

            if (user != null)
            {
                // Tarayıcı kapatılıp açılsa bile beni hatırla diyebiliriz.
                FormsAuthentication.SetAuthCookie(user.Email, false);

                // Session: Sunucu tarafında kullanıcının bilgilerini saklarız.
                Session["AdminId"] = user.UserId;
                Session["AdminName"] = user.FirstName + " " + user.LastName;
                Session["AdminEmail"] = user.Email;

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.Error = "Kullanıcı adı şifre hatalı veya yetkiniz yok.";
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}
