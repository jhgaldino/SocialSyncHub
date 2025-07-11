using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers
{
    /// <summary>
    /// Controlador para verificar o status de saúde do UserService e do Gateway.
    /// </summary>
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Verifica se o UserService está funcionando.
        /// </summary>
        [HttpGet]
        public IActionResult Get() => Ok("UserService funcionando!");
        
        /// <summary>
        /// Verifica se o Gateway está funcionando.
        /// </summary>
        [HttpGet("gateway")]
        public IActionResult GetGatewayHealth() => Ok("Gateway funcionando!");
    }
}
