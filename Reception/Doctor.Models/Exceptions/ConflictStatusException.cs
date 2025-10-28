namespace Doctor.Models.Exceptions;

public class ConflictStatusException : Exception
{
    public ConflictStatusException(string message) : base(message)
    {
    }
}