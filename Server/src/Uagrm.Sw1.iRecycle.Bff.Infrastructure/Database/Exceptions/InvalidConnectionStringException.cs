namespace Uagrm.Sw1.iRecycle.Bff.Infrastructure.Database.Exceptions;

public class InvalidConnectionStringException : Exception
{
    public InvalidConnectionStringException(string message) : base(message) { }
}
