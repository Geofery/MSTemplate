using System;
namespace Application.Commands
{
	public class CancelOrder : ICommand
	{
		public Guid OrderId { get; set; }
		public Guid PaymentId { get; set; }
		public string Reason { get; set; }
	}
}

