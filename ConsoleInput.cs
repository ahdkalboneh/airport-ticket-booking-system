using System.Globalization;
using AirportTicketBookingSystem.Enums;
public static class ConsoleInput
{
    private static readonly string[] Countries =
    {
        "Palestine", "Jordan", "Syria", "Turkey", "Egypt",
        "Qatar", "Saudi Arabia", "Lebanon", "Spain", "UAE", "Germany"
    };

    public static void DisplayAvailableCountries()
    {
        Console.WriteLine("\nAvailable countries:");

        foreach (var country in Countries)
        {
            Console.WriteLine($"- {country}");
        }
    }

    public static string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine("Input cannot be empty!");
        }
    }

    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                return value;

            Console.WriteLine("Please enter a valid positive number.");
        }
    }

    public static CabinClass ReadRequiredCabinClass()
    {
        while (true)
        {
            Console.WriteLine("Choose Cabin Class:");
            Console.WriteLine("1. EconomyClass");
            Console.WriteLine("2. BusinessClass");
            Console.WriteLine("3. FirstClass");
            Console.Write("Your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        return CabinClass.EconomyClass;

                    case 2:
                        return CabinClass.BusinessClass;

                    case 3:
                        return CabinClass.FirstClass;
                }
            }

            Console.WriteLine("Invalid choice. Please choose 1, 2, or 3.");
        }
    }

    public static int? ReadOptionalPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (int.TryParse(input, out int value) && value > 0)
                return value;

            Console.WriteLine("Please enter a valid positive number.");
        }
    }

    public static string? ReadOptionalCountry(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(input))
                return null;

            string? country = Countries.FirstOrDefault(c =>
                c.Equals(input, StringComparison.OrdinalIgnoreCase));

            if (country != null)
                return country;

            Console.WriteLine("Invalid country! Please choose from the available countries.");
            DisplayAvailableCountries();
        }
    }

    public static string ReadRequiredCountry(string prompt)
    {
        while (true)
        {
            string? country = ReadOptionalCountry(prompt);

            if (country != null)
                return country;

            Console.WriteLine("Country cannot be empty!");
        }
    }

    public static string? ReadOptionalString(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()?.Trim() ?? "";

        return string.IsNullOrWhiteSpace(input) ? null : input;
    }

    public static DateTime? ReadOptionalDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (DateTime.TryParseExact(
                input,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                return date;
            }

            Console.WriteLine("Invalid date! Use dd/MM/yyyy.");
        }
    }

    public static decimal? ReadOptionalPrice(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (decimal.TryParse(input, out decimal price) && price >= 0)
                return price;

            Console.WriteLine("Please enter a valid non-negative price.");
        }
    }

    public static CabinClass? ReadOptionalCabinClass()
    {
        while (true)
        {
            Console.WriteLine("\nChoose Cabin Class:");
            Console.WriteLine("1. EconomyClass");
            Console.WriteLine("2. BusinessClass");
            Console.WriteLine("3. FirstClass");
            Console.Write("Your choice (press Enter to skip): ");

            string input = Console.ReadLine()?.Trim() ?? "";

            switch (input)
            {
                case "":
                    return null;
                case "1":
                    return CabinClass.EconomyClass;
                case "2":
                    return CabinClass.BusinessClass;
                case "3":
                    return CabinClass.FirstClass;
                default:
                    Console.WriteLine("Invalid cabin class!");
                    break;
            }
        }
    }
}
