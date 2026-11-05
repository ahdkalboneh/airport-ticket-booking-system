
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AirportTicketBookingSystem.Enums;

public class ModelValidationDetails
{
    public void DisplayValidationDetails()
    {
        Type modelType = typeof(Flight);

        Console.WriteLine($"{modelType.Name} Validation Details");
        Console.WriteLine("---------------------------");

        PropertyInfo[] properties = modelType.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            Console.WriteLine($"{property.Name}");
            Console.WriteLine($"Type: {GetReadableType(property.PropertyType)}");

            var attributes = property.GetCustomAttributes<ValidationAttribute>();

            List<string> constraints = attributes.Select(GetConstraintDetails).ToList();

            if (constraints.Count == 0)
                Console.WriteLine("Constraints: None");
            else
                Console.WriteLine($"Constraints: {string.Join(", ", constraints)}");
        }
    }

    private string GetReadableType(Type type)
    {
        if (type == typeof(int))
            return "Integer";

        if (type == typeof(string))
            return "Free text";

        if (type == typeof(DateTime))
            return "DateTime";

        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Type[] arguments = type.GetGenericArguments();

            return $"Dictionary<{arguments[0].Name}, {arguments[1].Name}>";
        }

        return type.Name;
    }

    private string GetConstraintDetails(ValidationAttribute attribute)
    {
        return attribute switch
        {
            RequiredAttribute => "Required",
            FutureDateAttribute => "Allowed Range: Today → Future",
            _ => attribute.GetType().Name
        };
    }
}
