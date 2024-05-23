using AirlineBookingApp.DataManagement;
using AirlineBookingApp.UserManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.FlightManagement
{
    internal class FlightManager : UserManagement.Manager
    {
        private List<Flight> flights;

        public FlightManager(string username, string password, string email, Role role, string deparment, List<Flight> flights) : base(username, password, email, role, deparment)
        {
            this.flights = flights;
        }
        public void AddFlight(Flight flight)
        {
            flights.Add(flight);
            Console.WriteLine("The flight has been added successfully!");
        }
        public void RemoveFlight(Flight flight)
        {
            var flightToRemove = flights.Find(targetFlight => targetFlight.FlightId == flight.FlightId);
            if (flightToRemove != null)
            {
                flights.Remove(flightToRemove);
                Console.WriteLine("The flight has been deleted successfully!");
            }
            else
            {
                Console.WriteLine("Flight not Found");
            }
        }

        public bool UpdateSeatAvailability(int flightId, string seatClass, int numSeatsToBook)
        {
            var flight = flights.FirstOrDefault(f => f.FlightId == flightId);
            if (flight != null && flight.SeatAvailability.ContainsKey(seatClass))
            {
                int currentSeats = flight.SeatAvailability[seatClass];
                if (currentSeats >= numSeatsToBook)
                {
                    flight.SeatAvailability[seatClass] = currentSeats - numSeatsToBook;
                    return true;
                }
            }
            return false;
        }

        public decimal? GetPricePerSeat(int flightId, string seatClass)
        {
            var flight = flights.FirstOrDefault(f => f.FlightId == flightId);
            if (flight != null && flight.PricePerSeatClass.ContainsKey(seatClass))
            {
                return flight.PricePerSeatClass[seatClass];
            }
            return null;
        }
        // I have to implement this
        public override void GenerateReports()
        {
            throw new NotImplementedException();
        }

        public List<Flight> ReadFlightsFromCsv(string flightCsvFilePath)
        {
            var csvImporter = new CsvImporter(flightCsvFilePath);
            return csvImporter.ReadFlightsFromCsv();
        }
        bool Validate(Flight data, out IEnumerable<string> validationErrors)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(data.DepartureCountry))
                errors.Add("Departure country is required.");
            if (string.IsNullOrWhiteSpace(data.DestinationCountry))
                errors.Add("Destination country is required.");
            if (data.DepartureDate < DateTime.Now)
                errors.Add("Departure date must be in the future.");
            if (data.ArrivalDate <= data.DepartureDate)
                errors.Add("Arrival date must be after the departure date.");


            if (data.SeatAvailability == null)
                errors.Add("At least one class of seats must be defined.");
            if (data.PricePerSeatClass == null)
                errors.Add("Pricing must be defined for at least one class of seats.");


            validationErrors = errors;
            return !errors.Any();
        }
        public void LoadAndValidateFlights(string flightCsvFilePath)
        {
            var flightsFromFile = ReadFlightsFromCsv(flightCsvFilePath);
            foreach (var flight in flightsFromFile)
            {
                if (Validate(flight, out IEnumerable<string> validationErrors))
                {
                    AddFlight(flight);
                }
                else
                {
                    Console.WriteLine($"Failed to validate flight {flight.FlightId}: {string.Join(", ", validationErrors)}");
                }
            }
        }

        public Flight? GetFlightById(int flightId)
        {
            string csvFilePath = "flights.csv";

            CsvImporter csvImporter = new CsvImporter(csvFilePath);

            try
            {

                var flights = csvImporter.ReadFlightsFromCsv();

                var flight = flights.FirstOrDefault(f => f.FlightId == flightId);
                return flight;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error retrieving flight data: {ex.Message}");
                return null;
            }
        }

        public List<Flight> SearchAvailableFlights(FlightSearchCriteria criteria)
        {
            return flights.Where(f =>
                (!criteria.MaxPrice.HasValue || (f.PricePerSeatClass.ContainsKey(criteria.Class) && f.PricePerSeatClass[criteria.Class] <= criteria.MaxPrice)) &&
                (string.IsNullOrEmpty(criteria.DepartureCountry) || f.DepartureCountry == criteria.DepartureCountry) &&
                (string.IsNullOrEmpty(criteria.DestinationCountry) || f.DestinationCountry == criteria.DestinationCountry) &&
                (!criteria.DepartureDate.HasValue || f.DepartureDate.Date == criteria.DepartureDate.Value.Date) &&
                (string.IsNullOrEmpty(criteria.DepartureAirport) || f.DepartureAirport == criteria.DepartureAirport) &&
                (string.IsNullOrEmpty(criteria.ArrivalAirport) || f.ArrivalAirport == criteria.ArrivalAirport) &&
                (string.IsNullOrEmpty(criteria.Class) || f.SeatAvailability.ContainsKey(criteria.Class))
            ).ToList();
        }

    }
}
