using Medinova.Models;
using System.Linq;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class LogController : Controller
    {
        MedinovaContext db = new MedinovaContext();

        public ActionResult Index(string search, string type)
        {
            var logs = db.Logs.OrderByDescending(x => x.CreatedDate).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                logs = logs.Where(x => x.Action.Contains(search) ||
                                       x.Description.Contains(search) ||
                                       x.UserRole.Contains(search));
            }

            if (!string.IsNullOrEmpty(type))
            {
                logs = logs.Where(x => x.LogType == type);
            }

            // Performans için 500 kayıt limiti
            var model = logs.Take(500).ToList();
            return View(model);
        }

        [HttpGet]
        public JsonResult GetLogDetails(int id)
        {
            var log = db.Logs.Find(id);
            if (log == null) return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                success = true,
                data = new
                {
                    log.LogId,
                    Date = log.CreatedDate?.ToString("dd.MM.yyyy HH:mm:ss"),
                    log.UserRole,
                    log.UserId,
                    log.IPAddress,
                    log.LogType,
                    log.Action,
                    log.Description
                }
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
