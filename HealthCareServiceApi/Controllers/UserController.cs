using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize]
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
