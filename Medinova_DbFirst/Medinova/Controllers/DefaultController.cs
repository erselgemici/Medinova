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
