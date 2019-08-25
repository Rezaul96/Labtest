using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LabTest.Web.Configs;
using LabTest.Web.Helpers;
using LabTest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LabTest.Web.Controllers
{
    public class TaskController : BaseController
    {
        private object departmentsQuery;

       
        public async Task<IActionResult> Index()
        {
            var response = await AdminHttpClient.GetAsync(WebConfiguration.Instance.WebApiConfig, "api/Task", Request);
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadAsAsync<List<TaskModelView>>() ?? new List<TaskModelView>();
                return View(list);
            }
            return View(new List<TaskModelView>());
        }

        public async Task<IActionResult> Create()
        {
            await LoadDdl();         
            return View();
        }

        private async Task LoadDdl()
        {
            var lsituser = await AdminHttpClient.GetAsync<List<RegistrationModelView>>(WebConfiguration.Instance.WebApiConfig, $"api/Registration", Request) ?? new List<RegistrationModelView>();
            ViewBag.users = lsituser;
            
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModelView model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.UtcNow;
                model.AsaainTo = currentUser.Id;
                var response = await AdminHttpClient.PostAsync(WebConfiguration.Instance.WebApiConfig, $"api/Task", model, Request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int Id)
        {
            await LoadDdl();
            var taskModelView = await AdminHttpClient.GetAsync<TaskModelView>(WebConfiguration.Instance.WebApiConfig, $"api/Task/{Id}", Request) ?? new TaskModelView();
            return View(taskModelView);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskModelView model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.UtcNow;
                model.AsaainTo = currentUser.Id;
                var response = await AdminHttpClient.PostAsync(WebConfiguration.Instance.WebApiConfig, $"api/Task", model, Request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTask([FromBody]TaskModelView outSide)
        {
            if (outSide == null)
            {
                return BadRequest();
            }

            try
            {
                var response = await AdminHttpClient.DeleteAsync(WebConfiguration.Instance.WebApiConfig, $"api/Task/{outSide.TaskId}", outSide, Request);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }

    
}