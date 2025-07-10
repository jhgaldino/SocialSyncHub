using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("UserService funcionando!");
    }
}
