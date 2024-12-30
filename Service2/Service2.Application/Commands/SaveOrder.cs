
using Domain.Models;

namespace Application.Commands
{
	public class SaveOrder : ICommand
	{
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}

