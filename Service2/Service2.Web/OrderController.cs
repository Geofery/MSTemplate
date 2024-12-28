using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMessageSession messageSession, ILogger<OrderController> logger)
        {
            _messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Places a new order and starts the order processing workflow.
        /// </summary>
        /// <param name="model">The order details.</param>
        /// <returns>An HTTP status indicating the result of the operation.</returns>
        [HttpPost("place_order", Name = "PlaceOrder")]
        [SwaggerOperation(Summary = "Starts a new order process.", Description = "Sends a PlaceOrder command to initiate order processing.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Order process started successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid order data.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDTO model)
        {
            if (model is null)
            {
                _logger.LogWarning("PlaceOrder request failed: Model is null.");
                return BadRequest("Order data cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PlaceOrder request failed: Invalid model state. Model: {Model}", model);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Processing PlaceOrder request. Model: {Model}", model);

            try
            {
                // Create and send the PlaceOrder command
                var command = CreatePlaceOrderCommand(model);

                await _messageSession.SendLocal(command).ConfigureAwait(false);

                _logger.LogInformation("PlaceOrder command sent successfully. Command: {Command}", command);

                return Accepted(new { Message = "Order process started successfully.", OrderId = command.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the PlaceOrder request.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Creates a PlaceOrder command from the provided OrderDTO.
        /// </summary>
        /// <param name="model">The order details.</param>
        /// <returns>A PlaceOrder command.</returns>
        private static PlaceOrder CreatePlaceOrderCommand(OrderDTO model)
        {
            return new PlaceOrder
            {
                OrderId = Guid.NewGuid(),
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Street = model.Street,
                City = model.City,
                PostalCode = model.PostalCode,
                Products = model.Products
            };
        }
    }
}
