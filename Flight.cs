namespace AirportTicketBookingSystem.Enums;
public enum CabinClass {FirstClass, BusinessClass, EconomyClass}
public class Flight
{
    public int FlightId {get; private set;}
    public string DepartureCountry {get; private set;}
    public string DestinationCountry {get; private set;}
    public DateTime DepartureDate {get; private set;}
    public string DepartureAirport {get; private set;}
    public string ArrivalAirport {get; private set;}
    public Dictionary<CabinClass, decimal> Prices {get; private set;} = 
    new Dictionary<CabinClass, decimal>();

    public Flight(int flightId, string departureCountry, string destinationCountry,
        DateTime departureDate, string departureAirport, string arrivalAirport, Dictionary<CabinClass, decimal> prices)
    {
        FlightId = flightId;
        DepartureCountry = departureCountry;
        DestinationCountry = destinationCountry;
        DepartureDate = departureDate;
        DepartureAirport = departureAirport;
        ArrivalAirport = arrivalAirport;
        Prices = prices;
    }
}