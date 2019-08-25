using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LabTest.Web.Models;
using Microsoft.AspNetCore.Authorization;
using LabTest.Web.Helpers;
using LabTest.Web.Configs;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Web;

namespace LabTest.Web.Controllers
{
    public class HomeController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            int userId = currentUser.Id;
            var response = await AdminHttpClient.GetAsync(WebConfiguration.Instance.WebApiConfig, $"api/Task?assainedId={userId}", Request);
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadAsAsync<List<TaskModelView>>() ?? new List<TaskModelView>();
                return View(list);
            }
            return View(new List<TaskModelView>());
           
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(UserCredentialViewModel userCredentialViewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return RedirectToAction("Login", "Home");

            var response = await AdminHttpClient.PostAsync(WebConfiguration.Instance.WebApiConfig, "api/Auth/Login", userCredentialViewModel, Request);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Home", "", "LoginFail");
            }

            var credential = await response.Content.ReadAsAsync<UserCredentialViewModel>();
            CookieOptions cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTime.Now.AddHours(5),
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            };
            var val = credential.Token;
            Response.Cookies.Append(ConfigKeys.AuthCookieKey, val, cookieOptions);

            var model = await AdminHttpClient.GetAsync<RegistrationModelView>(WebConfiguration.Instance.WebApiConfig, $"api/Registration/{userCredentialViewModel.UserName}/token", credential.Token);
            if (model != null)
            {
                val = HttpUtility.UrlEncode(JsonConvert.SerializeObject(
                    new
                    {
                        FullName = model.FirstLastName,
                        model.Email,                      
                        model.Id,
                        model.MobileNumber
                    }, Formatting.None));

                Response.Cookies.Append(ConfigKeys.UserCookieKey, val, cookieOptions);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await AdminHttpClient.PostAsync(WebConfiguration.Instance.WebApiConfig, "/api/Auth/Logout", (object)null, Request);

                Response.Cookies.Delete("Auth");
                Response.Cookies.Delete("user");

                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }


    }
}
