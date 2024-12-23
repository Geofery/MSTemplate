﻿
namespace Application.Commands
{
	public class SaveOrder : ICommand
	{
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public List<(Guid Ids, int Quantity)> Products { get; set; }

    }
}
