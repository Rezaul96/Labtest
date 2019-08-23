using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LabTest.Web.Configs;
using LabTest.Web.Helpers;
using LabTest.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace LabTest.Web.Controllers
{
    public class RegistrationController : BaseController
    {
        public async Task<IActionResult> Index()
        {         
           
            var response = await AdminHttpClient.GetAsync(WebConfiguration.Instance.WebApiConfig, "api/Registration", Request);
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadAsAsync<List<RegistrationModelView>>() ?? new List<RegistrationModelView>();
                return View(list);
            }
            return View(new List<RegistrationModelView>());
        }

        public async Task<IActionResult> Create()
        {          
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( RegistrationModelView model)
        {
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.UtcNow;
                model.CreateBy = 1;//currentUser.Id;
                var response = await AdminHttpClient.PostAsync(WebConfiguration.Instance.WebApiConfig, $"api/Registration", model, Request);
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
    }
}