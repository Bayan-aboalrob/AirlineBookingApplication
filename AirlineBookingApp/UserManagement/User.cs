using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.UserManagement
{
    public class User
    {
        private static int _nextUserId = 1;

        public int UserId { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Role UserRole { get; set; }
        public User(string username, string password, string email, Role userRole)
        {
            UserId = GenerateUniqueId();
            Username = username;
            Password = password;
            Email = email;
            UserRole = userRole;
        }

        private int GenerateUniqueId()
        {
            return _nextUserId++;
        }
    }
}
