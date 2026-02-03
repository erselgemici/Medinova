using Medinova.DTOs;
using Medinova.Enums;
using Medinova.Models;
using Medinova.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Medinova.Controllers
{
    [AllowAnonymous]
    public class DefaultController : Controller
    {

        MedinovaContext _context = new MedinovaContext();

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult DefaultAppointment()
        {
            var departments = _context.Departments.ToList();
            ViewBag.departments = (from department in departments
                                   select new SelectListItem
                                   {
                                       Text = department.Name,
                                       Value = department.DepartmentId.ToString()
                                   }).ToList();

            var dateList = new List<SelectListItem>();

            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(i);

                dateList.Add(new SelectListItem
                {
                    Text = date.ToString("dd.MMMM.dddd"),
                    Value = date.ToString("yyyy-MM-dd")
                });
            }

            ViewBag.dateList = dateList;

            return PartialView();
        }

        [HttpPost]
        public ActionResult MakeAppointment(Appointment appointment)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            // O doktora, o tarihte ve saatte AKTİF bir randevu var mı?
            bool isBooked = _context.Appointments.Any(x =>
                x.DoctorId == appointment.DoctorId &&
                x.AppointmentDate == appointment.AppointmentDate &&
                x.AppointmentTime == appointment.AppointmentTime &&
                x.IsActive == true);
            if (isBooked)
            {
                // Hata mesajı ile geri dön (Bunu da SweetAlert ile gösterebilirsin istersen)
                TempData["Error"] = "Seçilen saatte randevu doludur, lütfen başka bir saat seçiniz.";
                return RedirectToAction("Index");
            }

            int userId = (int)Session["UserId"];
            var currentUser = _context.Users.Find(userId);

            appointment.FullName = currentUser.FirstName + " " + currentUser.LastName;
            appointment.Email = currentUser.Email;

            appointment.IsActive = true;
            _context.Appointments.Add(appointment);
            _context.SaveChanges();


            try
            {

                var log = new Log
                {
                    UserId = userId,
                    UserRole = "Patient",
                    LogType = "Business",
                    Action = "Randevu Alındı",
                    Description = $"{currentUser.FirstName} {currentUser.LastName} {DateTime.Now:dd MMMM yyyy HH:mm}'de, {appointment.AppointmentDate:dd.MM.yyyy} tarihi için randevu aldı.",
                    IPAddress = Request.UserHostAddress,
                    CreatedDate = DateTime.Now
                };
                _context.Logs.Add(log);
                _context.SaveChanges();

                var doctor = _context.Doctors.Find(appointment.DoctorId);
                string doctorName = doctor != null ? doctor.FullName : "Belirtilmedi";

                string host = WebConfigurationManager.AppSettings["SmtpHost"];
                int port = int.Parse(WebConfigurationManager.AppSettings["SmtpPort"]);
                string mailAddress = WebConfigurationManager.AppSettings["SmtpMail"];
                string mailPass = WebConfigurationManager.AppSettings["SmtpPass"];

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(mailAddress, mailPass);

                    var mail = new MailMessage();
                    mail.From = new MailAddress(mailAddress, "Medinova Hastanesi");
                    mail.To.Add(appointment.Email);
                    mail.Subject = "Randevu Onayı - Medinova";
                    mail.IsBodyHtml = true;
                    mail.Body = $@"
                        <div style='font-family:Arial; padding:20px; border:1px solid #eee;'>
                            <h2 style='color:#007bff;'>Sayın {appointment.FullName},</h2>
                            <p>Randevunuz başarıyla oluşturulmuştur.</p>
                            <ul>
                                <li><b>Doktor:</b> {doctorName}</li>
                                <li><b>Tarih:</b> {appointment.AppointmentDate:dd.MM.yyyy}</li>
                                <li><b>Saat:</b> {appointment.AppointmentTime}</li>
                            </ul>
                            <p>Geçmiş olsun dileklerimizle.</p>
                        </div>";

                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                // Mail hatası loglanabilir
            }

            TempData["AppointmentSuccess"] = "Randevunuz başarıyla oluşturulmuştur. Detaylar e-posta adresinize gönderildi.";

            return RedirectToAction("Index");
        }


        public JsonResult GetDoctorsByDepartmentId(int departmentId)
        {
            var doctors = _context.Doctors.Where(x => x.DepartmentId == departmentId)
                                    .Select(doctor => new SelectListItem
                                    {
                                        Text = doctor.FullName,
                                        Value = doctor.DoctorId.ToString()
                                    }).ToList();

            return Json(doctors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAvailableHours(DateTime selectedDate, int doctorId)
        {
            var bookedTimes = _context.Appointments.Where(x => x.DoctorId == doctorId && x.AppointmentDate == selectedDate)
                                                   .Select(x => x.AppointmentTime).ToList();

            var dtoList = new List<AppointmentAvailabilityDto>();
            foreach (var hour in Times.AppointmentHours)
            {
                var dto = new AppointmentAvailabilityDto();
                dto.Time = hour;

                if (bookedTimes.Contains(hour))
                {
                    dto.IsBooked = true;
                }
                else
                {
                    dto.IsBooked = false;
                }

                dtoList.Add(dto);
            }

            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDepartmentRecommendation(string complaint)
        {
            try
            {
                if (string.IsNullOrEmpty(complaint))
                {
                    return Json(new { success = false, message = "Lütfen şikayetinizi yazınız." });
                }

                complaint = complaint.ToLower(new System.Globalization.CultureInfo("tr-TR"));

                var departments = _context.Departments.ToList();

                var keywords = new Dictionary<string, List<string>>
                {
                    { "Kardiyoloji", new List<string> { "kalp", "çarpıntı", "göğüs ağrısı", "tansiyon", "damar", "nefes darlığı" } },
                    { "Dahiliye", new List<string> { "mide", "karın", "halsizlik", "grip", "ateş", "şeker", "diyabet", "bulantı", "kusma" } },
                    { "Göz Hastalıkları", new List<string> { "göz", "bulanık", "görme", "arıpacık", "miyop" } },
                    { "Ortopedi", new List<string> { "kırık", "çıkık", "eklem", "diz", "bel", "boyun", "ağrı", "kemik" } },

                    { "Pediatri", new List<string> { "çocuk", "bebek", "aşı", "gelişim", "pişik", "havale", "oğlum", "kızım" } },

                    { "Çocuk Sağlığı", new List<string> { "çocuk", "bebek", "aşı", "gelişim", "pişik", "havale" } },

                    { "Dermatoloji", new List<string> { "cilt", "deri", "kaşıntı", "sivilce", "akne", "egzama" } },
                    { "Kulak Burun Boğaz", new List<string> { "kulak", "burun", "boğaz", "duyma", "bademcik", "sinüzit" } },
                    { "Nöroloji", new List<string> { "baş ağrısı", "migren", "unutkanlık", "titreme", "felç" } },
                    { "Psikiyatri", new List<string> { "depresyon", "stres", "uykusuzluk", "panik", "anksiyete" } },
                    { "Genel Cerrahi", new List<string> { "apandisit", "fıtık", "safra", "meme", "hemoroid" } }
                };

                int bestMatchId = 0;
                int maxScore = 0;
                string recommendedDeptName = "";

                // EŞLEŞTİRME ALGORİTMASI
                foreach (var dept in departments)
                {
                    int score = 0;

                    if (keywords.ContainsKey(dept.Name))
                    {
                        foreach (var key in keywords[dept.Name])
                        {
                            if (complaint.Contains(key))
                            {
                                score += 2;
                            }
                        }
                    }

                    if (complaint.Contains(dept.Name.ToLower(new System.Globalization.CultureInfo("tr-TR"))))
                    {
                        score += 10;
                    }

                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestMatchId = dept.DepartmentId;
                        recommendedDeptName = dept.Name;
                    }
                }

                if (bestMatchId > 0)
                {
                    return Json(new { success = true, departmentId = bestMatchId, departmentName = recommendedDeptName });
                }
                else
                {
                    var defaultDept = departments.FirstOrDefault(x => x.Name == "Dahiliye" || x.Name == "İç Hastalıkları");
                    if (defaultDept != null)
                    {
                        return Json(new { success = true, departmentId = defaultDept.DepartmentId, departmentName = defaultDept.Name + " (Genel Kontrol)" });
                    }

                    return Json(new { success = false, message = "Şikayetinize uygun bölüm bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AskGemini(string question)
        {
            try
            {
                var departments = _context.Departments.Select(x => x.Name).ToList();
                string hospitalContext = string.Join(", ", departments);

                string apiKey = WebConfigurationManager.AppSettings["GeminiApiKey"];
                string modelName = "models/gemini-1.5-flash";

                using (var client = new HttpClient())
                {
                    try
                    {
                        string listUrl = $"https://generativelanguage.googleapis.com/v1beta/models?key={apiKey}";
                        var listResponse = await client.GetStringAsync(listUrl);
                        var listJson = JObject.Parse(listResponse);
                        var latestModel = listJson["models"]
                            .Where(m => m["name"].ToString().Contains("gemini") &&
                                            m["supportedGenerationMethods"].Any(x => x.ToString() == "generateContent"))
                            .OrderByDescending(m => m["name"].ToString())
                            .FirstOrDefault();
                        if (latestModel != null) modelName = latestModel["name"].ToString();
                    }
                    catch { }

                    string url = $"https://generativelanguage.googleapis.com/v1beta/{modelName}:generateContent?key={apiKey}";

                    var prompt = $@"
                Sen Medinova Sağlık Grubu'nun uzman ve yardımsever yapay zeka asistanısın.
                
                Hastanemizde SADECE şu tıbbi bölümler bulunmaktadır: [{hospitalContext}].
                
                Görevin:
                1. Hastanın şikayetini ({question}) analiz et.
                2. Önce hastayı rahatlatacak, evde uygulayabileceği çok kısa ve güvenli bir tıbbi tavsiye ver (Örn: 'Bol sıvı tüketin').
                3. Ardından şikayeti için YALNIZCA yukarıdaki listede yer alan en uygun bölümü seç.
                4. Cevabını tam olarak şu cümleyle bitir: 'Tedaviniz için hastanemizin [Bölüm Adı] bölümündeki uzman hekimlerimizden randevu oluşturabilirsiniz.'
                
                Not: Asla listede olmayan bir bölüm önerme. Cevabın 3-4 cümleyi geçmesin.
            ";

                    var requestBody = new
                    {
                        contents = new[] {
                    new {
                        parts = new[] {
                            new { text = prompt }
                        }
                    }
                }
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, jsonContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseString);

                    if (!response.IsSuccessStatusCode)
                    {
                        string err = json["error"]?["message"]?.ToString() ?? "Hata";
                        return Json(new { success = false, reply = $"Hata: {err}" });
                    }

                    string answer = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    // Loglama
                    try
                    {
                        new LogService().CreateLog((int?)Session["UserId"], "Patient", "Audit", $"AI ({modelName})", $"Soru: {question} | Cevap: {answer}");
                    }
                    catch { }

                    return Json(new { success = true, reply = answer });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, reply = "Sistem Hatası: " + ex.Message });
            }
        }


        public PartialViewResult DefaultHero()
        {
            var values = _context.Heroes.FirstOrDefault();
            var dto = new ResultHeroDto
            {
                HeroID = values.HeroID,
                Title = values.Title,
                Description = values.Description
            };
            return PartialView(dto);
        }

        public PartialViewResult DefaultAbout()
        {
            var aboutData = _context.Abouts.FirstOrDefault();
            var items = _context.AboutItems.ToList();
            ViewBag.Items = items;
            var dto = new ResultAboutDto
            {
                AboutID = aboutData.AboutId,
                Title = aboutData.Title,
                Description = aboutData.Description,
                ImageUrl = aboutData.ImageUrl
            };
            return PartialView(dto);
        }

        public PartialViewResult DefaultServices()
        {
            var values = _context.Services.ToList();
            return PartialView(values);
        }

        public PartialViewResult DefaultPricing()
        {
            var values = _context.Packages.ToList();
            return PartialView(values);
        }

        public PartialViewResult DefaultTeam()
        {
            var values = _context.Doctors.ToList();
            return PartialView(values);
        }

        public PartialViewResult DefaultSearch()
        {
            return PartialView();
        }
        public PartialViewResult DefaultTestimonial()
        {
            var values = _context.Testimonials.ToList();
            return PartialView(values);
        }

        //UI
        public PartialViewResult PartialHead()
        {
            return PartialView();
        }

        public PartialViewResult PartialNavbar()
        {
            return PartialView();
        }

        public PartialViewResult PartialFooter()
        {
            return PartialView();
        }

        public PartialViewResult PartialScripts()
        {
            return PartialView();
        }
    }
}
