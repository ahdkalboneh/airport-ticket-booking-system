using AirportTicketBookingSystem.Enums;
public class BookingsManager
{
    private readonly JsonFileStorage<Booking> Storage;
    private List<Booking> Bookings;
    private readonly FlightManager FlightManager;
    public BookingsManager(JsonFileStorage<Booking> storage, List<Booking> bookings, FlightManager flightManager)
    {
        Storage = storage;
        Bookings = bookings;
        FlightManager = flightManager;
    }
    public async Task<bool> CreateBooking(int userId, int flightId, CabinClass cabinClass)
    {
        Flight? flight = FlightManager.GetFlightById(flightId);
        if (flight == null)
            return false;

        if (!flight.Prices.TryGetValue(cabinClass, out decimal price))
            return false;

        int newBookingId = Bookings.Count == 0 ? 1 : Bookings.Max(booking => booking.BookingId) + 1;
        Booking newBooking = new Booking(newBookingId, userId, flightId, cabinClass, price, DateTime.Now);
        Bookings.Add(newBooking);
        await Storage.WriteFile(Bookings);
        return true;
    }

}