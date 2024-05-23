using AirlineBookingApp.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.BookManagement
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public Passanger owner { get; set; }
        public int NumSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; }

        public Booking(int flightId, Passanger passenger, int numSeats, decimal totalPrice)
        {
            FlightId = flightId;
            owner = passenger;
            NumSeats = numSeats;
            TotalPrice = totalPrice;
            BookingDate = DateTime.Now;
            Status = BookingStatus.Pending;
        }

        // Methods to update the status of the booking
        public void ConfirmBooking()
        {
            Status = BookingStatus.Confirmed;
        }

        public void CancelBooking()
        {
            Status = BookingStatus.Cancelled;
        }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        modified,
    }
}
