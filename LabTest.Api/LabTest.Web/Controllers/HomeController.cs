﻿using System;
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
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
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

            var model = await AdminHttpClient.GetAsync<RegistrationModelView>(WebConfiguration.Instance.WebApiConfig, $"api/Users/{userCredentialViewModel.UserName}/token", credential.Token);
            if (model != null)
            {
                val = HttpUtility.UrlEncode(JsonConvert.SerializeObject(
                    new
                    {
                        FullName = model.FirstLastName,
                        model.Email,
                        //model.Phone,
                        //model.Photo,
                        //UserType = model.UserType.Name,
                        model.Id
                       //model.ImageFullPath
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

     
    }
}
