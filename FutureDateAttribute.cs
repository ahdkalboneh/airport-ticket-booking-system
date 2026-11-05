
using System.ComponentModel.DataAnnotations;
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not DateTime date)
            return false;

        return date.Date >= DateTime.Today;
    }
}
