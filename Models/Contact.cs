using System.ComponentModel.DataAnnotations;

namespace ProjetInfo.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
