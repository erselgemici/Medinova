using Medinova.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    public class AiController : Controller
    {
        // GET: Ai
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAdvice(string question)
        {
            GeminiService aiService = new GeminiService();
            LogService logger = new LogService();

            // AI Cevabı
            string answer = await aiService.AskGeminiAsync(question);

            // Loglama (Audit)
            int? userId = Session["UserId"] as int?;
            logger.CreateLog(userId, "Patient", "Audit", "AI Soru", $"Soru: {question} | Cevap: {answer}");

            return Json(new { reply = answer });
        }
    }
}
