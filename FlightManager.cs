using AirportTicketBookingSystem.Enums;
using CsvHelper;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
public class FlightManager
{

    private readonly List<Flight> Flights;

    public FlightManager(List<Flight> flights)
    {
        Flights = flights;
    }

    public List<Flight> SearchFlights(FlightSearchCriteria criteria)
    {
        return Flights.Where(flight =>
            string.Equals(flight.DepartureCountry, criteria.DepartureCountry, StringComparison.OrdinalIgnoreCase)
            && string.Equals(flight.DestinationCountry, criteria.DestinationCountry, StringComparison.OrdinalIgnoreCase)
            && (criteria.DepartureDate == null || flight.DepartureDate.Date == criteria.DepartureDate.Value.Date)
            && (string.IsNullOrWhiteSpace(criteria.DepartureAirport) ||
            string.Equals(flight.DepartureAirport, criteria.DepartureAirport, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrWhiteSpace(criteria.ArrivalAirport) ||
            string.Equals(flight.ArrivalAirport, criteria.ArrivalAirport, StringComparison.OrdinalIgnoreCase))
            && (criteria.CabinClass == null || flight.Prices.ContainsKey(criteria.CabinClass.Value))
            && (criteria.MaxPrice == null || (criteria.CabinClass != null ? flight.Prices.TryGetValue(criteria.CabinClass.Value, out decimal price)
            && price <= criteria.MaxPrice.Value : flight.Prices.Values.Any(price => price <= criteria.MaxPrice.Value)))
        ).ToList();
    }

    public void DisplayFlights(List<Flight> flights)
    {

        if (!flights.Any())
        {
            Console.WriteLine("No available flights.");
            return;
        }

        foreach (var flight in flights)
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine($"Flight ID: {flight.FlightId}");
            Console.WriteLine($"From: {flight.DepartureCountry}");
            Console.WriteLine($"To: {flight.DestinationCountry}");
            Console.WriteLine($"Departure Date: {flight.DepartureDate}");
            Console.WriteLine($"Departure Airport: {flight.DepartureAirport}");
            Console.WriteLine($"Arrival Airport: {flight.ArrivalAirport}");

            Console.WriteLine("Prices:");

            foreach (var price in flight.Prices)
                Console.WriteLine($"{price.Key}: {price.Value}");
        }
        Console.WriteLine("-------------------------");
    }

