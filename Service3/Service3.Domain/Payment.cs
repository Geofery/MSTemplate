namespace Domain;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } // e.g., "Processed", "Failed"
    public string Reason { get; set; }
}
