﻿namespace ProjetInfo.Models
{
    public class Ride
    {
        public int RideID { get; set; }
        public int UserID { get; set; }
        public int? DriverID { get; set; }
        public string PickupLocation { get; set; } // Added this property
        public string DropOffLocation { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PaymentTransactionID { get; set; }
        public string Status { get; set; }
        // Navigation properties
        public User User { get; set; }
        public Driver Driver { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
        public ICollection<RideFeedback> RideFeedbacks { get; set; }
        public DateTime RideDateTime { get; set; }

    }

}
