using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AirlineBookingApp.UserManagement
{
    public class Passanger : User

    {
        private string? passengerName;
        private string? passportNumber;
        private string? passengerAddress;
        private string? nationality;
        private Gender gender;
        private string? phoneNumber;

        public enum Gender
        {
            Male,
            Female,
            Other
        }


        public string? PassengerName
        {
            get { return passengerName; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 50 && char.IsLetter(value[0]))
                {
                    passengerName = value;
                }
                else
                {
                    throw new ArgumentException("Passenger name must start with a character and cannot exceed 50 characters.");
                }
            }
        }


        public string? PassportNumber
        {
            get { return passportNumber; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 20 && value.All(char.IsLetterOrDigit))
                {
                    passportNumber = value;
                }
                else
                {
                    throw new ArgumentException("Passport number must be alphanumeric and cannot exceed 20 characters.");
                }
            }
        }


        public string? PassengerAddress
        {
            get { return passengerAddress; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 100)
                {
                    passengerAddress = value;
                }
                else
                {
                    throw new ArgumentException("Passenger address cannot be null or empty and must not exceed 100 characters.");
                }
            }
        }

        public string? Nationality
        {
            get { return nationality; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 50)
                {
                    nationality = value;
                }
                else
                {
                    throw new ArgumentException("Nationality cannot be null or empty and must not exceed 50 characters.");
                }
            }
        }

        public Gender PassengerGender
        {
            get { return gender; }
            set { gender = value; }
        }
        public string? PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                string pattern = @"^\+?\d{1,3}[-. ]?\(?\d{3}\)?[-. ]?\d{3}[-. ]?\d{4}$";

                // To Check if the provided value matches the regex pattern
                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, pattern))
                {
                    phoneNumber = value;
                }
                else
                {
                    throw new ArgumentException("Phone number must match the required pattern.");
                }
            }
        }

        public Passanger(string username,
        string password,
                         string email,
                         Role role,
                         string passengerName,
                         string passengerAddress,
                         string nationality,
                         Gender gender,
                         string phoneNumber)
     : base(username, password, email, role)
        {
            PassengerName = passengerName;
            PassengerAddress = passengerAddress;
            Nationality = nationality;
            PassengerGender = gender;
            PhoneNumber = phoneNumber;
        }

    }
}
