using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;

namespace AirlineBookingApp.UserManagement
{
    public class UserManager
    {
        private List<User> _users;

        public UserManager(List<User> users) => _users = users;

        public User CreateUser(string username, string password, string email, Role role)
        {
            ValidateUsername(username);
            ValidatePassword(password);
            ValidateEmail(email);

            if (_users.Any(u => u.Username == username))
                throw new Exception("User already exists");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            User newUser = new User(username, hashedPassword, email, role);
            _users.Add(newUser);
            return newUser;
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length < 8)
                throw new ArgumentException("Username must be at least 8 characters long.");
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.");

            if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                throw new ArgumentException("Password must contain at least one letter and one number.");
        }

        private void ValidateEmail(string email)
        {
            // Simple regex for basic email validation
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(email))
                throw new ArgumentException("Email is not in a valid format.");
        }

        public void DeleteUser(int userId)
        {
            var user = _users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _users.Remove(user);
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public void UpdateUser(int userId, string username, string email, string? password = null)
        {
            ValidateUsername(username);
            ValidateEmail(email);
            if (password != null)
                ValidatePassword(password);

            var user = _users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.Username = username;
                user.Email = email;
                if (!string.IsNullOrEmpty(password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                }
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }
    }
}
