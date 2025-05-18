namespace ProjetInfo
{
    public class ConnectedUsers
    {
        public static List<string> myConnectedUsers = new List<string>();

        public static void AddUser(string connectionId)
        {
            if (!myConnectedUsers.Contains(connectionId))
            {
                myConnectedUsers.Add(connectionId);
            }
        }

        public static void RemoveUser(string connectionId)
        {
            if (myConnectedUsers.Contains(connectionId))
            {
                myConnectedUsers.Remove(connectionId);
            }
        }

        public static List<string> GetConnectedUsers()
        {
            return myConnectedUsers;
        }
    }
}
