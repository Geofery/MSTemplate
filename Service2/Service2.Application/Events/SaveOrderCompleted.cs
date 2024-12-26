using System;
using Domain.Models;

namespace Application.Events
{
    public class SaveOrderCompleted : IEvent
    {
        public Guid OrderId { get; set; }
        public List<Product> Products { get; set; }
    }
}

