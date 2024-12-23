using System;
namespace Application.Sagas;

public class OrderSagaData : ContainSagaData
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid PaymentId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public List<(Guid Ids, int Quantity)> Products { get; set; }
}

