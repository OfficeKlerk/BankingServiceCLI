namespace BankingServiceCLI.Exceptions;

public class ClientNotFoundException : Exception
{
    public Guid ClientId { get; }
    public ClientNotFoundException(Guid id) : base($"No client found with such id: {id}")
    {
        ClientId = id;
    }
}