using Medinova.DTOs;
using Medinova.Enums;
using Medinova.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Kullanıcı giriş yapmış mı?
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
                // Hata mesajı ile geri dön
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

                    // İsim Kontrolü (Bölüm adı şikayette geçiyor mu?)
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

                // SONUÇ DÖNDÜRME
                if (bestMatchId > 0)
                {
                    return Json(new { success = true, departmentId = bestMatchId, departmentName = recommendedDeptName });
                }
                else
                {
                    // Eşleşme yoksa Dahiliye'ye yönlendir (Fallback)
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
                // Beklenmedik bir hata olursa 500 hatası vermek yerine bu mesajı göster
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
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
