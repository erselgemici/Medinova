using Medinova.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        MedinovaContext db = new MedinovaContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User p)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == p.UserName && x.Password == p.Password);

            if (user != null)
            {
                var userRole = user.Roles.FirstOrDefault()?.RoleName;

                if (string.IsNullOrEmpty(userRole)) userRole = "Patient";

                FormsAuthentication.SetAuthCookie(user.UserName, false);

                Session["UserInfo"] = user.FirstName + " " + user.LastName;
                Session["UserId"] = user.UserId;
                Session["UserRole"] = userRole;

                if (user.DoctorId != null)
                {
                    Session["DoctorId"] = user.DoctorId;
                }


                if (userRole == "Admin")
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else if (userRole == "Doctor")
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Doctor" });
                }
                else
                {
                    return RedirectToAction("Index", "Default", new { area = "" });
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login", new { area = "" });
        }
    }
}
