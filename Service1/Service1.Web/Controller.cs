using Microsoft.AspNetCore.Mvc;

namespace Service1.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetValues()
        {
            return Ok(new[] { "Service1 - Value1", "Service1 - Value2" });
        }

        [HttpGet("{id}")]
        public IActionResult GetValueById(int id)
        {
            return Ok($"Service1 - Value{id}");
        }
    }
}
