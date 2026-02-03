using Medinova.Models;
using System;
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

            
                try
                {
                    var log = new Log
                    {
                        UserId = user.UserId,
                        UserRole = userRole,
                        LogType = "Auth", 
                        Action = "Giriş Başarılı",
                        Description = $"{user.FirstName} {user.LastName} sisteme başarıyla giriş yaptı.",
                        IPAddress = Request.UserHostAddress,
                        CreatedDate = DateTime.Now
                    };
                    db.Logs.Add(log);
                    db.SaveChanges();
                }
                catch {  }

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
                
                try
                {
                    var log = new Log
                    {
                        UserId = null,
                        UserRole = "Anonymous",
                        LogType = "Auth",
                        Action = "Hatalı Giriş",
                        Description = $"Hatalı şifre denemesi yapıldı. Denenen Kullanıcı Adı: {p.UserName}",
                        IPAddress = Request.UserHostAddress,
                        CreatedDate = DateTime.Now
                    };
                    db.Logs.Add(log);
                    db.SaveChanges();
                }
                catch { }

                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        public ActionResult Logout()
        {
          
            if (Session["UserId"] != null)
            {
                try
                {
                    int uid = (int)Session["UserId"];
                    string role = Session["UserRole"]?.ToString() ?? "Unknown";
                    string name = Session["UserInfo"]?.ToString() ?? "Kullanıcı";

                    var log = new Log
                    {
                        UserId = uid,
                        UserRole = role,
                        LogType = "Auth",
                        Action = "Çıkış Yapıldı",
                        Description = $"{name} sistemden güvenli çıkış yaptı.",
                        IPAddress = Request.UserHostAddress,
                        CreatedDate = DateTime.Now
                    };
                    db.Logs.Add(log);
                    db.SaveChanges();
                }
                catch { }
            }

            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login", new { area = "" });
        }
    }
}
