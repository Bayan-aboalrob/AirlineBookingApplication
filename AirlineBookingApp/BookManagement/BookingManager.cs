using AirlineBookingApp.DataManagement;
using AirlineBookingApp.FlightManagement;
using AirlineBookingApp.UserManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.BookManagement
{
    internal class BookingManager : UserManagement.Manager
    {
        private FlightManager flightManager;

        public BookingManager(string username, string password, string email, Role role, string department, FlightManager flightManager)
            : base(username, password, email, role, department)
        {
            this.flightManager = flightManager;
        }

        public bool BookFlight(int flightId, string seatClass, int numSeats)
        {

            if (flightManager.UpdateSeatAvailability(flightId, seatClass, numSeats))
            {
                decimal? pricePerSeat = flightManager.GetPricePerSeat(flightId, seatClass);
                if (pricePerSeat != null)
                {

                    decimal totalCost = pricePerSeat.Value * numSeats;

                    if (ProcessPayment(totalCost))
                    {

                        GenerateBookingRecord(flightId, seatClass, numSeats, totalCost);

                        SendBookingConfirmation();

                        return true;
                    }
                }
            }
            return false;
        }

        private bool ProcessPayment(decimal amount)
        {

            Console.WriteLine($"Processing payment of {amount:C}");
            return true;
        }

        private void GenerateBookingRecord(int flightId, string seatClass, int numSeats, decimal totalCost)
        {
            Console.WriteLine($"Booking record created for Flight ID: {flightId}, Seat Class: {seatClass}, Number of Seats: {numSeats}, Total Cost: {totalCost:C}");
        }

        private void SendBookingConfirmation()
        {
            Console.WriteLine("Booking confirmation sent to the customer.");
        }

        public override void GenerateReports()
        {

            Console.WriteLine("Generating reports related to bookings.");
        }

        // Delete a specific booking
        public bool DeleteBooking(int bookingId)
        {
            string csvFilePath = "bookings.csv";

            try
            {
                // Here I am reading  the current bookings from the CSV file
                var bookings = ReadBookingsFromCsv(csvFilePath);
                var bookingToDelete = bookings.FirstOrDefault(b => b.BookingId == bookingId);

                if (bookingToDelete == null)
                {
                    Console.WriteLine($"No booking found with ID: {bookingId}");
                    return false;
                }

                bookings.Remove(bookingToDelete);

                // here I am writting the updated list back to the CSV bookings file
                WriteBookingsToCsv(bookings, csvFilePath);
                Console.WriteLine($"Successfully deleted booking ID: {bookingId}");
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error accessing the bookings file: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        private void WriteBookingsToCsv(List<Booking> bookings, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("BookingId,FlightId,UserId,NumSeats,TotalPrice,BookingDate,Status");
                foreach (var booking in bookings)
                {
                    writer.WriteLine($"{booking.BookingId},{booking.FlightId},{booking.owner.UserId},{booking.NumSeats},{booking.TotalPrice.ToString(CultureInfo.InvariantCulture)},{booking.BookingDate.ToString("yyyy-MM-dd")},{booking.Status}");
                }
            }
        }


        public bool ModifyBooking(int bookingId, string newSeatClass, int newNumSeats)
        {
            string csvFilePath = "bookings.csv";
            var csvImporter = new CsvImporter(csvFilePath);
            var bookings = csvImporter.ReadBookingsFromCsv();
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                Console.WriteLine("Booking not found.");
                return false;
            }
            var flight = flightManager.GetFlightById(booking.FlightId);
            if (flight == null)
            {
                Console.WriteLine("Flight not found.");
                return false;
            }

            decimal? newPricePerSeat = flightManager.GetPricePerSeat(flight.FlightId, newSeatClass);
            if (newPricePerSeat == null)
            {
                Console.WriteLine("Price for new seat class not found.");
                return false;
            }

            booking.NumSeats = newNumSeats;
            booking.TotalPrice = newNumSeats * newPricePerSeat.Value;
            booking.Status = BookingStatus.modified;

            WriteBookingsToCsv(bookings, csvFilePath);

            Console.WriteLine("Booking modified successfully.");
            return true;
        }
        private decimal CalculatePriceForBooking(int flightId, string seatClass, int numSeats)
        {
            var flight = GetFlightById(flightId);
            if (flight == null)
            {
                throw new InvalidOperationException("Flight not found");
            }

            decimal basePrice = flight.PricePerSeatClass[seatClass];
            decimal classMultiplier = 1.0M;

            switch (seatClass)
            {
                case "Business":
                    classMultiplier = 1.5M;
                    break;
                case "First":
                    classMultiplier = 2.0M;
                    break;
                case "Economy":
                    classMultiplier = 1.0M;
                    break;
                default:
                    throw new ArgumentException("Invalid seat class");
            }

            return basePrice * classMultiplier * numSeats;
        }

        public Flight GetFlightById(int flightId)
        {
            return flightManager.GetFlightById(flightId);
        }


        public List<Booking> ReadBookingsFromCsv(string flightCsvFilePath)
        {
            var csvImporter1 = new CsvImporter(flightCsvFilePath);
            return csvImporter1.ReadBookingsFromCsv();
        }
        public List<Booking> ViewBookingsDetailsForPassenger(string passengerId, string csvFilePath)
        {

            var allBookings = ReadBookingsFromCsv(csvFilePath);

            var passengerBookings = allBookings.Where(booking => booking.owner.UserId == int.Parse(passengerId)).ToList();

            foreach (var booking in passengerBookings)
            {
                Console.WriteLine($"Booking ID: {booking.BookingId}, Flight ID: {booking.FlightId}, Passenger ID: {booking.owner.UserId}, Number of Seats: {booking.NumSeats}");
            }

            return passengerBookings;
        }

        public List<Booking> FilterBookings(BookingFilter filter)
        {
            var csvFilePath = "bookings.csv";
            var csvImporter = new CsvImporter(csvFilePath);
            var bookings = csvImporter.ReadBookingsFromCsv();

            return bookings.Where(b =>
                (!filter.FlightId.HasValue || b.FlightId == filter.FlightId) &&
                (!filter.MaxPrice.HasValue || b.TotalPrice <= filter.MaxPrice) &&
                (string.IsNullOrEmpty(filter.DepartureCountry) || flightManager.GetFlightById(b.FlightId).DepartureCountry == filter.DepartureCountry) &&
                (string.IsNullOrEmpty(filter.DestinationCountry) || flightManager.GetFlightById(b.FlightId).DestinationCountry == filter.DestinationCountry) &&
                (!filter.DepartureDate.HasValue || flightManager.GetFlightById(b.FlightId).DepartureDate.Date == filter.DepartureDate.Value.Date) &&
                (string.IsNullOrEmpty(filter.DepartureAirport)) &&
                (string.IsNullOrEmpty(filter.ArrivalAirport) || flightManager.GetFlightById(b.FlightId).ArrivalAirport == filter.ArrivalAirport) &&
                (!filter.PassengerId.HasValue || b.owner.UserId == filter.PassengerId) &&
                (string.IsNullOrEmpty(filter.Class) || flightManager.GetFlightById(b.FlightId).PricePerSeatClass.ContainsKey(filter.Class))
            ).ToList();
        }


    }

}
