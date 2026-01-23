using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Session kontrolü: Kullanıcı giriş yapmamışsa (AdminId boşsa)
            if (Session["AdminId"] == null)
            {
                filterContext.Result = new RedirectResult("/Admin/Login/Index");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
