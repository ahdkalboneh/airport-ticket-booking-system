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

        do
        {
            Console.WriteLine("Choose:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. See available flights");
            Console.WriteLine("4. Exit");
            if (!int.TryParse(Console.ReadLine(), out Choice))
            {
                Console.WriteLine("Invalid input. Please enter a number from 1 to 4.");
                Choice = 0;
            }

        } while (Choice != 1 && Choice != 2 && Choice != 3 && Choice != 4);


        switch (Choice)
        {
            case 1:
                Console.WriteLine("Enter Username:");
                string username = Console.ReadLine() ?? "";

                Console.Write("Enter Password: ");
                string password = Console.ReadLine() ?? "";

                bool registered = await userManager.Register(username, password);
                if (registered)
                {
                    Console.WriteLine($"Welcome {userManager.CurrentUser!.Name}!");
                }
                else
                {
                    Console.WriteLine("Username already exists or invalid input.");
                }
                break;

            case 2:
                break;

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

                var FilteredFlights = Flights.Where(Flight =>
                                        string.Equals(Flight.DepartureCountry, DepartureCountry, StringComparison.OrdinalIgnoreCase)
                                        && string.Equals(Flight.DestinationCountry, DestenationCountry, StringComparison.OrdinalIgnoreCase)
                                        && Flight.DepartureDate.Date == DepartureDate.Date).ToList();

                if (FilteredFlights.Any())
                {
                    foreach (var flight in FilteredFlights)
                    {
                        Console.WriteLine($"Flight ID: {flight.FlightId}");
                        Console.WriteLine($"From: {flight.DepartureCountry}");
                        Console.WriteLine($"To: {flight.DestinationCountry}");
                        Console.WriteLine($"Departure Date: {flight.DepartureDate}");
                        Console.WriteLine($"Departure Airport: {flight.DepartureAirport}");
                        Console.WriteLine($"Arrival Airport: {flight.ArrivalAirport}");
                        Console.WriteLine("Prices:");
                        foreach (var price in flight.Prices)
                        {
                            Console.WriteLine($"{price.Key}: {price.Value}");
                        }
                        Console.WriteLine("-------------------------");
                    }
                }
                else
                {
                    Console.WriteLine($"Sorry. No available flights from {DepartureCountry} to {DestenationCountry} on this date.");
                }

                break;

            default:
                Environment.Exit(0);
                break;
        }


    }
}