using Domain.Models;
using SharedMessages;

namespace Web
{
    public class OrderDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public List<ProductDTO> Products { get; set; }
    }
}