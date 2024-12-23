using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Application.Commands;

namespace Web
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
        public async Task<IActionResult> SignupMember([FromBody] SignupDTO model)
        {
            try
            {
                if (model is null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Signup endpoint hit in UserController.");
                _logger.LogInformation($"THIS IS MY MESSAGE {model}");

                var command = new SignupCommand
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Street = model.AddressDTO.Street,
                    City = model.AddressDTO.City,
                    PostalCode = model.AddressDTO.PostalCode
                };

                await _messageSession.SendLocal(command).ConfigureAwait(false);
                _logger.LogInformation($"MessageFromService1 successfully send: {command}");

                return Accepted();
            }
            catch (Exception ex) {
                _logger.LogError($"Failed to publish message: {ex.Message}");
                return StatusCode(500);
            }

        }
    }
}
