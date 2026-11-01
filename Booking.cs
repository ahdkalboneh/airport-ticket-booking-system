using AirportTicketBookingSystem.Enums;
public class Booking
{
    public int BookingId {get; private set;}
    public int UserId {get; private set;}
    public int FlightId {get; private set;}
    public CabinClass CabinClass { get; private set; }
    public decimal Price { get; private set; }
    public DateTime BookingDate { get; private set; }

}