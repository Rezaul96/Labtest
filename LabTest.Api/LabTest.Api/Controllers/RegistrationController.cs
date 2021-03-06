﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabTest.Repository.Core;
using LabTest.Repository.Registration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LabTest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        public readonly UserManager<ApplicationUser> _userManager;
        public RegistrationController(IRegistrationRepository registrationRepository, UserManager<ApplicationUser> userManager)
        {
            _registrationRepository = registrationRepository;
            _userManager = userManager;
        }

        // GET: api/Registration
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _registrationRepository.Get();
            if (list.Any())
                return Ok(list);
            else
                return NotFound();
        }

        //GET: api/Registration/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var registration = await _registrationRepository.GetByIdAysnc(id);
                if (registration == null)
                {
                    return NotFound();
                }

                return Ok(registration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // POST: api/Registration
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.Registration registration)
        {

            try
            {
                if (registration == null)
                    return BadRequest();
                if (registration.Id == 0)
                {
                    var appUser = new ApplicationUser
                    {
                        Email = registration.Email,
                        UserName = registration.Email,
                        PhoneNumber = registration.MobileNumber,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var res = await _userManager.CreateAsync(appUser, registration.Password);

                    var result = await _registrationRepository.Insert(registration);
                    if (result != null)
                        return Ok(result);
                    else
                        return BadRequest();
                }
                else
                {
                    var result = await _registrationRepository.Update(registration);
                    if (result != null)
                        return Ok(result);
                    else
                        return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }



        }

        [HttpGet("{username}/{Token}")]
        public async Task<IActionResult> GetUserByName([FromRoute] string username, [FromRoute] string Token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _registrationRepository.GetByEmailAysnc(username);
            if (users == null)
            {
                return NotFound();
            }         
            return Ok(users);
        }

    }
}