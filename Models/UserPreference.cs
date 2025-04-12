namespace ProjetInfo.Models
{
    public class UserPreferences
    {
        public int PreferenceID { get; set; }
        public int UserID { get; set; }
        public string PreferredPayment { get; set; }
        public string PreferredRideType { get; set; }

        // Navigation property
        public  User User { get; set; }
    }
}
