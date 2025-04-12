namespace ProjetInfo.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public int DriverID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string PlateNumber { get; set; }
        public string Color { get; set; }

        // Navigation property
        public Driver Driver { get; set; }
    }
}
