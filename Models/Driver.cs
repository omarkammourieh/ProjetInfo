using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetInfo.Models
{
    public class Driver
    {
        [Key]
        public int DriverID { get; set; }
        [Required]
        [StringLength(100)]
        public int Name { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public int Phone { get; set; }
        [Required]
        [StringLength(100)]
        public int Vehicle { get; set; }
        [Required]
        [StringLength(100)]
        public int Plate { get; set; }
        [Required]
        [StringLength(100)]
        public int Rating { get; set; }

        public string Email { get; set; }

        [Required]
        [StringLength(20)]

        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }




        public int UserID { get; set; } // Link to the User table (contains FullName, PhoneNumber, etc.)
        public string LicenseNumber { get; set; }
        public bool Availability { get; set; } = true;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime ScheduledDateTime { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Ride> Rides { get; set; } = new List<Ride>();
        public virtual ICollection<RideFeedback> DriverFeedbacks { get; set; }

    }
}
