using System.Web.Mvc;

namespace Medinova.Areas.Doctor.Controllers
{
    public class DoctorBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            var role = Session["UserRole"]?.ToString();

            if (role != "Doctor")
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
