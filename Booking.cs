using AirportTicketBookingSystem.Enums;
public class Booking
{
    public int BookingId { get; private set; }
    public int UserId { get; private set; }
    public int FlightId { get; private set; }
    public CabinClass CabinClass { get; private set; }
    public decimal Price { get; private set; }
    public DateTime BookingDate { get; private set; }

    public Booking(int bookingId, int userId, int flightId, CabinClass cabinClass, decimal price, DateTime bookingDate)
    {
        BookingId = bookingId;
        UserId = userId;
        FlightId = flightId;
        CabinClass = cabinClass;
        Price = price;
        BookingDate = bookingDate;
    }

    public void UpdateCabinClass(CabinClass cabinClass, decimal price)
    {
        CabinClass = cabinClass;
        Price = price;
    }
}