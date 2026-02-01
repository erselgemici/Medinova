using System.Web.Mvc;

namespace Medinova.Areas.Patient.Controllers
{
    public class PatientBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            var role = Session["UserRole"]?.ToString();

            if (role != "Patient")
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
