using AirportTicketBookingSystem.Enums;
public class FlightManager
{

    private readonly List<Flight> Flights;

    public FlightManager(List<Flight> flights)
    {
        Flights = flights;
    }

    public List<Flight> SearchFlights(FlightSearchCriteria criteria)
    {
        return Flights.Where(flight =>
            string.Equals(flight.DepartureCountry, criteria.DepartureCountry, StringComparison.OrdinalIgnoreCase)
            && string.Equals(flight.DestinationCountry, criteria.DestinationCountry, StringComparison.OrdinalIgnoreCase)
            && (criteria.DepartureDate == null || flight.DepartureDate.Date == criteria.DepartureDate.Value.Date)
            && (string.IsNullOrWhiteSpace(criteria.DepartureAirport) ||
            string.Equals(flight.DepartureAirport, criteria.DepartureAirport, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrWhiteSpace(criteria.ArrivalAirport) ||
            string.Equals(flight.ArrivalAirport, criteria.ArrivalAirport, StringComparison.OrdinalIgnoreCase))
            && (criteria.CabinClass == null || flight.Prices.ContainsKey(criteria.CabinClass.Value))
            && (criteria.MaxPrice == null || (criteria.CabinClass != null ? flight.Prices.TryGetValue(criteria.CabinClass.Value, out decimal price)
            && price <= criteria.MaxPrice.Value : flight.Prices.Values.Any(price => price <= criteria.MaxPrice.Value)))
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