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
        bool alreadyBooked = Bookings.Any(booking => booking.UserId == userId && booking.FlightId == flightId);

        if (alreadyBooked)
            return false;

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

    public void PrintUserBookings(int userId)
    {
        List<Booking> userBookings = Bookings.Where(booking => booking.UserId == userId).ToList();

        if (userBookings.Count == 0)
        {
            Console.WriteLine("You have no bookings yet.");
            return;
        }

        foreach (var booking in userBookings)
        {
            Flight? flight = FlightManager.GetFlightById(booking.FlightId);

            if (flight == null)
                continue;

            Console.WriteLine("------------------------");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Flight ID: {flight.FlightId}");
            Console.WriteLine($"From: {flight.DepartureCountry}");
            Console.WriteLine($"To: {flight.DestinationCountry}");
            Console.WriteLine($"Departure Date: {flight.DepartureDate}");
            Console.WriteLine($"Cabin Class: {booking.CabinClass}");
            Console.WriteLine($"Price: {booking.Price}");
            Console.WriteLine($"Booking Date: {booking.BookingDate}");
        }
        Console.WriteLine("------------------------");

    }

    public async Task<bool> CancelBooking(int userId, int bookingId)
    {
        Booking? booking = Bookings.FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);

        if (booking == null)
            return false;

        Bookings.Remove(booking);

        await Storage.WriteFile(Bookings);
        return true;
    }

    public async Task<bool> UpdateBooking(int userId, int bookingId, CabinClass newCabinClass)
    {
        Booking? booking = Bookings.FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);

        if (booking == null)
            return false;

        Flight? flight = FlightManager.GetFlightById(booking.FlightId);

        if (flight == null)
            return false;

        if (!flight.Prices.TryGetValue(newCabinClass, out decimal newPrice))
            return false;

        booking.UpdateCabinClass(newCabinClass, newPrice);

        await Storage.WriteFile(Bookings);

        return true;
    }


    public List<Booking> SearchBookings(BookingSearchCriteria criteria)
    {
        return Bookings.Where(booking =>
        {
            if (criteria.PassengerId != null && booking.UserId != criteria.PassengerId.Value)
                return false;

            if (criteria.FlightId != null && booking.FlightId != criteria.FlightId.Value)
                return false;

            if (criteria.CabinClass != null && booking.CabinClass != criteria.CabinClass.Value)
                return false;

            if (criteria.MaxPrice != null && booking.Price > criteria.MaxPrice.Value)
                return false;

            Flight? flight = FlightManager.GetFlightById(booking.FlightId);

            if (flight == null)
                return false;

            if (!string.IsNullOrWhiteSpace(criteria.DepartureCountry) &&
                !string.Equals(flight.DepartureCountry, criteria.DepartureCountry, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrWhiteSpace(criteria.DestinationCountry) &&
                !string.Equals(flight.DestinationCountry, criteria.DestinationCountry, StringComparison.OrdinalIgnoreCase))
                return false;

            if (criteria.DepartureDate != null && flight.DepartureDate.Date != criteria.DepartureDate.Value.Date)
                return false;

            if (!string.IsNullOrWhiteSpace(criteria.DepartureAirport) &&
                !string.Equals(flight.DepartureAirport, criteria.DepartureAirport, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrWhiteSpace(criteria.ArrivalAirport) &&
                !string.Equals(flight.ArrivalAirport, criteria.ArrivalAirport, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }).ToList();
    }


    public void DisplayBookings(List<Booking> bookings)
    {
        if (!bookings.Any())
        {
            Console.WriteLine("No bookings found.");
            return;
        }

        foreach (var booking in bookings)
        {
            Flight? flight = FlightManager.GetFlightById(booking.FlightId);

            if (flight == null)
                continue;

            Console.WriteLine("-------------------------");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Passenger ID: {booking.UserId}");
            Console.WriteLine($"Flight ID: {booking.FlightId}");
            Console.WriteLine($"From: {flight.DepartureCountry}");
            Console.WriteLine($"To: {flight.DestinationCountry}");
            Console.WriteLine($"Departure Date: {flight.DepartureDate}");
            Console.WriteLine($"Departure Airport: {flight.DepartureAirport}");
            Console.WriteLine($"Arrival Airport: {flight.ArrivalAirport}");
            Console.WriteLine($"Cabin Class: {booking.CabinClass}");
            Console.WriteLine($"Price: {booking.Price}");
            Console.WriteLine($"Booking Date: {booking.BookingDate}");
        }

        Console.WriteLine("-------------------------");
    }


}