using ModelsRepository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthCareServiceApi.Services;
using ModelsRepository;

namespace HealthCareServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomControllerBase
    {
        public UserController(IServiceUnit serviceunit) : base(serviceunit)
        {
        }

        [HttpGet("Admins")]
        //[Authorize(Roles = "Administrator")]
        public IActionResult AdminsEndpoint()
        {
            return Ok($"Hi {CurrentUser?.Name}, you are an {CurrentUser?.Role}");
        }


        [HttpGet("Sellers")]
        [Authorize(Roles = "Seller")]
        public IActionResult SellersEndpoint()
        {
            return Ok($"Hi {CurrentUser.Name}, you are a {CurrentUser.Role}");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hi, you're on public property");
        }
       
    }
}
