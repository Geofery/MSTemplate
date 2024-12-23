using System;
namespace Domain.Models
{
	public class Address
	{
		public Address(Guid id, string street, string city, string postalCode)
		{
			Id = id;
			Street = street;
			City = city;
			PostalCode = postalCode;
		}

		public Guid Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}

