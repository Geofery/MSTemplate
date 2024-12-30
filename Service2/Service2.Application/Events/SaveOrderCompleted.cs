using System;
using Domain.Models;

namespace Application.Events
{
    public class SaveOrderCompleted : IEvent
    {
        public Guid OrderId { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}

