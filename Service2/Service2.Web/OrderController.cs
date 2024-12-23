﻿using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Swashbuckle.AspNetCore.Annotations;

namespace Web
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger<OrderController> _logger;


        public OrderController(IMessageSession messageSession, ILogger<OrderController> logger)
        {
            _logger = logger;
            _messageSession = messageSession;
        }

        [HttpPost("place_order", Name = "Place order")]
        [SwaggerResponse(StatusCodes.Status201Created, "Order process for new order started")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDTO model)
        {
            try
            {
                if (model is null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("PlaceOrder endpoint hit in OrderController.");
                _logger.LogInformation($"THIS IS MY MESSAGE {model}");

                var command = new PlaceOrder
                {
                    OrderId = new Guid(),
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Street = model.Street,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Products = model.Products
                };
     

                await _messageSession.SendLocal(command).ConfigureAwait(false);
                _logger.LogInformation($"MessageFromService1 successfully send: {command}");

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish message: {ex.Message}");
                return StatusCode(500);
            }

        }
    }
}
