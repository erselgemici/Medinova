using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            var role = Session["UserRole"]?.ToString();

            if (role != "Admin")
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
