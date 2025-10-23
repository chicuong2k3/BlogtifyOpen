namespace Blogtify.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string value, string valueName = "id")
        : base($"{entityName} with {valueName} '{value}' was not found.")
    {

    }
}
