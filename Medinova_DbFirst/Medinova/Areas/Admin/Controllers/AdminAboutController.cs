using Medinova.DTOs;
using Medinova.DTOs.AboutDtos;
using Medinova.Models;
using Medinova.Repositories.AboutRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Medinova.Areas.Admin.Controllers
{
    public class AdminAboutController : AdminBaseController
    {
        AboutRepository _repository = new AboutRepository();
        public async Task<ActionResult> Index()
        {
            var values = await _repository.GetAllAsync();

            var result = values.Select(x=> new ResultAboutDto
            {
                AboutID = x.AboutId,
                Title = x.Title,
                Description = x.Description,
                ImageUrl = x.ImageUrl
            }).ToList();

            return View(result);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateAboutDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            About t = new About();
            t.Title = dto.Title;
            t.Description = dto.Description;
            t.ImageUrl = dto.ImageUrl;

            await _repository.CreateAsync(t);

            return RedirectToAction("Index");

        }

        public async Task<ActionResult> Update(int id)
        {
            var value = await _repository.GetByIdAsync(id);
            if (value == null)
            {
                return RedirectToAction("Index");
            }

            UpdateAboutDto model = new UpdateAboutDto()
            {
                AboutId = value.AboutId,
                Title = value.Title,
                Description = value.Description,
                ImageUrl = value.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateAboutDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var value = await _repository.GetByIdAsync(dto.AboutId);

            value.Title = dto.Title;
            value.Description = dto.Description;
            value.ImageUrl = dto.ImageUrl;

            await _repository.UpdateAsync(value);
            return RedirectToAction("Index");
        }
    }
}
