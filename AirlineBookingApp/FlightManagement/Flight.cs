using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineBookingApp.FlightManagement
{
    public class Flight
    {
        public int FlightId { get; set; }
        public string DepartureCountry { get; set; }
        public string DestinationCountry { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public Dictionary<string, int> SeatAvailability { get; private set; }
        public Dictionary<string, decimal> PricePerSeatClass { get; private set; }

        public Flight(int flightId, string departureCountry, string destinationCountry, DateTime departureDate, DateTime arrivalDate, string departureAirport, string arrivalAirport)
        {
            FlightId = flightId;
            DepartureCountry = departureCountry;
            DestinationCountry = destinationCountry;
            DepartureDate = departureDate;
            ArrivalDate = arrivalDate;
            SeatAvailability = new Dictionary<string, int>();
            PricePerSeatClass = new Dictionary<string, decimal>();
            DepartureAirport = departureAirport;
            ArrivalAirport = arrivalAirport;
        }

        public void AddSeatClass(string seatClass, int numberOfSeats, decimal pricePerSeat)
        {
            SeatAvailability.Add(seatClass, numberOfSeats);
            PricePerSeatClass.Add(seatClass, pricePerSeat);
        }
    }
}
