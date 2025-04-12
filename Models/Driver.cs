using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjetInfo.Models
{
    public class Driver
    {
        [Key]
        public int DriverID { get; set; }

        public int UserID { get; set; } // Link to the User table (contains FullName, PhoneNumber, etc.)
        public string LicenseNumber { get; set; }
        public bool Availability { get; set; } = true;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<Ride> Rides { get; set; } = new List<Ride>();
        public virtual ICollection<RideFeedback> DriverFeedbacks { get; set; }

    }
}
