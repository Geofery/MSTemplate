using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SharedMessages;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger<UserController> _logger;

        public UserController(IMessageSession messageSession, ILogger<UserController> logger)
        {
            _messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts the signup process for a new user.
        /// </summary>
        /// <param name="model">The user signup details.</param>
        /// <returns>An HTTP status indicating the result of the operation.</returns>
        [HttpPost("signup", Name = "Signup")]
        [SwaggerOperation(Summary = "Starts the signup process for a new user.", Description = "Sends a SignupCommand to initiate user signup.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Signup process started successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid signup data.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")]
        public async Task<IActionResult> SignupMember([FromBody] SignupDTO model)
        {
            if (model is null)
            {
                _logger.LogWarning("Signup request failed: Model is null.");
                return BadRequest("Signup data cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Signup request failed: Invalid model state. Model: {Model}", model);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Processing SignupMember request. Model: {Model}", model);

            try
            {
                // Create and send the SignupCommand
                var command = CreateSignupCommand(model);

                await _messageSession.SendLocal(command).ConfigureAwait(false);

                _logger.LogInformation("SignupCommand sent successfully. Command: {Command}", command);

                return Accepted(new { Message = "Signup process started successfully.", Email = command.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the SignupMember request.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Creates a SignupCommand from the provided SignupDTO.
        /// </summary>
        /// <param name="model">The signup details.</param>
        /// <returns>A SignupCommand.</returns>
        private static SignupCommand CreateSignupCommand(SignupDTO model)
        {
            return new SignupCommand
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Street = model.AddressDTO.Street,
                City = model.AddressDTO.City,
                PostalCode = model.AddressDTO.PostalCode
            };
        }
    }
}