    public Flight? GetFlightById(int flightId)
    {
        return Flights.FirstOrDefault(flight => flight.FlightId == flightId);
    }
    public async Task ImportFlightsFromCsv(string filePath, JsonFileStorage<Flight> storage)
    {
        var importedFlights = new List<Flight>();
        var errors = new List<string>();

        int totalRows = 0;
        int rejectedRows = 0;

        try
        {
            using var csv = CsvReader.FromFile(filePath);

            int GetColumnIndex(string columnName)
            {
                int index = csv.GetFieldIndex(columnName);

                if (index < 0)
                    throw new InvalidDataException($"Missing required column: {columnName}");

                return index;
            }

            int flightIdIndex = GetColumnIndex("FlightId");
            int departureCountryIndex = GetColumnIndex("DepartureCountry");
            int destinationCountryIndex = GetColumnIndex("DestinationCountry");
            int departureDateIndex = GetColumnIndex("DepartureDate");
            int departureAirportIndex = GetColumnIndex("DepartureAirport");
            int arrivalAirportIndex = GetColumnIndex("ArrivalAirport");
            int economyPriceIndex = GetColumnIndex("EconomyPrice");
            int businessPriceIndex = GetColumnIndex("BusinessPrice");
            int firstClassPriceIndex = GetColumnIndex("FirstClassPrice");

            var existingFlightIds = Flights.Select(f => f.FlightId).ToHashSet();

            int rowNumber = 1;

            while (csv.Read())
            {
                rowNumber++;
                totalRows++;

                bool valid = true;
                var rowErrors = new List<string>();

                string flightIdText = csv.GetString(flightIdIndex);
                string departureCountry = csv.GetString(departureCountryIndex).Trim();
                string destinationCountry = csv.GetString(destinationCountryIndex).Trim();
                string departureDateText = csv.GetString(departureDateIndex);
                string departureAirport = csv.GetString(departureAirportIndex).Trim();
                string arrivalAirport = csv.GetString(arrivalAirportIndex).Trim();
                string economyText = csv.GetString(economyPriceIndex);
                string businessText = csv.GetString(businessPriceIndex);
                string firstClassText = csv.GetString(firstClassPriceIndex);

                var textFields = new Dictionary<string, string>
                {
                    ["DepartureCountry"] = departureCountry,
                    ["DestinationCountry"] = destinationCountry,
                    ["DepartureAirport"] = departureAirport,
                    ["ArrivalAirport"] = arrivalAirport
                };

                foreach (var field in textFields)
                {
                    if (string.IsNullOrWhiteSpace(field.Value))
                    {
                        rowErrors.Add($"{field.Key} is required.");
                        valid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(departureCountry) &&
                    !string.IsNullOrWhiteSpace(destinationCountry) &&
                    string.Equals(
                        departureCountry,
                        destinationCountry,
                        StringComparison.OrdinalIgnoreCase))
                {
                    rowErrors.Add("DepartureCountry and DestinationCountry cannot be the same.");
                    valid = false;
                }

                if (!int.TryParse(flightIdText, out int flightId))
                {
                    rowErrors.Add("FlightId must be an integer.");
                    valid = false;
                }
                else
                {
                    if (flightId <= 0)
                    {
                        rowErrors.Add("FlightId must be greater than zero.");
                        valid = false;
                    }

                    if (existingFlightIds.Contains(flightId))
                    {
                        rowErrors.Add($"FlightId {flightId} already exists.");
                        valid = false;
                    }
                }

                bool dateParsed = DateTime.TryParseExact(
                    departureDateText,
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime departureDate);

                if (!dateParsed)
                {
                    rowErrors.Add("Invalid DepartureDate. Expected format: yyyy-MM-dd HH:mm.");
                    valid = false;
                }

                bool economyParsed = decimal.TryParse(
                    economyText,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal economyPrice);

                bool businessParsed = decimal.TryParse(
                    businessText,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal businessPrice);

                bool firstClassParsed = decimal.TryParse(
                    firstClassText,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal firstClassPrice);

                if (!economyParsed)
                {
                    rowErrors.Add("EconomyPrice must be a valid decimal.");
                    valid = false;
                }
                else if (economyPrice < 0)
                {
                    rowErrors.Add("EconomyPrice cannot be negative.");
                    valid = false;
                }

                if (!businessParsed)
                {
                    rowErrors.Add("BusinessPrice must be a valid decimal.");
                    valid = false;
                }
                else if (businessPrice < 0)
                {
                    rowErrors.Add("BusinessPrice cannot be negative.");
                    valid = false;
                }

                if (!firstClassParsed)
                {
                    rowErrors.Add("FirstClassPrice must be a valid decimal.");
                    valid = false;
                }
                else if (firstClassPrice < 0)
                {
                    rowErrors.Add("FirstClassPrice cannot be negative.");
                    valid = false;
                }

                if (!valid)
                {
                    rejectedRows++;

                    foreach (string error in rowErrors)
                        errors.Add($"Row {rowNumber}: {error}");

                    continue;
                }

                var prices = new Dictionary<CabinClass, decimal>
                {
                    [CabinClass.EconomyClass] = economyPrice,
                    [CabinClass.BusinessClass] = businessPrice,
                    [CabinClass.FirstClass] = firstClassPrice
                };

                var flight = new Flight(
                    flightId,
                    departureCountry,
                    destinationCountry,
                    departureDate,
                    departureAirport,
                    arrivalAirport,
                    prices
                );

                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(
                    flight,
                    new ValidationContext(flight),
                    validationResults,
                    validateAllProperties: true
                );

                if (!isValid)
                {
                    rejectedRows++;

                    foreach (var result in validationResults)
                    {
                        string field = string.Join(", ", result.MemberNames);
                        errors.Add($"Row {rowNumber}, {field}: {result.ErrorMessage}");
                    }

                    continue;
                }

                importedFlights.Add(flight);
                existingFlightIds.Add(flightId);
            }

            if (importedFlights.Count > 0)
            {
                var updatedFlights = Flights.Concat(importedFlights).ToList();

                await storage.WriteFile(updatedFlights);

                Flights.AddRange(importedFlights);
            }

            Console.WriteLine("\nImport Summary:");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Total Rows: {totalRows}");
            Console.WriteLine($"Imported: {importedFlights.Count}");
            Console.WriteLine($"Rejected: {rejectedRows}");

            if (errors.Count > 0)
            {
                Console.WriteLine("\nValidation Errors:");

                foreach (string error in errors)
                    Console.WriteLine(error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Import failed: {ex.Message}");
        }
    }
}