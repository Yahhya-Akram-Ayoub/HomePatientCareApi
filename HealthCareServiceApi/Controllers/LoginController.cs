using ModelsRepository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ModelsRepository;
using HealthCareServiceApi.Services;
using Microsoft.AspNetCore.Cors;

namespace HealthCareServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : CustomControllerBase
    {
        private readonly JWTServices _JWTService;
        public LoginController(IConfiguration config, IServiceUnit serviceunit) : base(serviceunit)
        {
            _JWTService = new JWTServices(config, ServiceUnit);
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [EnableCors("_myAllowSpecificOrigins")]
        public IActionResult Login([FromForm] string Email, [FromForm] string Password)
        {
            try
            {
                User user = _JWTService.ChechAuthenticate(new User() { Email = Email, Password = Password });

                if (user != null)
                {
                    string token = _JWTService.GenerateToken(user);
                    return Ok(new JsonResult(new { token, user }));
                }
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }

            return NotFound("User not found");
        }

        [HttpPost]
        [Route("Register")]
        [EnableCors("_myAllowSpecificOrigins")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                User _user = ServiceUnit.Users.Add(user);

                if (_user != null)
                {
                    var token = _JWTService.GenerateToken(user);
                    return Ok(token);
                }
            }
            catch (Exception e)
            {
                return NotFound();
            }

            return NotFound("User not found");
        }
    }
}
