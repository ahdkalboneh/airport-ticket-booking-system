using System.Globalization;
using AirportTicketBookingSystem.Enums;

public class Program
{
    public static async Task Main(string[] args)
    {

        string DepartureCountry, DestenationCountry, input;
        DateTime DepartureDate;
        int Choice;

        string[] countries = {"Palestine", "Jordan", "Syria", "Turkey", "Egypt",
        "Qatar", "Saudi Arabia", "Lebanon", "Spain", "UAE"};

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
                Choice = 0;
            }

            switch (Choice)
            {
                case 1:
                    {
                        Console.WriteLine("Enter Username:");
                        string username = Console.ReadLine() ?? "";

                        Console.Write("Enter Password: ");
                        string password = Console.ReadLine() ?? "";

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
                        string username, password;
                        do
                        {
                            Console.Write("Enter Username: ");
                            username = Console.ReadLine()?.Trim() ?? "";

                            if (string.IsNullOrWhiteSpace(username))
                                Console.WriteLine("Username cannot be empty!");

                        } while (string.IsNullOrWhiteSpace(username));

                        do
                        {
                            Console.Write("Enter Password: ");
                            password = Console.ReadLine() ?? "";

                            if (string.IsNullOrWhiteSpace(password))
                                Console.WriteLine("Password cannot be empty!");

                        } while (string.IsNullOrWhiteSpace(password));

                        bool loggedIn = userManager.Login(username, password);
                        if (loggedIn)
                        {
                            Console.WriteLine($"Welcome back, {userManager.CurrentUser!.Name}!");
                            await RunPassengerMenu(userManager, flightManager, bookingsManager);
                        }
                        else
                        {
                            Console.WriteLine("Invalid username or password.");
                        }


                        break;
                    }

                case 3:
                    Console.WriteLine("Available countries:");
                    foreach (var country in countries)
                    {
                        Console.WriteLine($"- {country}");
                    }

                    do
                    {
                        Console.WriteLine("Leaving from:");
                        DepartureCountry = Console.ReadLine()?.Trim() ?? "";
                        if (!countries.Contains(DepartureCountry, StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Invalid country! Please enter a country from the list.");
                        }
                    } while (!countries.Contains(DepartureCountry, StringComparer.OrdinalIgnoreCase));

                    do
                    {
                        Console.WriteLine("To:");
                        DestenationCountry = Console.ReadLine()?.Trim() ?? "";
                        if (!countries.Contains(DestenationCountry, StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Invalid country! Please enter a country from the list.");
                        }
                        else if (DestenationCountry.Equals(DepartureCountry, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Destination country must be different from departure country.");
                        }
                    } while (!countries.Contains(DestenationCountry, StringComparer.OrdinalIgnoreCase)
                            || DestenationCountry.Equals(DepartureCountry, StringComparison.OrdinalIgnoreCase));

                    while (true)
                    {
                        Console.WriteLine("Departure date (dd/MM/yyyy):");
                        input = Console.ReadLine()?.Trim() ?? "";
                        if (DateTime.TryParseExact(
                        input,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DepartureDate))
                        {
                            break;
                        }
                        Console.WriteLine("Invalid date! Please use dd/MM/yyyy format.");
                    }

                    List<Flight> FilteredFlights = flightManager.SearchFlights(
                        DepartureCountry, DestenationCountry, DepartureDate);
                    flightManager.DisplayFlights(FilteredFlights);
                    Console.WriteLine("\nTo book a flight, please register or login first.\n");

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
                        Console.Write("Enter Flight ID to book: ");

                        if (!int.TryParse(Console.ReadLine(), out int flightId))
                        {
                            Console.WriteLine("Invalid Flight ID.");
                            break;
                        }

                        Console.WriteLine("Choose Cabin Class:");
                        Console.WriteLine("1. EconomyClass");
                        Console.WriteLine("2. BusinessClass");
                        Console.WriteLine("3. FirstClass");

                        if (!int.TryParse(Console.ReadLine(), out int classChoice))
                        {
                            Console.WriteLine("Invalid choice.");
                            break;
                        }

                        CabinClass cabinClass;

                        switch (classChoice)
                        {
                            case 1:
                                cabinClass = CabinClass.EconomyClass;
                                break;
                            case 2:
                                cabinClass = CabinClass.BusinessClass;
                                break;
                            case 3:
                                cabinClass = CabinClass.FirstClass;
                                break;
                            default:
                                Console.WriteLine("Invalid cabin class.");
                                continue;
                        }

                        bool booked = await bookingsManager.CreateBooking(
                            userManager.CurrentUser!.UserId, flightId, cabinClass);

                        Console.WriteLine(booked ? "Flight booked successfully!"
                            : "Booking failed. Flight not found or cabin class unavailable.");

                        break;
                    }

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                case 5:
                    break;

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
}

