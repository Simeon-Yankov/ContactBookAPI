namespace ContactBookAPI.Domain.Exceptions;

public class InvalidPersonException : BaseDomainException
{
    public InvalidPersonException()
    {
    }

    public InvalidPersonException(string error)
    {
        Error = error;
    }
}
