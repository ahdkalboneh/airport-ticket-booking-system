
using AirportTicketBookingSystem.Enums;
public class FlightSearchCriteria
{
    public string? DepartureCountry { get; set; }
    public string? DestinationCountry { get; set; }
    public DateTime? DepartureDate { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? DepartureAirport { get; set; }
    public string? ArrivalAirport { get; set; }
    public CabinClass? CabinClass { get; set; }
}
