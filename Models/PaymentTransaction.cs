using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetInfo.Models
{
    public class PaymentTransaction
    {
        public int TransactionID { get; set; }
        public int PaymentID { get; set; }
        public int UserID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Status { get; set; }

        // Navigation properties
        public User User { get; set; }
        public  ICollection<Ride> Rides { get; set; }
    }
}
