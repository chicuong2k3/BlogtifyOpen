namespace Blogtify.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string propertyName)
        : base($"A record with the same {propertyName} already exists.")
    {
    }
}