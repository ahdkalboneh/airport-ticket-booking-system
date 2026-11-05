using AirportTicketBookingSystem.Enums;

public class Program
{
    public static async Task Main(string[] args)
    {
        int Choice;

        Console.WriteLine("Welcome to the ticket booking system.");

        JsonFileStorage<User> UserStorage = new JsonFileStorage<User>("Users.json");
        Dictionary<string, User> Users = new Dictionary<string, User>(await UserStorage.ReadDictionary(), StringComparer.OrdinalIgnoreCase);
        UserManager userManager = new UserManager(UserStorage, Users);

        JsonFileStorage<Flight> Flightstorage = new JsonFileStorage<Flight>("Flights.json");
        List<Flight> Flights = await Flightstorage.ReadFile();
        FlightManager flightManager = new FlightManager(Flights);

        JsonFileStorage<Booking> BookingStorage = new JsonFileStorage<Booking>("Bookings.json");
        List<Booking> Bookings = await BookingStorage.ReadFile();
        BookingsManager bookingsManager = new BookingsManager(BookingStorage, Bookings, flightManager);

        do
        {
            Console.WriteLine("-- Main Menu --");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. See available flights");
            Console.WriteLine("4. Exit");
            if (!int.TryParse(Console.ReadLine(), out Choice))
            {
                Console.WriteLine("Invalid input. Please enter a number from 1 to 4.");
                continue;
            }

            switch (Choice)
            {
                case 1:
                    {
                        string username = ConsoleInput.ReadRequiredString("Enter Username:");
                        string password = ConsoleInput.ReadRequiredString("Enter Password:");

                        bool registered = await userManager.Register(username, password);
                        if (registered)
                        {
                            Console.WriteLine($"Welcome {userManager.CurrentUser!.Name}!");
                            await RunPassengerMenu(userManager, flightManager, bookingsManager);

                        }
                        else
                        {
                            Console.WriteLine("Username already exists or invalid input.");
                        }
                        break;
                    }

                case 2:
                    {
                        string username = ConsoleInput.ReadRequiredString("Enter Username:");
                        string password = ConsoleInput.ReadRequiredString("Enter Password:");

                        bool loggedIn = userManager.Login(username, password);
                        if (loggedIn)
                        {
                            Console.WriteLine($"Welcome back, {userManager.CurrentUser!.Name}!");
                            if (userManager.CurrentUser.Role == Role.Manager)
                            {
                                await RunManagerMenu(userManager, bookingsManager, flightManager, Flightstorage);
                            }
                            else
                            {
                                await RunPassengerMenu(userManager, flightManager, bookingsManager);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid username or password.");
                        }


                        break;
                    }

                case 3:
                    SearchAndDisplayFlights(flightManager);

                    break;

                case 4:
                    Console.WriteLine("Exiting..");
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please choose from 1 to 4.");
                    break;
            }


        } while (Choice != 4);


    }

    public static async Task RunPassengerMenu(UserManager userManager, FlightManager flightManager, BookingsManager bookingsManager)
    {
        bool loggedIn = true;

        while (loggedIn)
        {
            Console.WriteLine("-- Passenger Menu --");
            Console.WriteLine("1. Book a Flight");
            Console.WriteLine("2. View Available Flights");
            Console.WriteLine("3. View My Bookings");
            Console.WriteLine("4. Modify a Booking");
            Console.WriteLine("5. Cancel a Booking");
            Console.WriteLine("6. Logout");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    {
                        int flightId = ConsoleInput.ReadPositiveInt("Enter Flight ID to book: ");
                        CabinClass cabinClass = ConsoleInput.ReadRequiredCabinClass();

                        bool booked = await bookingsManager.CreateBooking(
                            userManager.CurrentUser!.UserId, flightId, cabinClass);

                        Console.WriteLine(booked ? "Flight booked successfully!"
                            : "Booking failed. Flight not found or cabin class unavailable or already booked.");

                        break;
                    }

                case 2:
                    SearchAndDisplayFlights(flightManager);
                    break;

                case 3:
                    bookingsManager.PrintUserBookings(userManager.CurrentUser!.UserId);
                    break;

                case 4:
                    {
                        int bookingId = ConsoleInput.ReadPositiveInt("Enter Booking ID: ");

                        CabinClass newCabinClass = ConsoleInput.ReadRequiredCabinClass();

                        bool updated = await bookingsManager.UpdateBooking(userManager.CurrentUser!.UserId,
                            bookingId, newCabinClass);

                        Console.WriteLine(updated ? "Booking updated successfully!" : "Booking update failed.");

                        break;
                    }
                case 5:
                    {
                        int bookingId = ConsoleInput.ReadPositiveInt("Enter Booking ID: ");

                        bool cancelled = await bookingsManager
                        .CancelBooking(userManager.CurrentUser!.UserId, bookingId);

                        Console.WriteLine(cancelled ? "Booking cancelled successfully!"
                        : "Booking not found.");

                        break;
                    }
                case 6:
                    userManager.Logout();
                    loggedIn = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    static async Task RunManagerMenu(UserManager userManager, BookingsManager bookingsManager,
                                FlightManager flightManager, JsonFileStorage<Flight> Flightstorage)
    {
        while (true)
        {
            Console.WriteLine("-- Manager Menu --");
            Console.WriteLine("1. Search Bookings");
            Console.WriteLine("2. Batch Flight Upload");
            Console.WriteLine("3. Flight Validation Details");
            Console.WriteLine("4. Logout");

            Console.Write("Choose an option:");

            string choice = Console.ReadLine()?.Trim() ?? "";

            switch (choice)
            {
                case "1":
                    SearchAndDisplayBookings(bookingsManager);
                    break;

                case "2":
                    string filePath = ConsoleInput.ReadRequiredFilePath("Enter CSV file path:");
                    await flightManager.ImportFlightsFromCsv(filePath, Flightstorage);
                    break;

                case "3":
                    ModelValidationDetails validationService = new();
                    validationService.DisplayValidationDetails();

                    break;

                case "4":
                    userManager.Logout();
                    return;

                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
    }

    public static void SearchAndDisplayFlights(FlightManager flightManager)
    {

        string departureCountry = ConsoleInput.ReadRequiredCountry("Leaving from: ");
        string destinationCountry;

        do
        {
            destinationCountry =
                ConsoleInput.ReadRequiredCountry("To: ");

            if (destinationCountry.Equals(
                departureCountry, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Destination must differ from departure country.");
            }

        } while (destinationCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase));


        var criteria = new FlightSearchCriteria
        {
            DepartureCountry = departureCountry,
            DestinationCountry = destinationCountry,
            DepartureDate = ConsoleInput.ReadOptionalDate("Departure Date (Format -> dd/mm/yyyy, press enter to skip)"),
            MaxPrice = ConsoleInput.ReadOptionalPrice("Maximum Price (Enter to skip):"),
            DepartureAirport = ConsoleInput.ReadOptionalString("Departure Airport (Enter to skip):"),
            ArrivalAirport = ConsoleInput.ReadOptionalString("Arrival Airport (Enter to skip):"),
            CabinClass = ConsoleInput.ReadOptionalCabinClass(),
        };

        List<Flight> filteredFlights = flightManager.SearchFlights(criteria);
        Console.WriteLine("\nSearch Results:");
        flightManager.DisplayFlights(filteredFlights);
    }

    static void SearchAndDisplayBookings(BookingsManager bookingsManager)
    {
        Console.WriteLine("\n--- Search Bookings ---");

        var criteria = new BookingSearchCriteria
        {
            FlightId = ConsoleInput.ReadOptionalPositiveInt("Flight ID (Enter to skip): "),
            PassengerId = ConsoleInput.ReadOptionalPositiveInt("Passenger ID (Enter to skip): "),
            DepartureCountry = ConsoleInput.ReadOptionalCountry("Departure Country (Enter to skip): "),
            DestinationCountry = ConsoleInput.ReadOptionalCountry("Destination Country (Enter to skip): "),
            DepartureDate = ConsoleInput.ReadOptionalDate("Departure Date (dd/MM/yyyy, Enter to skip): "),
            DepartureAirport = ConsoleInput.ReadOptionalString("Departure Airport (Enter to skip): "),
            ArrivalAirport = ConsoleInput.ReadOptionalString("Arrival Airport (Enter to skip): "),
            CabinClass = ConsoleInput.ReadOptionalCabinClass(),
            MaxPrice = ConsoleInput.ReadOptionalPrice("Maximum Price (Enter to skip): ")
        };

        List<Booking> results = bookingsManager.SearchBookings(criteria);

        if (results.Count == 0)
        {
            Console.WriteLine("No bookings found matching your search criteria.");
            return;
        }

        Console.WriteLine($"Found {results.Count} booking(s):");
        bookingsManager.DisplayBookings(results);
    }
}

