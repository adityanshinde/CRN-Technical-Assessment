namespace ProductApi.Domain.Exceptions;

/// <summary>
/// Thrown when an operation conflicts with the current state of the resource.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) is in a conflicting state.")
    {
    }
}
