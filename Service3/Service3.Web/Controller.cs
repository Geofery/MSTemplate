using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetValues()
        {
            return Ok(new[] { "Service3 - Value1", "Service3 - Value2" });
        }

        [HttpGet("{id}")]
        public IActionResult GetValueById(int id)
        {
            return Ok($"Service1 - Value{id}");
        }
    }
}
