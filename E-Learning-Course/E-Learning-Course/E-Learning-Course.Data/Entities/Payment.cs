using E_Learning_Course.Data.Entities;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionId { get; set; }
    public bool IsSuccessful { get; set; }

    // Foreign Keys
    public int CourseId { get; set; }
    public Course Course { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    // Additional properties if needed
    public string PaymentMethod { get; set; }
    public int Status { get; set; }
}
