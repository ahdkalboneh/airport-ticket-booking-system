using AirportTicketBookingSystem.Enums;
public class FlightManager
{

    private readonly List<Flight> Flights;

    public FlightManager(List<Flight> flights)
    {
        Flights = flights;
    }

    public List<Flight> SearchFlights(string departureCountry, string destinationCountry, DateTime departureDate)
    {
        return Flights.Where(flight =>
            string.Equals(flight.DepartureCountry, departureCountry, StringComparison.OrdinalIgnoreCase)
            && string.Equals(flight.DestinationCountry, destinationCountry, StringComparison.OrdinalIgnoreCase)
            && flight.DepartureDate.Date == departureDate.Date
        ).ToList();
    }

    public void DisplayFlights(List<Flight> flights)
    {

        if (!flights.Any())
        {
            Console.WriteLine("No available flights.");
            return;
        }

        foreach (var flight in flights)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine($"Flight ID: {flight.FlightId}");
            Console.WriteLine($"From: {flight.DepartureCountry}");
            Console.WriteLine($"To: {flight.DestinationCountry}");
            Console.WriteLine($"Departure Date: {flight.DepartureDate}");
            Console.WriteLine($"Departure Airport: {flight.DepartureAirport}");
            Console.WriteLine($"Arrival Airport: {flight.ArrivalAirport}");

            Console.WriteLine("Prices:");

            foreach (var price in flight.Prices)
                Console.WriteLine($"{price.Key}: {price.Value}");
        }
        Console.WriteLine("-------------------------");
    }

    public Flight? GetFlightById(int flightId)
    {
        return Flights.FirstOrDefault(flight => flight.FlightId == flightId);
    }
}