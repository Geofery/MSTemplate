using System;
namespace Web
{
	public class AddressDTO
	{
		public AddressDTO(string street, string city, string postalCode)
		{
			Street = street;
			City = city;
			PostalCode = postalCode;
		}

        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}

