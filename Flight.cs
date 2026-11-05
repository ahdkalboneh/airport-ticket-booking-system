using System.ComponentModel.DataAnnotations;
namespace AirportTicketBookingSystem.Enums;
public enum CabinClass {FirstClass, BusinessClass, EconomyClass}
public class Flight
{
    public int FlightId {get; private set;}
    [Required]
    public string DepartureCountry {get; private set;}
    [Required]
    public string DestinationCountry {get; private set;}
    [Required]
    [FutureDate]
    public DateTime DepartureDate {get; private set;}
    [Required]
    public string DepartureAirport {get; private set;}
    [Required]
    public string ArrivalAirport {get; private set;}
    [Required]
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