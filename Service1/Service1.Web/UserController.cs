using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SharedMessages;

namespace Service1.Web
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger<UserController> _logger;

        public UserController(IMessageSession messageSession, ILogger<UserController> logger)
        {
            _logger = logger;
            _messageSession = messageSession;
        }

        [HttpPost("Signup", Name = "Signup")]
        [SwaggerResponse(StatusCodes.Status201Created, "Signup process for new member started")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO model)
        {
            try
            {
                if (model is null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Signup endpoint hit in UserController.");
                _logger.LogInformation($"THIS IS MY MESSAGE {model}");

                await _messageSession.SendLocal(model).ConfigureAwait(false);
                _logger.LogInformation($"MessageFromService1 successfully send: {model}");

                return Accepted();
            }
            catch (Exception ex) {
                _logger.LogError($"Failed to publish message: {ex.Message}");
                return StatusCode(500);
            }

        }
    }
}
