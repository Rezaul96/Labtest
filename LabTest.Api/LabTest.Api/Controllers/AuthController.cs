using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LabTest.Api.Helpers;
using LabTest.Models;
using LabTest.Repository.Core;
using LabTest.Repository.Registration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LabTest.Api.Controllers
{
    [Route("api/[controller]")]

    public class AuthController : LabTestControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;    
        private readonly IConfiguration _configuration;
        private readonly IRegistrationRepository _registrationRepository;     

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration,   IRegistrationRepository registrationRepository)
        {
            _userManager = userManager;
            _configuration = configuration;       
            _registrationRepository = registrationRepository;
        }

        [HttpPost]
        [Route("Login")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(401)]
        //[ProducesResponseType(500)]
        public async Task<IActionResult> Index([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return Unauthorized();
            }
            var login = await _registrationRepository.GetByEmailAysnc(user.Email);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.PhoneNumber ?? "017XXXXXXXXX"),
                new Claim (JwtRegisteredClaimNames.Jti , login.Id.ToString())
            };

            var claimIdentity = new ClaimsIdentity(claims);
            claimIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = Convert.FromBase64String(_configuration["AppSettings:Key"]);
            var sighingkey = new SymmetricSecurityKey(key);

            var token = new JwtSecurityToken(
                issuer: "http://oec.com",
                audience: "http://oec.com",
                expires: DateTime.UtcNow.AddHours(12),
                claims: claimIdentity.Claims,
                signingCredentials: new SigningCredentials(sighingkey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                me = new { user.Email, user.PhoneNumber, user.UserName, login.Id, login.FirstLastName }

            });
        }
    }
}