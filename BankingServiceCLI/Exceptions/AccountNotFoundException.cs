namespace BankingServiceCLI.Exceptions;

public class AccountNotFoundException : Exception
{
    public Guid AccountId {get; set;}

    public AccountNotFoundException(Guid id) : base($"No account found with such id: {id}")
    {
        AccountId = id;
    }
}