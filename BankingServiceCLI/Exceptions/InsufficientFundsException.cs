namespace BankingServiceCLI.Exceptions;

public class InsufficientFundsException : Exception
{
    public decimal Available {get; set;}
    public decimal Requested {get; set;}

    public InsufficientFundsException(decimal available, decimal requested) : 
           base($"insufficient funds in the account - available: {available}, requested: {requested}")
    {
        Available = available;
        Requested = requested;
    }
}