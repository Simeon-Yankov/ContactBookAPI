namespace ContactBookAPI.Domain.Exceptions;

public class IvnalidAddressException : BaseDomainException
{
    public IvnalidAddressException()
    {
    }

    public IvnalidAddressException(string error)
    {
        Error = error;
    }
}
