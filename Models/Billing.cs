using System.ComponentModel.DataAnnotations;

namespace UniqueStudio.Models
{
    public class Billing
    {
        [Key]
        public int BillingId { get; set; }

        public int BookingId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public string PaymentStatus { get; set; }
    }
}